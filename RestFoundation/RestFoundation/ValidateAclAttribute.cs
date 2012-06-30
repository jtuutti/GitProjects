using System;

namespace RestFoundation
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateAclAttribute : Attribute
    {
        public ValidateAclAttribute(string nameValueSectionName)
        {
            if (String.IsNullOrWhiteSpace(nameValueSectionName))
            {
                throw new ArgumentNullException("nameValueSectionName");
            }

            SectionName = nameValueSectionName;
        }

        public string SectionName { get; private set; }
    }
}
