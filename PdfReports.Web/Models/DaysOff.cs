using PdfReports.Web.Constants;
using System.ComponentModel;

namespace PdfReports.Web.Models
{
    public class DaysOff
    {
        public int? Vacation { get; set; }

        public int? Paid { get; set; }

        public int? Unpaid { get; set; }

        [DisplayName(DisplayNameConstants.SickLeave)]
        public int? SickLeave { get; set; }
    }
}
