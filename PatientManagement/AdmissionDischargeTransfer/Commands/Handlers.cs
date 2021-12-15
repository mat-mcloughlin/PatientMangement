using PatientManagement.Framework;
using PatientManagement.Framework.Commands;

namespace PatientManagement.AdmissionDischargeTransfer.Commands;

public class Handlers : CommandHandler
{
    public Handlers(AggregateRepository repository)
    {
        Register<AdmitPatient>(async (command, ct) =>
        {
            var (patientId, patientName, ageInYears, _, wardNumber) = command;
            var encounter = new Encounter(patientId, patientName, ageInYears, wardNumber);
            await repository.SaveAsync(encounter, ct);
        });

        Register<TransferPatient>(async (command, ct) =>
        {
            var (patientId, wardNumber) = command;
            var encounter = await repository.GetAsync<Encounter>(patientId, ct);
            encounter.Transfer(wardNumber);
            await repository.SaveAsync(encounter, ct);
        });

        Register<DischargePatient>(async (command, ct) =>
        {
            var encounter = await repository.GetAsync<Encounter>(command.PatientId, ct);
            encounter.DischargePatient();
            await repository.SaveAsync(encounter, ct);
        });
    }
}