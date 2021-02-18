using System;
using MongoDB.Driver;
using PatientManagement.AdmissionDischargeTransfer;

namespace ProjectionManager
{
    internal class WardViewProjection : Projection
    {
        public WardViewProjection(IMongoDatabase database)
        {
            var collection = database.GetCollection<Patient>("WardView");

            When<PatientAdmitted>(e =>
            {
                collection.InsertOne(new Patient
                {
                    Id = e.PatientId,
                    WardNumber = e.WardNumber,
                    PatientName = e.PatientName,
                    AgeInYears = e.AgeInYears
                });

                Console.WriteLine($"Recording Patient Admission: {e.PatientName}");
            });

            When<PatientTransferred>(e =>
            {
                var filter = Builders<Patient>.Filter.Eq(x => x.Id, e.PatientId);
                var update = Builders<Patient>.Update.Set(a => a.WardNumber, e.WardNumber);

                collection.UpdateOneAsync(filter, update);

                Console.WriteLine($"Recording Patient Transfer: {e.PatientId}");
            });

            When<PatientDischarged>(e =>
            {
                var filter = Builders<Patient>.Filter.Eq(x => x.Id, e.PatientId);

                collection.FindOneAndDelete(filter);

                Console.WriteLine($"Recording Patient Discharged: {e.PatientId}");
            });
        }
    }

    public class Patient
    {
        public Guid Id { get; set; }

        public int WardNumber { get; set; }

        public string PatientName { get; set; }

        public int AgeInYears { get; set; }
    }
}
