using System;

namespace PatientManagement.AdmissionDischargeTransfer
{
    public record PatientAdmitted(Guid PatientId, string PatientName, int AgeInYears, int WardNumber);

    public record PatientDischarged(Guid PatientId);

    public record PatientTransferred(Guid PatientId, int WardNumber);
}
