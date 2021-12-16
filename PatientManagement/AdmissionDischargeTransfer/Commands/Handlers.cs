using PatientManagement.Framework;
using PatientManagement.Framework.Commands;

namespace PatientManagement.AdmissionDischargeTransfer.Commands;

public class Handlers : CommandHandler
{
    public Handlers(AggregateRepository repository)
    {
        Register<AdmitPatient>(async c =>
        {
            var (patientId, patientName, ageInYears, _, wardNumber) = c;
            var encounter = new Encounter(patientId, patientName, ageInYears, wardNumber);
            await repository.Save(encounter);
        });

        Register<TransferPatient>(async c =>
        {
            var (patientId, wardNumber) = c;
            var encounter = await repository.Get<Encounter>(patientId);
            encounter.Transfer(wardNumber);
            await repository.Save(encounter);
        });

        Register<DischargePatient>(async c =>
        {
            var encounter = await repository.Get<Encounter>(c.PatientId);
            encounter.DischargePatient();
            await repository.Save(encounter);
        });
    }
}