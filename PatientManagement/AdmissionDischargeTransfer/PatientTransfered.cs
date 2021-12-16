using System;

namespace PatientManagement.AdmissionDischargeTransfer;

public record PatientTransfered(
    Guid PatientId, 
    int WardNumber
);