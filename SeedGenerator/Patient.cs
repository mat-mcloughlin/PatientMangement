using System;
using System.Collections.Generic;
using RandomNameGenerator;

namespace SeedGenerator
{
    class Patient
    {
        public static List<Patient> Generate(int number)
        {
            var list = new List<Patient>();
            var dateRandom = new Random();

            for (var i = 0; i < number; i++)
            {
                list.Add(new Patient
                {
                    Id = Guid.NewGuid(),
                    Name = i % 2 == 0 ? NameGenerator.Generate(Gender.Female) : NameGenerator.Generate(Gender.Male),
                    BirthDate = DateTime.UtcNow.AddDays(dateRandom.Next(25550) * -1).Date
                });
            }

            return list;
        }

        public Guid Id { get; set; }

        public DateTime BirthDate { get; set; }

        public string Name { get; set; }

        public int Age => DateTime.UtcNow.Year - BirthDate.Year;
    }
}