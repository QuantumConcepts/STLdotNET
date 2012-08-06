using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Formats.StereoLithography
{
    public class STL : IEquatable<STL>
    {
        public string Name { get; set; }
        public List<Facet> Facets { get; set; }

        public STL()
        {
            this.Facets = new List<Facet>();
        }

        public STL(string name, IEnumerable<Facet> facets)
            : this()
        {
            this.Name = name;
            this.Facets = facets.ToList();
        }

        public static STL Read(StreamReader reader)
        {
            const string regexSolid = @"solid\s+(?<Name>[^\r\n]+)?";

            if (reader == null)
                return null;

            //Read the header.
            string header = reader.ReadLine();
            Match headerMatch = Regex.Match(header, regexSolid);
            STL stl = null;
            Facet currentFacet = null;

            //Check the header.
            if (!headerMatch.Success)
                throw new FormatException("Invalid STL header, expected \"solid [name]\" but found \"{0}\".".FormatString(header));

            //Create the STL and extract the name (optional).
            stl = new STL()
            {
                Name = headerMatch.Groups["Name"].Value
            };

            //Read each facet until the end of the stream.
            while ((currentFacet = Facet.Read(reader)) != null)
                stl.Facets.Add(currentFacet);

            return stl;
        }

        public static STL Read(BinaryReader reader)
        {
            if (reader == null)
                return null;

            byte[] buffer = new byte[80];
            string header = null;
            STL stl = null;
            Facet currentFacet = null;

            //Read the header.
            buffer = reader.ReadBytes(80);
            header = Encoding.ASCII.GetString(buffer);

            //Create the STL.
            stl = new STL();

            //Read (ignore) the number of triangles.
            reader.ReadBytes(4);

            //Read each facet until the end of the stream.
            while ((currentFacet = Facet.Read(reader)) != null)
            {
                stl.Facets.Add(currentFacet);

                if (reader.BaseStream.Position == reader.BaseStream.Length)
                    break;
            }

            return stl;
        }

        public void Write(StreamWriter writer)
        {
            //Write the header.
            writer.WriteLine(this.ToString());

            //Write each facet.
            this.Facets.ForEach(o => o.Write(writer));

            //Write the footer.
            writer.Write("end{0}".FormatString(this.ToString()));
        }

        public void Write(BinaryWriter writer)
        {
            //Write the header and facet count.
            writer.Write("Binary STL created by Quantum Concepts. www.quantumconceptscorp.com");
            writer.Write(this.Facets.Count);

            //Write each facet.
            this.Facets.ForEach(o => o.Write(writer));
        }

        public override string ToString()
        {
            return "solid {0}".FormatString(this.Name);
        }

        public bool Equals(STL other)
        {
            return (string.Equals(this.Name, other.Name)
                    && this.Facets.Count == other.Facets.Count
                    && this.Facets.All((i, o) => o.Equals(other.Facets[i])));
        }
    }
}
