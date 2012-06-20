using System;
using System.IO;
using System.Text;

namespace RestFoundation
{
    public interface IDataFormatter
    {
        object Format(Stream body, Encoding encoding, Type objectType);
    }
}
