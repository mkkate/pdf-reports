# Kreiranje PDF izveštaja

Kreiranje PDF dokumenata u .NET Core‐u, korišćenjem itext7 biblioteke, sa konverzijom HTML strane u PDF stranu.
___

## Sadržaj


## Problem koji se rešava
U mnogim poslovnim sistemima postoji potreba za generisanjem izveštaja u PDF formatu – za štampu, arhiviranje, deljenje korisnicima i druge formalne svrhe. Ručno kreiranje i formatiranje PDF dokumenata je nepraktično i sklono greškama, posebno kada je potrebna konzistentnost izgleda i dinamičko punjenje podacima.

**Ovaj projekat rešava problem automatskog generisanja PDF izveštaja u ASP.NET Core MVC aplikaciji, koristeći unapred formatiranu HTML šablonsku stranicu.**


## Ključne karakteristike izabranih tehnologija


### 🧰 Tehnologije

- **ASP.NET Core MVC** – moderna, open-source, višeslojna platforma za razvoj web aplikacija.
- **iText 7** – robusna biblioteka za rad sa PDF dokumentima u .NET aplikacijama.
- **Bogus** – biblioteka za generisanje lažnih (mock) podataka tokom testiranja.
- **HTML + CSS** – korišćeni za šablon koji se konvertuje u PDF pomoću iText.


### Konkurentna rešenja

- **IronPdf**

Biblioteka koja jednostavno pretvara HTML i CSS u PDF, ima podršku za JavaScript rendering (bootstrap, jQuery) i lako se ubacuju header-i, footer-i i stilizovane tabele.
Glavni nedostaci su to što nije open-source, a besplatna verzija dodaje watermark.
  
- **QuestPdf**

Ovo je open-source biblioteka (MIT licenca), koja vrši izuzetno precizno generisanje PDF dokumenta - koristi fluent API pristup (npr. `Container().Row().Column()`).
Međutim, ne podržava direktnu konverziju HTML u PDF, već se sadržaj mora kreirati u C#-u koristeći layout komponente, kao na primer:
```
page.Content()
    .Column(column =>
    {
        column.Item().Text("Izveštaj zaposlenih");
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Cell().Text("Ime");
            table.Cell().Text("Pozicija");
        });
    });
```


### Prednosti iText7 biblioteke u odnosu na konkurentna rešenja

U poređenju sa navedenim konkurentnim rešenjima, **iText7** omogućava:
- Konverziju već postojećih HTML šablona u PDF bez potrebe za pisanjem layout koda.
- Visoku kontrolu nad izgledom PDF-a (margine, fontovi, slike, linkovi, paginacija).
- Fleksibilnu integraciju u ASP.NET Core.
- Jednostavno ubacivanje dinamičkog sadržaja (npr. tabele sa korisnicima).

Ovo je posebno važno u projektima gde postoji potreba za:
- Već postojećim HTML/CSS šablonima (kao u ovom slučaju).
- Podrška za specijalne karaktere (npr. ćirilica).
- Visokim kvalitetom štampe za formalne dokumente (npr. akademski izveštaji, fakture).


## Implementacija

