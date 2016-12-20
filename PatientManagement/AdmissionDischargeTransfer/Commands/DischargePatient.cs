using System;

namespace PatientManagement.AdmissionDischargeTransfer.Commands
{
    public class DischargePatient
    {
        public DischargePatient(Guid patientId)
        {
            PatientId = patientId;
        }

        public Guid PatientId { get; }
    }
}