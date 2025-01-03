using Bogus;
using PdfReports.Web.Models;

namespace PdfReports.Web.Services
{
    public class UserService
    {
        public List<User> Users { get; set; } = new();

        public UserService()
        {
            Users = GenerateUsers(10);
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

        public User Create(User userModel)
        {
            userModel.Id = Users.Count + 1;
            Users.Add(userModel);
            return userModel;
        }
    }
}
