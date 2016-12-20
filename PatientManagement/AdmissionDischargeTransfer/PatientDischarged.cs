using System;

namespace PatientManagement.AdmissionDischargeTransfer
{
    public class PatientDischarged
    {
        public PatientDischarged(Guid patientId)
        {
            PatientId = patientId;
        }

        public Guid PatientId { get; }
    }
}