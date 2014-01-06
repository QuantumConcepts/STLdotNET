using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantumConcepts.Formats.StereoLithography;
using QuantumConcepts.Common.Extensions;
using System.Linq;
using QuantumConcepts.Common.IO;
using System.Diagnostics;

namespace QuantumConcepts.Formats.StereoLithography.Test
{
    [TestClass]
    public class STLTests
    {
        private const string NetfabbPath = @"%AppData%\Roaming\netfabb\netfabb.exe";

        [TestMethod]
        [Description("Ensures that both string and binary STL files can be read by the STLDocument.Read method.")]
        public void FromStringAndBinary()
        {
            STLDocument stlString = null;
            STLDocument stlBinary = null;

            using (Stream stream = GetData("ASCII.stl"))
            {
                stlString = STLDocument.Read(stream);
            }

            ValidateSTL(stlString);

            using (Stream stream = GetData("Binary.stl"))
            {
                stlBinary = STLDocument.Read(stream);
            }

            ValidateSTL(stlBinary);
        }

        [TestMethod]
        [Description("Ensures that reading text-based STLs works correctly.")]
        public void FromText()
        {
            STLDocument stl = null;

            using (Stream stream = GetData("ASCII.stl"))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.ASCII, true, 1024, true))
                {
                    stl = STLDocument.Read(reader);
                }
            }

            ValidateSTL(stl);
        }

        [TestMethod]
        [Description("Ensures that reading binary-based STLs works correctly.")]
        public void FromBinary()
        {
            STLDocument stl = null;

            using (Stream stream = GetData("Binary.stl"))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    {
                        stl = STLDocument.Read(reader);
                    }
                }
            }

            ValidateSTL(stl);
        }

        [TestMethod]
        [Description("Ensures that reading STLs from a string works correctly.")]
        public void FromString()
        {
            string stlText = null;
            STLDocument stl = null;

            using (Stream stream = GetData("ASCII.stl"))
            using (StreamReader reader = new StreamReader(stream))
                stlText = reader.ReadToEnd();

            stl = STLDocument.Read(stlText);

            ValidateSTL(stl);
        }

        [TestMethod]
        [Description("Ensures that reading STLs from a file works correctly.")]
        public void FromFile()
        {
            STLDocument stl = null;

            using (Stream stream = GetData("ASCII.stl"))
            {
                string tempFilePath = Path.GetTempFileName();

                using (TemporaryFileStream tempStream = new TemporaryFileStream(tempFilePath))
                {
                    stream.CopyTo(tempStream);
                    tempStream.Flush();
                    tempStream.Close();
                    stl = STLDocument.Open(tempFilePath);
                }
            }

            ValidateSTL(stl);
        }

        [TestMethod]
        [Description("Ensures that writing string STLs works correctly.")]
        public void WriteString()
        {
            STLDocument stl1 = new STLDocument("WriteString", new List<Facet>()
            {
                new Facet(new Normal( 0, 0, 1), new List<Vertex>()
                {
                    new Vertex( 0, 0, 0),
                    new Vertex(-10, -10, 0),
                    new Vertex(-10, 0, 0)
                }, 0)
            });
            STLDocument stl2 = null;
            byte[] stl1Data = null;
            string stl1String = null;
            byte[] stl2Data = null;
            string stl2String = null;

            using (MemoryStream stream = new MemoryStream())
            {
                stl1.WriteText(stream);
                stl1Data = stream.ToArray();
                stl1String = Encoding.ASCII.GetString(stl1Data);
            }

            using (MemoryStream stream = new MemoryStream(stl1Data))
            {
                stl2 = STLDocument.Read(stream);
                stl2Data = stream.ToArray();
                stl2String = Encoding.ASCII.GetString(stl2Data);
            }

            Assert.IsTrue(stl1.Equals(stl2));
            Assert.AreEqual(stl1String, stl2String);
        }

        [TestMethod]
        [Description("Ensures that writing binary STLs works correctly.")]
        public void WriteBinary()
        {
            STLDocument stl1 = new STLDocument("WriteBinary", new List<Facet>()
            {
                new Facet(new Normal( 0, 0, 1), new List<Vertex>()
                {
                    new Vertex( 0, 0, 0),
                    new Vertex(-10, -10, 0),
                    new Vertex(-10, 0, 0)
                }, 0)
            });
            STLDocument stl2 = null;
            byte[] stl1Data = null;
            byte[] stl2Data = null;

            using (MemoryStream stream = new MemoryStream())
            {
                stl1.WriteBinary(stream);
                stl1Data = stream.ToArray();
            }

            using (MemoryStream stream = new MemoryStream(stl1Data))
            {
                stl2 = STLDocument.Read(stream);
                stl2Data = stream.ToArray();
            }

            Assert.IsTrue(stl1.Equals(stl2));
            Assert.IsTrue(stl1Data.SequenceEqual(stl2Data));
        }

        [TestMethod]
        [Description("Ensures that copying an STL document as text works correctly.")]
        public void CopyAsText()
        {
            STLDocument stlStringFrom = null;
            STLDocument stlStringTo = null;
            STLDocument stlBinaryFrom = null;
            STLDocument stlBinaryTo = null;

            using (Stream inStream = GetData("ASCII.stl"), outStream = new MemoryStream())
            {
                stlStringFrom = STLDocument.Read(inStream);
                stlStringTo = STLDocument.CopyAsText(inStream, outStream);
            }

            Assert.IsNotNull(stlStringFrom);
            Assert.IsNotNull(stlStringTo);
            Assert.IsTrue(stlStringFrom.Equals(stlStringTo));

            using (Stream inStream = GetData("Binary.stl"), outStream = new MemoryStream())
            {
                stlBinaryFrom = STLDocument.Read(inStream);
                stlBinaryTo = STLDocument.CopyAsText(inStream, outStream);
            }

            Assert.IsNotNull(stlBinaryFrom);
            Assert.IsNotNull(stlBinaryTo);
            Assert.IsTrue(stlBinaryFrom.Equals(stlBinaryTo));
        }

        [TestMethod]
        [Description("Ensures that copying an STL document as binary works correctly.")]
        public void CopyAsBinary()
        {
            STLDocument stlStringFrom = null;
            STLDocument stlStringTo = null;
            STLDocument stlBinaryFrom = null;
            STLDocument stlBinaryTo = null;

            using (Stream inStream = GetData("ASCII.stl"), outStream = new MemoryStream())
            {
                stlStringFrom = STLDocument.Read(inStream);
                stlStringTo = STLDocument.CopyAsBinary(inStream, outStream);
            }

            Assert.IsNotNull(stlStringFrom);
            Assert.IsNotNull(stlStringTo);
            Assert.IsTrue(stlStringFrom.Equals(stlStringTo));

            using (Stream inStream = GetData("Binary.stl"), outStream = new MemoryStream())
            {
                stlBinaryFrom = STLDocument.Read(inStream);
                stlBinaryTo = STLDocument.CopyAsBinary(inStream, outStream);
            }

            Assert.IsNotNull(stlBinaryFrom);
            Assert.IsNotNull(stlBinaryTo);
            Assert.IsTrue(stlBinaryFrom.Equals(stlBinaryTo));
        }

        [TestMethod]
        [Description("Ensures that the stream is left open after reading a text-based STL.")]
        public void StreamLeftOpen()
        {
            STLDocument stl = null;

            using (Stream stream = GetData("ASCII.stl"))
            {
                stl = STLDocument.Read(stream);

                try
                {
                    stream.ReadByte();
                }
                catch (ObjectDisposedException)
                {
                    Assert.Fail("Stream is closed.");
                }
            }
        }

        [TestMethod]
        [Description("Ensures that STL equality comparison functions correctly.")]
        public void Equality()
        {
            STLDocument[] stls = new STLDocument[2];

            for (int i = 0; i < stls.Length; i++)
            {
                using (Stream stream = GetData("ASCII.stl"))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        stls[i] = STLDocument.Read(reader);
                    }
                }
            }

            Assert.IsTrue(stls[0].Equals(stls[1]));
        }

        [TestMethod]
        [Description("Ensures that facet appending functions correctly.")]
        public void AppendFacets()
        {
            STLDocument stl1 = null;
            STLDocument stl2 = null;
            int facetCount = 0;

            using (Stream stream = GetData("ASCII.stl"))
            {
                stl1 = STLDocument.Read(stream);
                stl2 = STLDocument.Read(stream);
            }

            ValidateSTL(stl1);
            ValidateSTL(stl2);

            facetCount = (stl1.Facets.Count + stl2.Facets.Count);
            stl1.AppendFacets(stl2);

            ValidateSTL(stl1, facetCount);
        }

        [TestMethod]
        [Description("Ensures that saving to a file functions correctly.")]
        public void SaveToFile()
        {
            STLDocument stl = null;
            STLDocument stlText = null;
            STLDocument stlBinary = null;
            string stlTextPath = Path.GetTempFileName();
            string stlBinaryPath = Path.GetTempFileName();

            using (Stream stream = GetData("ASCII.stl"))
                stl = STLDocument.Read(stream);

            stl.SaveAsText(stlTextPath);
            stlText = STLDocument.Open(stlTextPath);
            stl.SaveAsBinary(stlBinaryPath);
            stlBinary = STLDocument.Open(stlBinaryPath);

            ValidateSTL(stlText);
            ValidateSTL(stlBinary);

            try { File.Delete(stlTextPath); }
            catch { }

            try { File.Delete(stlBinaryPath); }
            catch { }
        }

        private Stream GetData(string filename)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("QuantumConcepts.Formats.StereoLithography.Test.Data.{0}".FormatString(filename));
        }

        private void ValidateSTL(STLDocument stl, int expectedFacetCount = 12)
        {
            Assert.IsNotNull(stl);
            Assert.AreEqual(expectedFacetCount, stl.Facets.Count);

            foreach (Facet facet in stl.Facets)
                Assert.AreEqual(3, facet.Vertices.Count);
        }
    }
}
