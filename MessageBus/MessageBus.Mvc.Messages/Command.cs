using System;
using System.Collections.Generic;

namespace MessageBus.Mvc.Messages
{
    [Serializable]
    public class Command : IMessage
    {
        #region Inherit from the MessageBus.MessageBase instead of implementing the MessageBus.IMessage interface to avoid implementing this section

        private readonly IDictionary<string, string> headers = new Dictionary<string, string>();

        public IDictionary<string, string> Headers
        {
            get
            {
                return headers;
            }
        }

        #endregion

        public int Id { get; set; }
    }
}
