using System;

namespace PatientManagement.AdmissionDischargeTransfer.Commands;

public record TransferPatient(
    Guid PatientId,
    int WardNumber
);