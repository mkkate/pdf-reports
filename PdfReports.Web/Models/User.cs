using Bogus;
using PdfReports.Web.Constants;
using System.ComponentModel;

namespace PdfReports.Web.Models
{
    public class User
    {
        //public List<User> Users { get; set; }

        //public User()
        //{

        //}

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

        //private void CreateUsers(int count)
        //{
        //    Users = new List<User>();
        //    for (int i = 0; i < count; i++)
        //    {
        //        var faker = new Faker();

        //        Users.Add(new User
        //        {
        //            Id = i + 1,
        //            FirstName = faker.Name.FirstName(),
        //            LastName = faker.Name.LastName(),
        //            Address = faker.Address.FullAddress(),
        //            EmploymentStartDate = faker.Date.PastDateOnly(10),
        //            EmploymentEndDate = faker.Random.Bool() ? faker.Date.FutureDateOnly(5) : null,
        //            DaysOff = new DaysOff
        //            {
        //                Vacation = faker.Random.Number(0, 20),
        //                Paid = faker.Random.Number(0, 10),
        //                Unpaid = faker.Random.Number(0, 10),
        //                SickLeave = faker.Random.Number(0, 10)
        //            },
        //            Position = new Position
        //            {
        //                Title = faker.Name.JobTitle(),
        //                SeniorityLevel = faker.PickRandom(new[] { "Junior", "Medior", "Senior" })
        //            }
        //        });
        //    }
        //}
    }
}
