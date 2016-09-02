using System;

namespace Bioware.GFF.Exception
{
    public class CompositeException : ApplicationException
    {
        public CompositeException(string error) : base(error)
        {
        }
    }
}