﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QuantumConcepts.Formats.StereoLithography
{
    /// <summary>A simple XYZ representation of a vertex.</summary>
    public class Vertex : IEquatable<Vertex>
    {
        /// <summary>The X coordinate of this <see cref="Vertex"/>.</summary>
        public float X { get; set; }

        /// <summary>The Y coordinate of this <see cref="Vertex"/>.</summary>
        public float Y { get; set; }

        /// <summary>The Z coordinate of this <see cref="Vertex"/>.</summary>
        public float Z { get; set; }

        /// <summary>Creates a new, empty <see cref="Vertex"/>.</summary>
        public Vertex() { }

        /// <summary>Creates a new <see cref="Vertex"/> using the provided coordinates.</summary>
        /// <param name="x"><see cref="X"/></param>
        /// <param name="y"><see cref="Y"/></param>
        /// <param name="z"><see cref="Z"/></param>
        public Vertex(float x, float y, float z)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>Shifts the <paramref name="Vertex"/> by the X, Y and Z values in the <paramref name="shift"/> parameter.</summary>
        /// <param name="shift">The amount to shift the vertex.</param>
        public void Shift(Vertex shift)
        {
            this.X += shift.X;
            this.Y += shift.Y;
            this.Z += shift.Z;
        }

        /// <summary>Writes the <see cref="Vertex"/> as text to the <paramref name="writer"/>.</summary>
        /// <param name="writer">The writer to which the <see cref="Vertex"/> will be written at the current position.</param>
        public void Write(StreamWriter writer)
        {
            writer.WriteLine("\t\t\t{0}", this.ToString());
        }

        /// <summary>Writes the <see cref="Vertex"/> as binary to the <paramref name="writer"/>.</summary>
        /// <param name="writer">The writer to which the <see cref="Vertex"/> will be written at the current position.</param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(this.X);
            writer.Write(this.Y);
            writer.Write(this.Z);
        }

        /// <summary>Returns the string representation of this <see cref="Vertex"/>.</summary>
        public override string ToString()
        {
            return string.Format("vertex {0} {1} {2}", this.X, this.Y, this.Z);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        /// <summary>Determines whether or not this instance is the same as the <paramref name="other"/> instance.</summary>
        /// <param name="other">The <see cref="Vertex"/> to which to compare.</param>
        public bool Equals(Vertex other)
        {
            return (this.X.Equals(other.X)
                    && this.Y.Equals(other.Y)
                    && this.Z.Equals(other.Z));
        }

        /// <summary>Reads a single <see cref="Vertex"/> from the <paramref name="reader"/>.</summary>
        /// <param name="reader">The reader which contains a <see cref="Vertex"/> to be read at the current position</param>
        public static Vertex Read(StreamReader reader)
        {
            const string regex = @"\s*(facet normal|vertex)\s+(?<X>[^\s]+)\s+(?<Y>[^\s]+)\s+(?<Z>[^\s]+)";
            const NumberStyles numberStyle = (NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);

            string data = null;
            float x, y, z;
            Match match = null;

            if (reader == null)
                return null;

            //Read the next line of data.
            data = reader.ReadLine();

            if (data == null)
                return null;

            //Ensure that the data is formatted correctly.
            match = Regex.Match(data, regex, RegexOptions.IgnoreCase);

            if (!match.Success)
                return null;

            //Parse the three coordinates.
            if (!float.TryParse(match.Groups["X"].Value, numberStyle, CultureInfo.CurrentCulture, out x))
                throw new FormatException(string.Format("Could not parse X coordinate \"{0}\" as a decimal.", match.Groups["X"]));

            if (!float.TryParse(match.Groups["Y"].Value, numberStyle, CultureInfo.CurrentCulture, out y))
                throw new FormatException(string.Format("Could not parse Y coordinate \"{0}\" as a decimal.", match.Groups["Y"]));

            if (!float.TryParse(match.Groups["Z"].Value, numberStyle, CultureInfo.CurrentCulture, out z))
                throw new FormatException(string.Format("Could not parse Z coordinate \"{0}\" as a decimal.", match.Groups["Z"]));

            return new Vertex()
            {
                X = x,
                Y = y,
                Z = z
            };
        }

        /// <summary>Reads a single <see cref="Vertex"/> from the <paramref name="reader"/>.</summary>
        /// <param name="reader">The reader which contains a <see cref="Vertex"/> to be read at the current position</param>
        public static Vertex Read(BinaryReader reader)
        {
            const int floatSize = sizeof(float);
            const int vertexSize = (floatSize * 3);

            if (reader == null)
                return null;

            //Read 3 floats.
            byte[] data = new byte[vertexSize];
            int bytesRead = reader.Read(data, 0, data.Length);

            //If no bytes are read then we're at the end of the stream.
            if (bytesRead == 0)
                return null;
            else if (bytesRead != data.Length)
                throw new FormatException(string.Format("Could not convert the binary data to a vertex. Expected {0} bytes but found {1}.", vertexSize, bytesRead));

            //Convert the read bytes to their numeric representation.
            return new Vertex()
            {
                X = BitConverter.ToSingle(data, 0),
                Y = BitConverter.ToSingle(data, floatSize),
                Z = BitConverter.ToSingle(data, (floatSize * 2))
            };
        }
    }
}
