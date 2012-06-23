using System;

namespace RestFoundation
{
    [Flags]
    public enum DataFormatterType
    {
        None = 0x0,
        ContentType = 0x1,
        ResourceType = 0x2
    }
}
