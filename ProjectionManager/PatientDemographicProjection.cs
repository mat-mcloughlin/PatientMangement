using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using PatientManagement.AdmissionDischargeTransfer;

namespace ProjectionManager
{
    internal class PatientDemographicProjection : Projection
    {
        public PatientDemographicProjection(IMongoDatabase database)
        {
            var collection = database.GetCollection<Range>("Range");

            When<PatientAdmitted>(e =>
            {
                var rangeLookup = RangeLookup.Get(e.AgeInYears);

                var filter = Builders<Range>.Filter.Eq(x => x.Id, rangeLookup.Name);
                var update = Builders<Range>.Update.Inc(x => x.Count, 1);
                var options = new UpdateOptions() {IsUpsert = true};

                collection.UpdateOne(filter, update, options);
            });
        }
    }

    public class RangeLookup
    {
        static readonly List<RangeLookup> Ranges = new()
        {
            new() {Name = "0 - 10", Min = 0, Max = 10},
            new() {Name = "11 - 20", Min = 11, Max = 20},
            new() {Name = "21 - 30", Min = 21, Max = 30},
            new() {Name = "31 - 40", Min = 31, Max = 40},
            new() {Name = "41 - 50", Min = 41, Max = 50},
            new() {Name = "51 - 60", Min = 51, Max = 60},
            new() {Name = "61 - 70", Min = 61, Max = 70},
            new() {Name = "71+", Min = 71, Max = int.MaxValue}
        };

        public string Name { get; set; }

        public int Min { get; set; }

        public int Max { get; set; }

        public static RangeLookup Get(int ageInYears)
        {
            return Ranges.Single(r => r.Min <= ageInYears && ageInYears <= r.Max);
        }
    }

    public class Range
    {
        public string Id { get; set; }

        public int Count { get; set; }
    }
}
