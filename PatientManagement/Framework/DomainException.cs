using System;

namespace PatientManagement.Framework;

public class DomainException : Exception
{
    public DomainException(string message)
        :base(message)
    {
    }
}