using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Formats.StereoLithography
{
    public class Normal : Vertex
    {
        public Normal() { }

        public Normal(decimal x, decimal y, decimal z)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static Normal Read(StreamReader reader)
        {
            return Normal.FromVertex(Vertex.Read(reader));
        }

        public static Normal Read(BinaryReader reader)
        {
            return Normal.FromVertex(Vertex.Read(reader));
        }

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

        public override string ToString()
        {
            return "normal {0} {1} {2}".FormatString(this.X, this.Y, this.Z);
        }
    }
}
