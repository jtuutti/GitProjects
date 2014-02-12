// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestFoundation.Runtime
{
    internal sealed class CompositeValidationResult : ValidationResult
    {
        private readonly List<ValidationResult> m_results = new List<ValidationResult>();

        public CompositeValidationResult(string errorMessage)
            : base(errorMessage)
        {
        }

        public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames)
            : base(errorMessage, memberNames)
        {
        }

        public IEnumerable<ValidationResult> Results
        {
            get
            {
                foreach (ValidationResult result in m_results)
                {
                    yield return result;
                }
            }
        }

        public void AddResult(ValidationResult validationResult)
        {
            if (validationResult == null)
            {
                throw new ArgumentNullException("validationResult");
            }

            m_results.Add(validationResult);
        }
    }
}
