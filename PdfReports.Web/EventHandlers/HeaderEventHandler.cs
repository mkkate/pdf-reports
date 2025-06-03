using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Event;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace PdfReports.Web.EventHandlers
{
    public class HeaderEventHandler : AbstractPdfDocumentEventHandler
    {
        readonly PdfFont _pdfFont;

        public HeaderEventHandler()
        {
            // Creating a font inside the event handler on every page event can be expensive.
            // Usually, you want to create the font once (in constructor or as a static/shared field) and reuse it.
            _pdfFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        }

        protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event)
        {
            var docEvent = (PdfDocumentEvent)@event;
            var page = docEvent.GetPage();
            var pdfDoc = docEvent.GetDocument();
            var pageSize = page.GetPageSize();

            var canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
            var canvasModel = new Canvas(canvas, pageSize);

            float logoHeight = 0;
            string[] logos = ["elfak-logo.png", "univerzitet-nis.png"];
            foreach (var logo in logos)
            {
                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "icons", logo);
                if (File.Exists(logoPath))
                {
                    var logoImageData = ImageDataFactory.Create(logoPath);
                    var logoImage = new Image(logoImageData)
                        .ScaleToFit(60, 60);

                    if (logo == logos[0])
                    {
                        logoImage.SetFixedPosition(pageSize.GetLeft() + 70, pageSize.GetTop() - 70);
                    }
                    else
                    {
                        float rightPosition = pageSize.GetRight() - 70 - logoImage.GetImageScaledWidth();
                        logoImage.SetFixedPosition(rightPosition, pageSize.GetTop() - 70);
                    }

                    canvasModel.Add(logoImage);
                    logoHeight = logoImage.GetImageScaledHeight();
                }
            }

            canvasModel
                .SetFont(_pdfFont)
                .SetFontSize(12)
                .ShowTextAligned("University of Nis\nFaculty of Electrical Engineering",
                                 pageSize.GetWidth() / 2,
                                 pageSize.GetTop() - logoHeight + 10,
                                 TextAlignment.CENTER);

            canvasModel.Close();
        }
    }
}
