namespace PdfReports.Web.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateOnly EmploymentStartDate { get; set; }
        public DateOnly? EmploymentEndDate { get; set; }
        public DaysOff DaysOff { get; set; } = new DaysOff();
        public Position Position { get; set; } = new Position();
    }
}
