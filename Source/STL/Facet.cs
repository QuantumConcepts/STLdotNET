using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using QuantumConcepts.Formats.StereoLithography;

namespace QuantumConcepts.Formats.StereoLithography
{
    /// <summary>A representation of a facet which is defined by its location (<see cref="Vertices"/>) and directionality (<see cref="Normal"/>).</summary>
    public class Facet : IEquatable<Facet>, IEnumerable<Vertex>
    {
        /// <summary>Indicates the directionality of the <see cref="Facet"/>.</summary>
        public Normal Normal { get; set; }

        /// <summary>Indicates the location of the <see cref="Facet"/>.</summary>
        public IList<Vertex> Vertices { get; set; }

        /// <summary>Additional data attached to the facet.</summary>
        /// <remarks>Depending on the source of the STL, this could be used to indicate such things as the color of the <see cref="Facet"/>. This functionality only exists in binary STLs.</remarks>
        public UInt16 AttributeByteCount { get; set; }

        /// <summary>Creates a new, empty <see cref="Facet"/>.</summary>
        public Facet()
        {
            this.Vertices = new List<Vertex>();
        }

        /// <summary>Creates a new <see cref="Facet"/> using the provided parameters.</summary>
        /// <param name="normal">The directionality of the <see cref="Facet"/>.</param>
        /// <param name="vertices">The location of the <see cref="Facet"/>.</param>
        /// <param name="attributeByteCount">Additional data to attach to the <see cref="Facet"/>.</param>
        public Facet(Normal normal, IEnumerable<Vertex> vertices, UInt16 attributeByteCount)
            : this()
        {
            this.Normal = normal;
            this.Vertices = vertices.ToList();
            this.AttributeByteCount = attributeByteCount;
        }

        /// <summary>Writes the <see cref="Facet"/> as text to the <paramref name="writer"/>.</summary>
        /// <param name="writer">The writer to which the <see cref="Facet"/> will be written at the current position.</param>
        public void Write(StreamWriter writer)
        {
            writer.Write("\t");
            writer.WriteLine(this.ToString());
            writer.WriteLine("\t\touter loop");

            //Write each vertex.
            this.Vertices.ForEach(o => o.Write(writer));

            writer.WriteLine("\t\tendloop");
            writer.WriteLine("\tendfacet");
        }

        /// <summary>Writes the <see cref="Facet"/> as binary to the <paramref name="writer"/>.</summary>
        /// <param name="writer">The writer to which the <see cref="Facet"/> will be written at the current position.</param>
        public void Write(BinaryWriter writer)
        {
            //Write the normal.
            this.Normal.Write(writer);

            //Write each vertex.
            this.Vertices.ForEach(o => o.Write(writer));

            //Write the attribute byte count.
            writer.Write(this.AttributeByteCount);
        }

        /// <summary>Returns the string representation of this <see cref="Facet"/>.</summary>
        public override string ToString()
        {
            return "facet {0}".Interpolate(this.Normal);
        }

        /// <summary>Determines whether or not this instance is the same as the <paramref name="other"/> instance.</summary>
        /// <param name="other">The <see cref="Facet"/> to which to compare.</param>
        public bool Equals(Facet other)
        {
            return (this.Normal.Equals(other.Normal)
                    && this.Vertices.Count == other.Vertices.Count
                    && this.Vertices.All((i, o) => o.Equals(other.Vertices[i])));
        }

        /// <summary>Iterates through the <see cref="Vertices"/> collection.</summary>
        public IEnumerator<Vertex> GetEnumerator()
        {
            return this.Vertices.GetEnumerator();
        }

        /// <summary>Iterates through the <see cref="Vertices"/> collection.</summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Reads a single <see cref="Facet"/> from the <paramref name="reader"/>.</summary>
        /// <param name="reader">The reader which contains a <see cref="Facet"/> to be read at the current position</param>
        public static Facet Read(StreamReader reader)
        {
            if (reader == null)
                return null;

            //Create the facet.
            Facet facet = new Facet();

            //Read the normal.
            if ((facet.Normal = Normal.Read(reader)) == null)
                return null;

            //Skip the "outer loop".
            reader.ReadLine();

            //Read 3 vertices.
            facet.Vertices = Enumerable.Range(0, 3).Select(o => Vertex.Read(reader)).ToList();

            //Read the "endloop" and "endfacet".
            reader.ReadLine();
            reader.ReadLine();

            return facet;
        }

        /// <summary>Reads a single <see cref="Facet"/> from the <paramref name="reader"/>.</summary>
        /// <param name="reader">The reader which contains a <see cref="Facet"/> to be read at the current position</param>
        public static Facet Read(BinaryReader reader)
        {
            if (reader == null)
                return null;

            //Create the facet.
            Facet facet = new Facet();

            //Read the normal.
            facet.Normal = Normal.Read(reader);

            //Read 3 vertices.
            facet.Vertices = Enumerable.Range(0, 3).Select(o => Vertex.Read(reader)).ToList();

            //Read the attribute byte count.
            facet.AttributeByteCount = reader.ReadUInt16();

            return facet;
        }
    }
}