### Priprema PDF sadržaja
Pritiskom na `Generate PDF report` dugme na početnoj strani, izvršava se `GeneratePdfReport()` metoda koja:
1. Pribavlja sve korisnike - `var users = GetAll();`
2. Učitava HTML šablon iz navedenog fajla ([AllEmployees.html](#AllEmployees.html)), koji se koristi kao osnova za HTML sadržaj koji će biti konvertovan u PDF.
```
var htmlTemplate = File.ReadAllText("EmailTemplates/AllEmployees.html");
```
3. Dinamički generiše HTML redove u tabeli, tako što za svakog korisnika kreira `<tr>` oznaku i za svako svojstvo (property) objekta kreira `<td>`. 
Korišćenjem `HtmlEncode` se sprečavaju XSS (Cross site scripting) napadi i greške u prikazu zbog specijalnih karaktera.
```
var rows = new StringBuilder();
foreach (var user in users)
{
    rows.AppendLine($@"
        <tr>
            <td>{user.Id}</td>
            <td>{WebUtility.HtmlEncode(user.FirstName)} {WebUtility.HtmlEncode(user.LastName)}</td>
            <td>{WebUtility.HtmlEncode(user.EmploymentStartDate.ToString(dateFormat))}</td>
            <td>{WebUtility.HtmlEncode(user.EmploymentEndDate.HasValue ? user.EmploymentEndDate.Value.ToString(dateFormat) : "-")}</td>
            <td>{WebUtility.HtmlEncode(user.DaysOff.Vacation.ToString())}</td>
            <td>{WebUtility.HtmlEncode(user.DaysOff.Paid.ToString())}</td>
            <td>{WebUtility.HtmlEncode(user.DaysOff.Unpaid.ToString())}</td>
            <td>{WebUtility.HtmlEncode(user.DaysOff.SickLeave.ToString())}</td>
            <td>{WebUtility.HtmlEncode(user.Position.SeniorityLevel)}</td>
            <td>{WebUtility.HtmlEncode(user.Position.Title)}</td>
        </tr>");
}
```
4. Generisani HTML redovi dalje zamenjuju placeholder `{{EMPLOYEE_ROWS}}`, čime je tabela finalizirana.
```
htmlTemplate = htmlTemplate.Replace("{{EMPLOYEE_ROWS}}", rows.ToString());
```

### Priprema PDF dokumenta
1. Kreiraju se:
- `PdfWriter` - upisuje bajtove u `MemoryStream` dok se dokument kreira
- `PdfDocument` - glavni objekat koji opisuje PDF dokument - upravlja stranicama, sadržajem, događajima, metapodacima i internim strukturama PDF-a
```
var pdfWriter = new PdfWriter(stream);
var pdfDocument = new PdfDocument(pdfWriter);
```
2. Postavljaju se metapodaci PDF-a, koji se dodaju i `Info` sekciju PDF-a. Ovi podaci se ne vide direktno u samom sadržaju dokumenta, ali je koristan u pretragama.
```
pdfDocument.GetDocumentInfo()
    .SetTitle("Employees report")
    .SetAuthor("Katarina Mladenovic")
    .SetSubject("Report of all employees")
    .SetKeywords("employees, report, IT company");
```
3. Dodaju se Header i Footer na svaku stranicu:
- `HeaderEventHandler` - crta zaglavlje pre nego što se stranica iscrta
- `FooterEventHandler` - dodaje podnožje nakon što se završi iscrtavanje stranice
Ovime se header i footer automatski pojavljuju na svakoj stranici, čak i kada se tabela nastavlja na više stranica. Više o njihovoj implementaciji [ovde](#Header-i-Footer-Event-Handler-i).
```
pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderEventHandler());
pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandler());
```
4. Konfigurišu se dimenzije i margine dokumenta
- Kreiranjem `Document` objekta omogućava se rad sa "high-level" elementima, kao što su tabele, pasusi, itd.
- Postavljamo *A4* format stranice
- `false` onemogućava automatsko iscrtavanje prve stranice, veoma bitno kod konverzije HTML-a
- Postavljaju se margine redom: gore, desno, dole, levo.
```
var document = new Document(pdfDocument, PageSize.A4, false);
document.SetMargins(100, 36, 60, 36);
```
5. Konverzija HTML sadržaja u PDF
U ovom koraku se zapravo vrši prikaz u PDF
- `ConverterProperties` - omogućava dodatne postavke za konverziju, kao što su stilovi, fontovi, slike
- `HtmlConverter.ConvertToPdf` - parsuje `htmlTemplate`, tumači stilove i render-uje sadržaj u `pdfDocument` poštujući gore navedene margine.
```
var converterProperties = new ConverterProperties();
HtmlConverter.ConvertToPdf(htmlTemplate, pdfDocument, converterProperties);
```
6. Zatvaranje dokumenta - mora se eksplicitno zatvoriti, kako bi svi delovi dokumenata bili upisani u stream.
```
pdfDocument.Close();
```

### AllEmployees.html

```
<!DOCTYPE html>
<html>
<head>
    ...
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 80px 36px 60px 36px;
        }
        table {
            width: 100%;
            border-collapse: collapse;
            page-break-inside: auto;
        }
        thead {
            /*key for repeating header*/
            display: table-header-group;
        }
        tbody {
            display: table-row-group;
        }
        tr {
            page-break-inside: avoid;
            page-break-after: auto;
        }
        ...
        td {
            page-break-inside: avoid;
        }
        ...
    </style>
</head>
<body>
    ...
    <table>
        <thead>
            ...
        </thead>
        <tbody>
            {{EMPLOYEE_ROWS}}
        </tbody>
    </table>
</body>
</html>
```

### Header i Footer Event Handler-i
To su event handler-i koji nasleđuju `AbstractPdfDocumentEventHandler` iz `iText7` biblioteke, i reaguju na događaje prilikom pravljenja svake stranice (`START_PAGE` i `END_PAGE` događaji).

Da bismo lakše ispratili kakav će biti sadržaj PDF-a, na početnoj stranici aplikacije je prikazana tabela sa 40 generisanih zaposlenih, koristeći Bogus biblioteku.

![image](https://github.com/user-attachments/assets/779da9e5-fb79-49ca-a0a6-4328f2194e71)


