using System;

namespace MessageBus.Msmq
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class QueueNameAttribute : Attribute
    {
        private readonly string name;

        public QueueNameAttribute(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            this.name = name;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }
    }
}
