using System;

namespace Bioware.Geometry
{
    public class GeometricException : ApplicationException
    {
        public GeometricException(string message) : base("GeometricException : " + message)
        {
        }
    }
}