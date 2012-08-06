using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Formats.StereoLithography
{
    public class Facet : IEquatable<Facet>
    {
        public Normal Normal { get; set; }
        public List<Vertex> Vertices { get; set; }
        public int AttributeByteCount { get; set; }

        public Facet()
        {
            this.Vertices = new List<Vertex>();
        }

        public Facet(Normal normal, IEnumerable<Vertex> vertices, int attributeByteCount)
            : this()
        {
            this.Normal = normal;
            this.Vertices = vertices.ToList();
            this.AttributeByteCount = attributeByteCount;
        }

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
            facet.AttributeByteCount = reader.ReadInt16();

            return facet;
        }

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
        
        public void Write(BinaryWriter writer)
        {
            //Write the normal.
            this.Normal.Write(writer);

            //Write each vertex.
            this.Vertices.ForEach(o => o.Write(writer));
        }

        public override string ToString()
        {
            return "facet {0}".FormatString(this.Normal);
        }

        public bool Equals(Facet other)
        {
            return (this.Normal.Equals(other.Normal)
                    && this.Vertices.Count == other.Vertices.Count
                    && this.Vertices.All((i, o) => o.Equals(other.Vertices[i])));
        }
    }
}
