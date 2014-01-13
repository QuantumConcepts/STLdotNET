﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantumConcepts.Formats.StereoLithography;
using System.Linq;
using System.Diagnostics;

namespace QuantumConcepts.Formats.StereoLithography.Test
{
    [TestClass]
    public class STLTests
    {
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
                using (StreamReader reader = new StreamReader(stream, Encoding.ASCII, true, 1024))
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
            STLDocument stl = STLDocument.Open(@"Data\ASCII.stl");
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
        [Description("Tests that facet ordering isn't important in equality checks")]
        public void EqualityUnorderedFacets()
        {
            Facet a = new Facet(new Normal(0, 0, 1), 
                new Vertex[] { new Vertex(0, 0, 0), new Vertex(1, 0, 0), new Vertex(0, 1, 0) }, 0);
            
            Facet b = new Facet(new Normal(0, 0, 1),
                new Vertex[] { new Vertex(1, 0, 0), new Vertex(1, 1, 0), new Vertex(0, 1, 0) }, 0);
            
            Facet c = new Facet(new Normal(1, 0, 0),
                new Vertex[] { new Vertex(0, 0, 0), new Vertex(0, 1, 0), new Vertex(0, 0, 1) }, 0);

            Facet a_reversed_normal = new Facet(new Normal(0, 0, -1), 
                new Vertex[] { new Vertex(0, 0, 0), new Vertex(1, 0, 0), new Vertex(0, 1, 0) }, 0);

            var stl1 = new STLDocument();
            var stl2 = new STLDocument();
            stl1.AppendFacets(new Facet[] { a, b, c });
            stl2.AppendFacets(new Facet[] { c, b, a });

            Assert.IsTrue(stl1.Equals(stl2), "Comparing two STLDocument's with the same facets in a different order should return true");

            stl2 = new STLDocument();
            stl2.AppendFacets(new Facet[]{a_reversed_normal, b, c});

            Assert.IsFalse(stl1.Equals(stl2), "Comparing facets with a reversed normal should result in a false comparison");
        }

        [TestMethod]
        [Description("Tests that vertex ordering is important in equality checks")]
        public void EqualityUnorderedVertices()
        {
            Facet a = new Facet(new Normal(0, 0, 1),
                new Vertex[] { new Vertex(0, 0, 0), new Vertex(1, 0, 0), new Vertex(0, 1, 0) }, 0);

            Facet b = new Facet(new Normal(0, 0, 1),
                new Vertex[] { new Vertex(1, 0, 0), new Vertex(0, 1, 0), new Vertex(0, 0, 0) }, 0);

            var stl1 = new STLDocument();
            var stl2 = new STLDocument();
            stl1.AppendFacets(new Facet[] { a });
            stl2.AppendFacets(new Facet[] { b });

            Assert.IsFalse(stl1.Equals(stl2), "STLDocument's with vertices in different orders should not be equal");
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

        [TestMethod]
        [Description("Ensures that facet (vertex) shifting functions correctly.")]
        public void ShiftFacets()
        {
            STLDocument stl1 = null;
            STLDocument stl2 = null;
            Vertex shift = new Vertex(100, -100, 50);

            using (Stream stream = GetData("ASCII.stl"))
            {
                stl1 = STLDocument.Read(stream);
                stl2 = STLDocument.Read(stream);
            }

            stl2.Facets.Shift(shift);

            for (int f = 0; f < stl1.Facets.Count; f++)
            {
                for (int v = 0; v < stl1.Facets[f].Vertices.Count; v++)
                {
                    Assert.AreEqual(stl1.Facets[f].Vertices[v].X, stl2.Facets[f].Vertices[v].X - shift.X);
                    Assert.AreEqual(stl1.Facets[f].Vertices[v].Y, stl2.Facets[f].Vertices[v].Y - shift.Y);
                    Assert.AreEqual(stl1.Facets[f].Vertices[v].Z, stl2.Facets[f].Vertices[v].Z - shift.Z);
                }
            }
        }

        [TestMethod]
        [Description("Ensures that facet (normal) inversion functions correctly.")]
        public void InvertFacets()
        {
            STLDocument stl1 = null;
            STLDocument stl2 = null;

            using (Stream stream = GetData("ASCII.stl"))
            {
                stl1 = STLDocument.Read(stream);
                stl2 = STLDocument.Read(stream);
            }

            stl2.Facets.Invert();

            for (int f = 0; f < stl1.Facets.Count; f++)
            {
                for (int v = 0; v < stl1.Facets[f].Vertices.Count; v++)
                {
                    Assert.AreEqual(stl1.Facets[f].Normal.X, (stl2.Facets[f].Normal.X * -1));
                    Assert.AreEqual(stl1.Facets[f].Normal.Y, (stl2.Facets[f].Normal.Y * -1));
                    Assert.AreEqual(stl1.Facets[f].Normal.Z, (stl2.Facets[f].Normal.Z * -1));
                }
            }
        }

        private Stream GetData(string filename)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("QuantumConcepts.Formats.StereoLithography.Test.Data.{0}", filename));
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
