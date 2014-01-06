using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Formats.StereoLithography
{
    /// <summary>The outer-most STL object which contains the <see cref="Facets"/> that make up the model.</summary>
    public class STLDocument : IEquatable<STLDocument>, IEnumerable<Facet>
    {
        /// <summary>Defines the buffer size to use when reading from a <see cref="StreamReader"/>.</summary>
        private const int DefaultBufferSize = 1024;

        /// <summary>The name of the solid.</summary>
        /// <remarks>This property is not used for binary STLs.</remarks>
        public string Name { get; set; }

        /// <summary>The list of <see cref="Facet"/>s within this solid.</summary>
        public IList<Facet> Facets { get; set; }

        /// <summary>Creates a new, empty <see cref="STLDocument"/>.</summary>
        public STLDocument()
        {
            this.Facets = new List<Facet>();
        }

        /// <summary>Creates a new <see cref="STLDocument"/> with the given <paramref name="name"/> and populated with the given <paramref name="facets"/>.</summary>
        /// <param name="name">
        ///     The name of the solid.
        ///     <remarks>This property is not used for binary STLs.</remarks>
        /// </param>
        /// <param name="facets">The facets with which to populate this solid.</param>
        public STLDocument(string name, IEnumerable<Facet> facets)
            : this()
        {
            this.Name = name;
            this.Facets = facets.ToList();
        }

        /// <summary>Writes the <see cref="STLDocument"/> as text to the provided <paramref name="writer"/>.</summary>
        /// <param name="writer">The writer to which the <see cref="STLDocument"/> will be written.</param>
        public void Write(StreamWriter writer)
        {
            //Write the header.
            writer.WriteLine(this.ToString());

            //Write each facet.
            this.Facets.ForEach(o => o.Write(writer));

            //Write the footer.
            writer.Write("end{0}".FormatString(this.ToString()));
        }

        /// <summary>Writes the <see cref="STLDocument"/> as binary to the provided <paramref name="writer"/>.</summary>
        /// <param name="writer">The writer to which the <see cref="STLDocument"/> will be written.</param>
        public void Write(BinaryWriter writer)
        {
            //Write the header and facet count.
            writer.Write("Binary STL created by Quantum Concepts. www.quantumconceptscorp.com");
            writer.Write(this.Facets.Count);

            //Write each facet.
            this.Facets.ForEach(o => o.Write(writer));
        }

        /// <summary>Appends the provided facets to this instance's <see cref="Facets"/>.</summary>
        /// <remarks>An entire <see cref="STLDocument"/> can be passed to this method and all of the facets which it contains will be appended to this instance.</remarks>
        /// <param name="facets">The facets to append.</param>
        public void AppendFacets(IEnumerable<Facet> facets)
        {
            foreach (Facet facet in facets)
                this.Facets.Add(facet);
        }

        /// <summary>Determines if the <see cref="STLDocument"/> contained within the <paramref name="stream"/> is text-based.</summary>
        /// <remarks>The <paramref name="stream"/> will be reset to position 0.</remarks>
        /// <param name="stream">The stream which contains the STL data.</param>
        /// <returns>True if the <see cref="STLDocument"/> is text-based, otherwise false.</returns>
        public static bool IsText(Stream stream)
        {
            const string solid = "solid";

            byte[] buffer = new byte[5];
            string header = null;

            //Reset the stream to tbe beginning and read the first few bytes, then reset the stream to the beginning again.
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(buffer, 0, buffer.Length);
            stream.Seek(0, SeekOrigin.Begin);

            //Read the header as ASCII.
            header = Encoding.ASCII.GetString(buffer);

            return solid.EqualsIgnoreCase(header);
        }

        /// <summary>Determines if the <see cref="STLDocument"/> contained within the <paramref name="stream"/> is binary-based.</summary>
        /// <remarks>The <paramref name="stream"/> will be reset to position 0.</remarks>
        /// <param name="stream">The stream which contains the STL data.</param>
        /// <returns>True if the <see cref="STLDocument"/> is binary-based, otherwise false.</returns>
        public static bool IsBinary(Stream stream)
        {
            return !IsText(stream);
        }

        /// <summary>Reads the <see cref="STLDocument"/> contained within the <paramref name="stream"/> into a new <see cref="STLDocument"/>.</summary>
        /// <remarks>This method will determine how to read the <see cref="STLDocument"/> (whether to read it as text or binary data).</remarks>
        /// <param name="stream">The stream which contains the STL data.</param>
        /// <returns>An <see cref="STLDocument"/> representing the data contained in the stream or null if the stream is empty.</returns>
        public static STLDocument Read(Stream stream)
        {
            //Determine if the stream contains a text-based or binary-based <see cref="STLDocument"/>, and then read it.
            if (IsText(stream))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.ASCII, true, DefaultBufferSize, true))
                {
                    return Read(reader);
                }
            }
            else
            {
                using (BinaryReader reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    return Read(reader);
                }
            }
        }

        /// <summary>Reads the STL document contained within the <paramref name="stream"/> into a new <see cref="STLDocument"/>.</summary>
        /// <remarks>This method expects a text-based STL document to be contained within the <paramref name="reader"/>.</remarks>
        /// <param name="reader">The reader which contains the text-based STL data.</param>
        /// <returns>An <see cref="STLDocument"/> representing the data contained in the stream or null if the stream is empty.</returns>
        public static STLDocument Read(StreamReader reader)
        {
            const string regexSolid = @"solid\s+(?<Name>[^\r\n]+)?";

            if (reader == null)
                return null;

            //Read the header.
            string header = reader.ReadLine();
            Match headerMatch = Regex.Match(header, regexSolid);
            STLDocument stl = null;
            Facet currentFacet = null;

            //Check the header.
            if (!headerMatch.Success)
                throw new FormatException("Invalid STL header, expected \"solid [name]\" but found \"{0}\".".FormatString(header));

            //Create the STL and extract the name (optional).
            stl = new STLDocument()
            {
                Name = headerMatch.Groups["Name"].Value
            };

            //Read each facet until the end of the stream.
            while ((currentFacet = Facet.Read(reader)) != null)
                stl.Facets.Add(currentFacet);

            return stl;
        }

        /// <summary>Reads the STL document contained within the <paramref name="stl"/> parameter into a new <see cref="STLDocument"/>.</summary>
        /// <param name="stl">A string which contains the STL data.</param>
        /// <returns>An <see cref="STLDocument"/> representing the data contained in the <paramref name="stl"/> parameter or null if the parameter is empty.</returns>
        public static STLDocument Read(string stl)
        {
            if (stl.IsNullOrEmpty())
                return null;

            using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(stl)))
                return Read(stream);
        }

        /// <summary>Reads the STL document located at the <paramref name="path"/> into a new <see cref="STLDocument"/>.</summary>
        /// <param name="path">A full path to a file which contains the STL data.</param>
        /// <returns>An <see cref="STLDocument"/> representing the data contained in the file located at the <paramref name="path"/> specified or null if the parameter is empty.</returns>
        public static STLDocument Open(string path)
        {
            if (path.IsNullOrEmpty())
                throw new ArgumentNullException("path");

            using (Stream stream = File.OpenRead(path))
                return Read(stream);
        }

        /// <summary>Reads the STL document contained within the <paramref name="stream"/> into a new <see cref="STLDocument"/>.</summary>
        /// <remarks>This method will expects a binary-based <see cref="STLDocument"/> to be contained within the <paramref name="reader"/>.</remarks>
        /// <param name="reader">The reader which contains the binary-based STL data.</param>
        /// <returns>An <see cref="STLDocument"/> representing the data contained in the stream or null if the stream is empty.</returns>
        public static STLDocument Read(BinaryReader reader)
        {
            if (reader == null)
                return null;

            byte[] buffer = new byte[80];
            STLDocument stl = new STLDocument();
            Facet currentFacet = null;

            //Read (and ignore) the header and number of triangles.
            buffer = reader.ReadBytes(80);
            reader.ReadBytes(4);

            //Read each facet until the end of the stream. Stop when the end of the stream is reached.
            while ((reader.BaseStream.Position != reader.BaseStream.Length) && (currentFacet = Facet.Read(reader)) != null)
                stl.Facets.Add(currentFacet);

            return stl;
        }

        /// <summary>Reads the <see cref="STLDocument"/> within the <paramref name="inStream"/> as text into the <paramref name="outStream"/>.</summary>
        /// <param name="inStream">The stream to read from.</param>
        /// <param name="outStream">The stream to read into.</param>
        /// <returns>The <see cref="STLDocument"/> that was copied.</returns>
        public static STLDocument CopyAsText(Stream inStream, Stream outStream)
        {
            STLDocument stl = Read(inStream);

            using (StreamWriter writer = new StreamWriter(outStream, Encoding.ASCII, DefaultBufferSize, true))
                stl.Write(writer);

            return stl;
        }

        /// <summary>Reads the <see cref="STLDocument"/> within the <paramref name="inStream"/> as binary into the <paramref name="outStream"/>.</summary>
        /// <param name="inStream">The stream to read from.</param>
        /// <param name="outStream">The stream to read into.</param>
        /// <returns>The <see cref="STLDocument"/> that was copied.</returns>
        public static STLDocument CopyAsBinary(Stream inStream, Stream outStream)
        {
            STLDocument stl = Read(inStream);

            using (BinaryWriter writer = new BinaryWriter(outStream, Encoding.ASCII, true))
                stl.Write(writer);

            return stl;
        }

        /// <summary>Returns the header representation of this <see cref="STLDocument"/>.</summary>
        public override string ToString()
        {
            return "solid {0}".FormatString(this.Name);
        }

        /// <summary>Determines whether or not this instance is the same as the <paramref name="other"/> instance.</summary>
        /// <param name="other">The <see cref="STLDocument"/> to which to compare.</param>
        /// <returns>True if this instance is equal to the <paramref name="other"/> instance.</returns>
        public bool Equals(STLDocument other)
        {
            return (string.Equals(this.Name, other.Name)
                    && this.Facets.Count == other.Facets.Count
                    && this.Facets.All((i, o) => o.Equals(other.Facets[i])));
        }

        /// <summary>Iterates through the <see cref="Facets"/> collection.</summary>
        public IEnumerator<Facet> GetEnumerator()
        {
            return this.Facets.GetEnumerator();
        }

        /// <summary>Iterates through the <see cref="Facets"/> collection.</summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
