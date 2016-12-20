using System;
using System.Collections.Generic;

namespace PatientManagement.Framework
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        readonly Dictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

        readonly List<object> _events = new List<object>();

        public Guid Id { get; protected set; }

        public int Version { get; protected set; } = -1;

        List<object> IAggregateRoot.GetEvents()
        {
            return _events;
        }

        void IAggregateRoot.ClearEvents()
        {
            _events.Clear();
        }

        protected void Register<T>(Action<T> when)
        {
            _handlers.Add(typeof(T), e => when((T)e));
        }

        void IAggregateRoot.Apply(object e)
        {
            Raise(e);
            Version++;
        }

        protected void Raise(object e)
        {
            _handlers[e.GetType()](e);
            _events.Add(e);
        }
    }
}