using System;

namespace PatientManagement.AdmissionDischargeTransfer.Commands
{
    public class AdmitPatient
    {
        public AdmitPatient(Guid patientId, string patientName, int ageInYears, DateTime timeOfAdmission, int wardNumber)
        {
            PatientId = patientId;
            PatientName = patientName;
            AgeInYears = ageInYears;
            TimeOfAdmission = timeOfAdmission;
            WardNumber = wardNumber;
        }

        public Guid PatientId { get; }

        public string PatientName { get; }

        public int AgeInYears { get; }

        public DateTime TimeOfAdmission { get; }

        public int WardNumber { get; }
    }
}