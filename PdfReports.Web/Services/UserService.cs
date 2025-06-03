using Bogus;
using iText.Html2pdf;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Event;
using iText.Layout;
using PdfReports.Web.EventHandlers;
using PdfReports.Web.Models;
using System.Net;
using System.Text;

namespace PdfReports.Web.Services
{
    public class UserService
    {
        public List<User> Users { get; set; } = new();

        public UserService()
        {
            Users = GenerateUsers(40);
        }

        public List<User> GenerateUsers(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var faker = new Faker();

                Users.Add(new User
                {
                    Id = i + 1,
                    FirstName = faker.Name.FirstName(),
                    LastName = faker.Name.LastName(),
                    Address = faker.Address.FullAddress(),
                    EmploymentStartDate = faker.Date.PastDateOnly(10),
                    EmploymentEndDate = faker.Random.Bool() ? faker.Date.FutureDateOnly(5) : null,
                    DaysOff = new DaysOff
                    {
                        Vacation = faker.Random.Number(0, 22),
                        Paid = faker.Random.Number(0, 10),
                        Unpaid = faker.Random.Number(0, 10),
                        SickLeave = faker.Random.Number(0, 10)
                    },
                    Position = new Position
                    {
                        Title = faker.Name.JobTitle(),
                        SeniorityLevel = faker.PickRandom(new[] { "Junior", "Medior", "Senior" })
                    }
                });
            }
            return Users;
        }

        public List<User> GetAll()
        {
            return Users;
        }

        public byte[] GeneratePdfReport()
        {
            var users = GetAll();
            var htmlTemplate = File.ReadAllText("EmailTemplates/AllEmployees.html");
            const string dateFormat = "dd-MM-yyyy";

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

            htmlTemplate = htmlTemplate.Replace("{{EMPLOYEE_ROWS}}", rows.ToString());

            using (var stream = new MemoryStream())
            {
                var pdfWriter = new PdfWriter(stream);
                var pdfDocument = new PdfDocument(pdfWriter);

                // PDF document metadata
                pdfDocument.GetDocumentInfo()
                    .SetTitle("Employees report")
                    .SetAuthor("Katarina Mladenovic")
                    .SetSubject("Report of all employees")
                    .SetKeywords("employees, report, IT company");

                pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderEventHandler());
                pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandler());

                var document = new Document(pdfDocument, PageSize.A4, false);
                document.SetMargins(100, 36, 60, 36);

                var converterProperties = new ConverterProperties();
                HtmlConverter.ConvertToPdf(htmlTemplate, pdfDocument, converterProperties);

                pdfDocument.Close();

                return stream.ToArray();
            }
        }
    }
}
