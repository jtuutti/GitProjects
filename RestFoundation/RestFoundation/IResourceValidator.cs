using System.Collections.Generic;

namespace RestFoundation
{
    public interface IResourceValidator
    {
        bool IsValid(object resource, out ICollection<ValidationError> errors);
    }
}
