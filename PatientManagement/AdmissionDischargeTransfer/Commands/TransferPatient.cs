using System;

namespace PatientManagement.AdmissionDischargeTransfer.Commands
{
    public class TransferPatient
    {
        public TransferPatient(Guid patientId, int wardNumber)
        {
            PatientId = patientId;
            WardNumber = wardNumber;
        }

        public Guid PatientId { get; }

        public int WardNumber { get; }
    }
}