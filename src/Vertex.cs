using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using QuantumConcepts.Formats.StereoLithography;

namespace QuantumConcepts.Formats.StereoLithography
{
    /// <summary>A simple XYZ representation of a vertex.</summary>
    public class Vertex : IEquatable<Vertex>
    {
        /// <summary>The X coordinate of this <see cref="Vertex"/>.</summary>
        public float X { get; set; } = 0;

        /// <summary>The Y coordinate of this <see cref="Vertex"/>.</summary>
        public float Y { get; set; } = 0;

        /// <summary>The Z coordinate of this <see cref="Vertex"/>.</summary>
        public float Z { get; set; } = 0;

        /// <summary>Creates a new, empty <see cref="Vertex"/>.</summary>
        public Vertex() { }

        /// <summary>Creates a new <see cref="Vertex"/> using the provided coordinates.</summary>
        /// <param name="x"><see cref="X"/></param>
        /// <param name="y"><see cref="Y"/></param>
        /// <param name="z"><see cref="Z"/></param>
        public Vertex(float x, float y, float z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>Shifts the <paramref name="shift"/> by the X, Y and Z values in the <paramref name="shift"/> parameter.</summary>
        /// <param name="shift">The amount to shift the vertex.</param>
        public void Shift(Vertex shift)
        {
            X += shift.X;
            Y += shift.Y;
            Z += shift.Z;
        }

        /// <summary>Writes the <see cref="Vertex"/> as text to the <paramref name="writer"/>.</summary>
        /// <param name="writer">The writer to which the <see cref="Vertex"/> will be written at the current position.</param>
        public void Write(StreamWriter writer)
        {
            writer.WriteLine($"\t\t\t{this}");
        }

        /// <summary>Writes the <see cref="Vertex"/> as binary to the <paramref name="writer"/>.</summary>
        /// <param name="writer">The writer to which the <see cref="Vertex"/> will be written at the current position.</param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }

        /// <summary>Reads a single <see cref="Vertex"/> from the <paramref name="reader"/>.</summary>
        /// <param name="reader">The reader which contains a <see cref="Vertex"/> to be read at the current position</param>
        public static Vertex Read(StreamReader reader)
        {
            const string regex = @"\s*(facet normal|vertex)\s+(?<X>[^\s]+)\s+(?<Y>[^\s]+)\s+(?<Z>[^\s]+)";
            string? data;
            Match match;

            if (reader == null) throw new ArgumentNullException(nameof(reader));

            // Read the next line of data.
            data = reader.ReadLine();

            if (data == null) throw new InvalidOperationException("No data could be read from reader.");

            // Ensure that the data is formatted correctly.
            match = Regex.Match(data, regex, RegexOptions.IgnoreCase);

            if (!match.Success) throw new InvalidOperationException($"Vertex is not formatted correctly: {data}");

            // Parse the three coordinates.
            return new Vertex()
            {
                X = ParseCoordinate("X", match.Groups["X"].Value),
                Y = ParseCoordinate("Y", match.Groups["Y"].Value),
                Z = ParseCoordinate("Z", match.Groups["Z"].Value),
            };
        }

        private static float ParseCoordinate(string which, string value)
        {
            const NumberStyles numberStyle = (NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);
            float parsed;

            if (!float.TryParse(value, numberStyle, CultureInfo.InvariantCulture, out parsed))
            {
                throw new FormatException($"Could not parse {which} coordinate as a decimal from value: {value}");
            }

            return parsed;
        }

        /// <summary>Reads a single <see cref="Vertex"/> from the <paramref name="reader"/>.</summary>
        /// <param name="reader">The reader which contains a <see cref="Vertex"/> to be read at the current position</param>
        public static Vertex Read(BinaryReader reader)
        {
            const int floatSize = sizeof(float);
            const int vertexSize = (floatSize * 3);

            if (reader == null) throw new ArgumentNullException(nameof(reader));

            // Read 3 floats.
            byte[] data = new byte[vertexSize];
            int bytesRead = reader.Read(data, 0, data.Length);

            if (bytesRead == 0) throw new InvalidOperationException("No data could be read from reader.");
            if (bytesRead != data.Length) throw new FormatException($"Could not convert the binary data to a vertex. Expected {vertexSize} bytes but found {bytesRead}.");

            // Convert the read bytes to their numeric representation.
            return new Vertex()
            {
                X = BitConverter.ToSingle(data, 0),
                Y = BitConverter.ToSingle(data, floatSize),
                Z = BitConverter.ToSingle(data, (floatSize * 2))
            };
        }

        /// <summary>Returns the string representation of this <see cref="Vertex"/>.</summary>
        public override string ToString()
        {
            return $"vertex {X} {Y} {Z}";
        }

        /// <see cref="Object.GetHashCode"/>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <see cref="Equals(Vertex)"/>
        public override bool Equals(object? other)
        {
            return Equals(other as Vertex);
        }

        /// <summary>Determines whether or not this instance is the same as the <paramref name="other"/> instance.</summary>
        /// <param name="other">The <see cref="Vertex"/> to which to compare.</param>
        public bool Equals(Vertex? other)
        {
            return
                other != null &&
                X == other.X &&
                Y == other.Y &&
                Z == other.Z;
        }
    }
}
