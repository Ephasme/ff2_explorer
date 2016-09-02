using System;

namespace Bioware.GFF.Exception
{
    public class FieldException : ApplicationException
    {
        public FieldException(string error) : base(error)
        {
        }
    }
}