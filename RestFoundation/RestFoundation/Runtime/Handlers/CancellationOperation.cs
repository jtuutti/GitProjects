// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Threading;

namespace RestFoundation.Runtime.Handlers
{
    internal sealed class CancellationOperation : IDisposable
    {
        public const int MinTimeoutInMilliseconds = 1000;

        private readonly Thread m_currentThread;
        private readonly Timer m_timer;

        private volatile bool m_isCancelled;
        private bool m_isDisposed;

        public CancellationOperation(Thread currentThread, int timeoutInMilliseconds)
        {
            if (currentThread == null)
            {
                throw new ArgumentNullException("currentThread");
            }

            if (timeoutInMilliseconds < MinTimeoutInMilliseconds)
            {
                throw new InvalidOperationException(RestResources.OutOfRangeAsyncServiceTimeout);
            }

            m_currentThread = currentThread;
            m_timer = InitializeTimer(timeoutInMilliseconds);
        }

        ~CancellationOperation()
        {
            if (!m_isDisposed)
            {
                m_timer.Dispose();
            }
        }

        public bool IsCancelled
        {
            get
            {
                return m_isCancelled;
            }
        }

        public void Dispose()
        {
            if (m_isDisposed)
            {
                return;
            }

            m_timer.Dispose();
            GC.SuppressFinalize(this);

            m_isDisposed = true;
        }

        private Timer InitializeTimer(int timeoutInMilliseconds)
        {
            return new Timer(thread =>
            {
                m_isCancelled = true;

                try
                {
                    m_currentThread.Interrupt();
                }
                catch (Exception)
                {
                }
            }, null, timeoutInMilliseconds, Timeout.Infinite);
        }
    }
}
