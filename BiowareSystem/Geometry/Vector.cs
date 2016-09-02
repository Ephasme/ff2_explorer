namespace Bioware.Geometry
{
    public class Vector
    {
        private readonly Vertex _dest;

        private readonly Vertex _or;

        public Vector(Vertex or, Vertex dest)
        {
            _or = or;
            _dest = dest;
        }

        public double this[int index] => _dest[index] - _or[index];
    }
}