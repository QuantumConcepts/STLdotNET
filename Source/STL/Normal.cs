using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QuantumConcepts.Formats.StereoLithography;
using System.Globalization;

namespace QuantumConcepts.Formats.StereoLithography
{
    /// <summary>A simple XYZ representation of a normal (<see cref="Vertex"/>).</summary>
    public class Normal : Vertex
    {
        /// <summary>Creates a new, empty <see cref="Normal"/>.</summary>
        public Normal() : base() { }

        /// <summary>Creates a new <see cref="Normal"/> using the provided coordinates.</summary>
        public Normal(float x, float y, float z) : base(x, y, z) { }

        /// <summary>Flips the normal so it faces the opposite direction.</summary>
        public void Invert()
        {
            this.X *= -1;
            this.Y *= -1;
            this.Z *= -1;
        }

        /// <summary>Returns the string representation of this <see cref="Normal"/>.</summary>
        public override string ToString()
        {
            //return "normal {0} {1} {2}".FormatString(this.X, this.Y, this.Z);
            return String.Format(CultureInfo.InvariantCulture, "normal {0} {1} {2}", this.X, this.Y, this.Z);

        }

        /// <summary>Reads a single <see cref="Normal"/> from the <paramref name="reader"/>.</summary>
        /// <param name="reader">The reader which contains a <see cref="Normal"/> to be read at the current position</param>
        public static Normal Read(StreamReader reader)
        {
            return Normal.FromVertex(Vertex.Read(reader));
        }

        /// <summary>Reads a single <see cref="Normal"/> from the <paramref name="reader"/>.</summary>
        /// <param name="reader">The reader which contains a <see cref="Normal"/> to be read at the current position</param>
        public static Normal Read(BinaryReader reader)
        {
            return Normal.FromVertex(Vertex.Read(reader));
        }

        /// <summary>Converts the <paramref name="vertex"/> to a normal.</summary>
        /// <remarks>This does nothing more than copy the X, Y and Z coordinates of the <paramref name="vertex"/> into a new <see cref="Normal"/> instance.</remarks>
        /// <param name="vertex">The <see cref="Vertex"/> to be converted into a <see cref="Normal"/>.</param>
        /// <returns>A <see cref="Normal"/> or null if the <paramref name="vertex"/> is null.</returns>
        public static Normal FromVertex(Vertex vertex)
        {
            if (vertex == null)
                return null;

            return new Normal()
            {
                X = vertex.X,
                Y = vertex.Y,
                Z = vertex.Z
            };
        }
    }
}
