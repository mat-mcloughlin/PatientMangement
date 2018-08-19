using System;

namespace PatientManagement.AdmissionDischargeTransfer
{
    public class PatientDischarged
    {
        public PatientDischarged(String patientId)
        {
            PatientId = patientId;
        }

        public String PatientId { get; }
    }
}