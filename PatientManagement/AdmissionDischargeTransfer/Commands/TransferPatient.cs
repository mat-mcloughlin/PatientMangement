using System;

namespace PatientManagement.AdmissionDischargeTransfer.Commands
{
    public class TransferPatient
    {
        public TransferPatient(String patientId, int wardNumber)
        {
            PatientId = patientId;
            WardNumber = wardNumber;
        }

        public String PatientId { get; }

        public int WardNumber { get; }
    }
}