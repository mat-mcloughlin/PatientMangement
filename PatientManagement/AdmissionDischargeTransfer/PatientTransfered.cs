using System;

namespace PatientManagement.AdmissionDischargeTransfer
{
    public class PatientTransfered
    {
        public PatientTransfered(Guid patientId, int wardNumber)
        {
            PatientId = patientId;
            WardNumber = wardNumber;
        }

        public Guid PatientId { get; }

        public int WardNumber { get; }
    }
}