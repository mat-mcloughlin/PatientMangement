using System;

namespace PatientManagement.AdmissionDischargeTransfer.Commands
{
    public class DischargePatient
    {
        public DischargePatient(String patientId)
        {
            PatientId = patientId;
        }

        public String PatientId { get; }
    }
}