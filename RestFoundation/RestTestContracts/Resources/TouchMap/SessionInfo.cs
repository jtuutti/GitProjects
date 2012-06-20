using System;

namespace RestTestContracts.Resources
{
    public struct SessionInfo
    {
        private readonly string m_applicationId;
        private readonly string m_customerId;
        private readonly Guid m_sessionId;
        private readonly string m_culture;
        private readonly string m_environment;

        public SessionInfo(string applicationId, string customerId, string sessionId, string culture, string environment)
        {
            m_applicationId = applicationId;
            m_customerId = customerId;
            m_culture = !String.IsNullOrEmpty(culture) ? culture : "en-US";
            m_environment = environment;

            if (!Guid.TryParse(sessionId, out m_sessionId))
            {
                m_sessionId = Guid.Empty;
            }
        }

        public string ApplicationId
        {
            get
            {
                return m_applicationId;
            }
        }

        public string CustomerId
        {
            get
            {
                return m_customerId;
            }
        }

        public Guid SessionId
        {
            get
            {
                return m_sessionId;
            }
        }

        public string Culture
        {
            get
            {
                return m_culture;
            }
        }

        public string Environment
        {
            get
            {
                return m_environment;
            }
        }
    }
}
