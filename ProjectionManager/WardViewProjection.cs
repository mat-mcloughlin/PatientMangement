using System;
using PatientManagement.AdmissionDischargeTransfer;

namespace ProjectionManager;

public class WardViewProjection : Projection
{
    public WardViewProjection(ConnectionFactory connectionFactory)
    {
        When<PatientAdmitted>(async (e, ct) =>
        {
            using var session = connectionFactory.Connect();
            await session.StoreAsync(new Patient
            {
                Id = e.PatientId.ToString(),
                WardNumber = e.WardNumber,
                PatientName = e.PatientName,
                AgeInYears = e.AgeInYears
            }, ct);

            await session.SaveChangesAsync(ct);

            Console.WriteLine($"Recording Patient Admission: {e.PatientName}");
        });

        When<PatientTransfered>(async (e, ct) =>
        {
            using var session = connectionFactory.Connect();
            var patient = await session.LoadAsync<Patient>(e.PatientId.ToString(), ct);
            patient.WardNumber = e.WardNumber;
            await session.SaveChangesAsync(ct);

            Console.WriteLine($"Recording Patient Transfer: {e.PatientId}");
        });

        When<PatientDischarged>(async (e, ct) =>
        {
            using var session = connectionFactory.Connect();
            var patient = await session.LoadAsync<Patient>(e.PatientId.ToString(), ct);
            session.Delete(patient);

            await session.SaveChangesAsync(ct);

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