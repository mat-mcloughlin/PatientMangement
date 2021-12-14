using System;

namespace PatientManagement.AdmissionDischargeTransfer;

public record PatientAdmitted(
    Guid PatientId, 
    string PatientName, 
    int AgeInYears, 
    int WardNumber
);