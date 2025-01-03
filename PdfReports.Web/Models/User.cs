using Bogus;
using PdfReports.Web.Constants;
using System.ComponentModel;

namespace PdfReports.Web.Models
{
    public class User
    {
        public int Id { get; set; }

        [DisplayName(DisplayNameConstants.FirstName)]
        public string FirstName { get; set; }

        [DisplayName(DisplayNameConstants.LastName)]
        public string LastName { get; set; }

        [DisplayName(DisplayNameConstants.Address)]
        public string Address { get; set; }

        [DisplayName(DisplayNameConstants.StartDate)]
        public DateOnly EmploymentStartDate { get; set; }

        [DisplayName(DisplayNameConstants.EndDate)]
        public DateOnly? EmploymentEndDate { get; set; }


        public DaysOff DaysOff { get; set; } = new DaysOff();

        [DisplayName(DisplayNameConstants.Position)]
        public Position Position { get; set; } = new Position();
    }
}
