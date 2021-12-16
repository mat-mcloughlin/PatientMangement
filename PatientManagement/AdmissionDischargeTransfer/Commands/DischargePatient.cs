using System;

namespace PatientManagement.AdmissionDischargeTransfer.Commands;

public record DischargePatient(
    Guid PatientId
);