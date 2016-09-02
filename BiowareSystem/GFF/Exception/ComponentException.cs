using System;

namespace Bioware.GFF.Exception
{
    public class ComponentException : ApplicationException
    {
        public ComponentException(string error) : base(error)
        {
        }
    }
}