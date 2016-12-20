using System;

namespace PatientManagement.AdmissionDischargeTransfer
{
    public class PatientAdmitted
    {
        public PatientAdmitted(Guid patientId, string patientName, int ageInYears, int wardNumber)
        {
            PatientId = patientId;
            PatientName = patientName;
            AgeInYears = ageInYears;
            WardNumber = wardNumber;
        }

        public Guid PatientId { get; }

        public string PatientName { get; }

        public int AgeInYears { get; }

        public int WardNumber { get; }
    }
}