using System;

namespace PatientManagement.AdmissionDischargeTransfer.Commands
{
    public record AdmitPatient(Guid PatientId, string PatientName, int AgeInYears, DateTime TimeOfAdmission,
        int WardNumber);

    public record DischargePatient(Guid PatientId);

    public record TransferPatient (Guid PatientId, int WardNumber);
}
