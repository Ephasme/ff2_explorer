﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bioware.Geometry {
    public class GeometricException : ApplicationException {
        public GeometricException(string message) : base("GeometricException : " + message) {}
    }
    public class Vector {
        private readonly Vertex _dest;
        private readonly Vertex _or;
        public Vector(Vertex or, Vertex dest) {
            _or = or;
            _dest = dest;
        }
        public double this[int index] {
            get { return _dest[index] - _or[index]; }
        }
    }
    public class Vertex : IEnumerable<double>, IEquatable<Vertex> {
        private readonly double[] _c;
        public Vertex(params double[] c) {
            _c = c;
        }
        public int Dimension {
            get { return _c.Length; }
        }

        public double this[int ind] {
            get { return _c[ind]; }
            set { _c[ind] = value; }
        }
        public override string ToString() {
            string res = _c.Aggregate("(", (current, d) => current + (d.ToString() + ","));
            res = res.TrimEnd(',') + ")";
            return res;
        }

        private static Vertex DoOpe(Vertex vA, Vertex vB, CoordOpe ope) {
            if (vA.Dimension == vB.Dimension) {
                var lD = new double[vA.Dimension];
                for (int i = 0; i < vA.Dimension; i++) {
                    lD[i] = ope(vA._c[i], vB._c[i]);
                }
                return new Vertex(lD);
            }
            throw new GeometricException("Impossible d'ajouter des points de dimensions différentes.");
        }

        public static Vertex operator +(Vertex vA, Vertex vB) {
            return DoOpe(vA, vB, (dA, dB) => dA + dB);
        }
        public static Vertex operator -(Vertex vA, Vertex vB) {
            return DoOpe(vA, vB, (dA, dB) => dA - dB);
        }

        #region IEnumerable<double> Membres
        public IEnumerator<double> GetEnumerator() {
            return (IEnumerator<double>) _c.GetEnumerator();
        }
        #endregion

        #region IEnumerable Membres
        IEnumerator IEnumerable.GetEnumerator() {
            return _c.GetEnumerator();
        }
        #endregion

        /// <summary>
        /// Indique si l'objet actuel est égal à un autre objet du même type.
        /// </summary>
        /// <returns>
        /// true si l'objet en cours est égal au paramètre <paramref name="v"/> ; sinon, false.
        /// </returns>
        /// <param name="v">Objet à comparer avec cet objet.
        ///             </param>
        public bool Equals(Vertex v) {
            if (v == null || v.Dimension != Dimension) {
                return false;
            }
            if (ReferenceEquals(this, v)) {
                return true;
            }
            var i = 0;
            while (i<Dimension && v._c[i] - double.Epsilon <= _c[i] && _c[i] <= v._c[i]+double.Epsilon) {
            i++;
            }
            return i == Dimension ;
        }
        /// <summary>
        /// Détermine si l'objet <see cref="T:System.Object"/> spécifié est égal à l'objet <see cref="T:System.Object"/> en cours.
        /// </summary>
        /// <returns>
        /// true si l'objet <see cref="T:System.Object"/> spécifié est égal à l'objet <see cref="T:System.Object"/> en cours ; sinon, false.
        /// </returns>
        /// <param name="obj"><see cref="T:System.Object"/> à comparer avec le <see cref="T:System.Object"/> en cours. 
        ///             </param><exception cref="T:System.NullReferenceException">Le paramètre <paramref name="obj"/> est null.
        ///             </exception><filterpriority>2</filterpriority>
        public override bool Equals(object obj) {
            return Equals(obj as Vertex);
        }
        /// <summary>
        /// Sert de fonction de hachage pour un type particulier. 
        /// </summary>
        /// <returns>
        /// Code de hachage du <see cref="T:System.Object"/> actuel.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode() {
            return (_c != null ? _c.GetHashCode() : 0);
        }

        public static bool operator ==(Vertex left, Vertex right) {
            return Equals(left, right);
        }
        public static bool operator !=(Vertex left, Vertex right) {
            return !Equals(left, right);
        }

        #region Nested type: CoordOpe
        private delegate double CoordOpe(double dA, double dB);
        #endregion
    }
}