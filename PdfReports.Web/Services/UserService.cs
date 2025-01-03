using Bogus;
using PdfReports.Web.Models;

namespace PdfReports.Web.Services
{
    public class UserService
    {
        public List<User> Users { get; set; } = new();

        public List<User> GenerateUsers(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var faker = new Faker();

                //var Id = i + 1;
                //var FirstName = faker.Name.FirstName();
                //var LastName = faker.Name.LastName();
                //var Address = faker.Address.FullAddress();
                //var EmploymentStartDate = faker.Date.PastDateOnly(10);
                //DateOnly? EmploymentEndDate = faker.Random.Bool() ? faker.Date.FutureDateOnly(5) : null;
                //var DaysOff = new DaysOff
                //{
                //    Vacation = faker.Random.Number(0, 22),
                //    Paid = faker.Random.Number(0, 10),
                //    Unpaid = faker.Random.Number(0, 10),
                //    SickLeave = faker.Random.Number(0, 10)
                //};
                //var Position = new Position
                //{
                //    Title = faker.Name.JobTitle(),
                //    SeniorityLevel = faker.PickRandom(new[] { "Junior", "Medior", "Senior" })
                //};

                //Users.Add(new User
                //{
                //    Id = Id,
                //    FirstName = FirstName,
                //    LastName = LastName,
                //    Address = Address,
                //    EmploymentStartDate = EmploymentStartDate,
                //    EmploymentEndDate = EmploymentEndDate,
                //    DaysOff = DaysOff,
                //    Position = Position
                //});

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

        //public List<User> Create()
        //{
        //    return GenerateUsers(1);
        //}

        public User Create(User userModel)
        {
            Users.Add(userModel);
            return userModel;
        }
    }
}
