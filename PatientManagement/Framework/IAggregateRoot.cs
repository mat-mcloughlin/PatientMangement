using System;
using System.Collections.Generic;

namespace PatientManagement.Framework
{
    public interface IAggregateRoot
    {
        List<object> GetEvents();

        void ClearEvents();

        void Apply(object e);

        String Id { get; }

        int Version { get; }
    }
}