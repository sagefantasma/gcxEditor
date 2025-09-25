using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GcxEditor
{
    public class ParserException(string message) : Exception(message)
    {
    }

    public class EncoderException : Exception
    {
        public EncoderException(string message) : base(message)
        {
        }

        public EncoderException(string message,  Exception innerException) : base(message, innerException) { }
    }

    public class JsonImporterException : Exception
    {
        public JsonImporterException(string message) : base(message) { }
        public JsonImporterException(string message, Exception innerException) : base(message, innerException) { }
    }
}
