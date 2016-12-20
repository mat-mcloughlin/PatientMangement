using System;
using PatientManagement.Framework;

namespace PatientManagement.AdmissionDischargeTransfer
{
    public class Encounter : AggregateRoot
    {
        string _patientName;

        int _ageInYears;

        int _ward;

        bool _currentlyAdmitted;

        public Encounter(Guid patientId, string patientName, int ageInYears, int wardNumber)
            : this()
        {
            Raise(new PatientAdmitted(patientId, patientName, ageInYears, wardNumber));
        }

        private Encounter()
        {
            Register<PatientAdmitted>(When);
            Register<PatientDischarged>(When);
            Register<PatientTransfered>(When);
        }

        public void DischargePatient()
        {
            CheckPatientIsAdmitted();

            Raise(new PatientDischarged(Id));
        }

        public void When(PatientAdmitted e)
        {
            _currentlyAdmitted = true;
            Id = e.PatientId;
            _patientName = e.PatientName;
            _ageInYears = e.AgeInYears;
            _ward = e.WardNumber;
        }

        private void When(PatientDischarged e)
        {
            _currentlyAdmitted = false;
        }

        public void CheckPatientIsAdmitted()
        {
            if (!_currentlyAdmitted)
            {
                throw new DomainException("Patient needs to be admitted first.");
            }
        }

        public void Transfer(int wardNumber)
        {
            CheckPatientIsAdmitted();

            Raise(new PatientTransfered(Id, wardNumber));
        }

        private void When(PatientTransfered e)
        {
            _ward = e.WardNumber;
        }
    }
}
