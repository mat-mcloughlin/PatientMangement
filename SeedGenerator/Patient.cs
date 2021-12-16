using System;
using System.Collections.Generic;
using Bogus;
using Bogus.DataSets;

namespace SeedGenerator;

class Patient
{
    public static List<Patient> Generate(int number)
    {
        var testOrders = new Faker<Patient>()
            .StrictMode(true)
            .RuleFor(o => o.Id, _ => Guid.NewGuid())
            .RuleFor(o => o.Name, f => f.Name.FullName(f.PickRandom<Name.Gender>()))
            .RuleFor(o => o.BirthDate, f => f.Date.Past(100));

        return testOrders.Generate(number);
    }

    public Guid Id { get; set; }

    public DateTime BirthDate { get; set; }

    public string Name { get; set; } = default!;

    public int Age => DateTime.UtcNow.Year - BirthDate.Year;
}