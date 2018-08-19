using System;

namespace PatientManagement.AdmissionDischargeTransfer
{
    public class PatientAdmitted
    {
        public PatientAdmitted(String patientId, string patientName, int ageInYears, int wardNumber)
        {
            PatientId = patientId;
            PatientName = patientName;
            AgeInYears = ageInYears;
            WardNumber = wardNumber;
        }

        public String PatientId { get; }

        public string PatientName { get; }

        public int AgeInYears { get; }

        public int WardNumber { get; }
    }
}