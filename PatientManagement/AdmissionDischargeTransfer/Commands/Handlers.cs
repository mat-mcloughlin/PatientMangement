using PatientManagement.Framework;
using PatientManagement.Framework.Commands;

namespace PatientManagement.AdmissionDischargeTransfer.Commands
{
    public class Handlers : CommandHandler
    {
        public Handlers(AggregateRepository repository)
        {
            Register<AdmitPatient>(async c =>
            {
                var encounter = new Encounter(c.PatientId, c.PatientName, c.AgeInYears, c.WardNumber);
                await repository.Save(encounter);
            });

            Register<TransferPatient>(async c =>
            {
                var encounter = await repository.Get<Encounter>(c.PatientId);
                encounter.Transfer(c.WardNumber);
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
}