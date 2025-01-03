using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using PdfReports.Web.Constants;
using System.ComponentModel;

namespace PdfReports.Web.Models
{
    public class Position
    {
        public string Title { get; set; }

        [DisplayName(DisplayNameConstants.SeniorityLevel)]
        public string SeniorityLevel { get; set; }
    }
}
