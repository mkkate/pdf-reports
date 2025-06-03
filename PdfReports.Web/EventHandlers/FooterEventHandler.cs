using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Event;
using iText.Layout;
using iText.Layout.Properties;

namespace PdfReports.Web.EventHandlers
{
    public class FooterEventHandler : AbstractPdfDocumentEventHandler
    {
        readonly PdfFont _pdfFont;

        public FooterEventHandler()
        {
            _pdfFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);
        }

        protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event)
        {
            var docEvent = (PdfDocumentEvent)@event;
            var page = docEvent.GetPage();
            var pdfDoc = docEvent.GetDocument();
            var pageSize = page.GetPageSize();

            var canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
            var canvasModel = new Canvas(canvas, pageSize);

            string footerText = $"Pdf reports - Page {pdfDoc.GetPageNumber(page)}";

            canvasModel
                .SetFont(_pdfFont)
                .SetFontSize(10)
                .ShowTextAligned(footerText,
                                 pageSize.GetWidth() / 2,
                                 pageSize.GetBottom() + 20,  // 20 units from bottom
                                 TextAlignment.CENTER);

            canvasModel.Close();
        }
    }
}
