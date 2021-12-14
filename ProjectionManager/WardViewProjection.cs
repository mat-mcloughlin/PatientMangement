using System;
using PatientManagement.AdmissionDischargeTransfer;

namespace ProjectionManager;

internal class WardViewProjection : Projection
{
    public WardViewProjection(ConnectionFactory connectionFactory)
    {
        When<PatientAdmitted>(e =>
        {
            using var session = connectionFactory.Connect();
            session.Store(new Patient
            {
                Id = e.PatientId.ToString(),
                WardNumber = e.WardNumber,
                PatientName = e.PatientName,
                AgeInYears = e.AgeInYears
            });

            session.SaveChanges();

            Console.WriteLine($"Recording Patient Admission: {e.PatientName}");
        });

        When<PatientTransfered>(e =>
        {
            using var session = connectionFactory.Connect();
            var patient = session.Load<Patient>(e.PatientId.ToString());
            patient.WardNumber = e.WardNumber;
            session.SaveChanges();

            Console.WriteLine($"Recording Patient Transfer: {e.PatientId}");
        });

        When<PatientDischarged>(e =>
        {
            using var session = connectionFactory.Connect();
            var patient = session.Load<Patient>(e.PatientId.ToString());
            session.Delete(patient);

            session.SaveChanges();

            Console.WriteLine($"Recording Patient Discharged: {e.PatientId}");
        });
    }
}

public class Patient
{
    public string Id { get; set; } = default!;

    public int WardNumber { get; set; }

    public string PatientName { get; set; } = default!;

    public int AgeInYears { get; set; }
}