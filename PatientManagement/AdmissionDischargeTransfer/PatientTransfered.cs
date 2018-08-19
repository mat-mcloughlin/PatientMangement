using System;

namespace PatientManagement.AdmissionDischargeTransfer
{
    public class PatientTransfered
    {
        public PatientTransfered(String patientId, int wardNumber)
        {
            PatientId = patientId;
            WardNumber = wardNumber;
        }

        public String PatientId { get; }

        public int WardNumber { get; }
    }
}