using System;

namespace RestFoundation
{
    public interface IObjectActivator
    {
        object Create(Type objectType);
    }
}
