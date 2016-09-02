using System;

namespace Bioware.GFF.Exception
{
    public class FileException : ApplicationException
    {
        public FileException(string error) : base(error)
        {
        }
    }
}