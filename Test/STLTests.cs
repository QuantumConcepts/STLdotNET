using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantumConcepts.Formats.StereoLithography;
using QuantumConcepts.Common.Extensions;
using System.Linq;

namespace QuantumConcepts.Formats.StereoLithography.Test
{
    [TestClass]
    public class STLTests
    {
        [TestMethod]
        public void FromString()
        {
            STL stl = null;

            using (Stream stream = GetData("ASCII.stl"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    stl = STL.Read(reader);
                }
            }

            Assert.IsNotNull(stl);
            Assert.AreEqual(12, stl.Facets.Count);

            foreach (Facet facet in stl.Facets)
                Assert.AreEqual(3, facet.Vertices.Count);
        }

        [TestMethod]
        public void FromBinary()
        {
            STL stl = null;

            using (Stream stream = GetData("Binary.stl"))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    {
                        stl = STL.Read(reader);
                    }
                }
            }

            Assert.IsNotNull(stl);
            Assert.AreEqual(12, stl.Facets.Count);

            foreach (Facet facet in stl.Facets)
                Assert.AreEqual(3, facet.Vertices.Count);
        }

        [TestMethod]
        public void WriteString()
        {
            STL stl1 = new STL("WriteString", new List<Facet>()
            {
                new Facet(new Normal( 0, 0, 1), new List<Vertex>()
                {
                    new Vertex( 0, 0, 0),
                    new Vertex(-10, -10, 0),
                    new Vertex(-10, 0, 0)
                }, 0)
            });
            STL stl2 = null;
            byte[] stl1Data = null;
            string stl1String = null;
            byte[] stl2Data = null;
            string stl2String = null;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    stl1.Write(writer);
                }

                stl1Data = stream.ToArray();
                stl1String = Encoding.ASCII.GetString(stl1Data);
            }

            using (MemoryStream stream = new MemoryStream(stl1Data))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    stl2 = STL.Read(reader);
                }

                stl2Data = stream.ToArray();
                stl2String = Encoding.ASCII.GetString(stl2Data);
            }

            Assert.IsTrue(stl1.Equals(stl2));
            Assert.AreEqual(stl1String, stl2String);
        }

        [TestMethod]
        public void WriteBinary()
        {
            STL stl1 = new STL("WriteBinary", new List<Facet>()
            {
                new Facet(new Normal( 0, 0, 1), new List<Vertex>()
                {
                    new Vertex( 0, 0, 0),
                    new Vertex(-10, -10, 0),
                    new Vertex(-10, 0, 0)
                }, 0)
            });
            STL stl2 = null;
            byte[] stl1Data = null;
            byte[] stl2Data = null;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    stl1.Write(writer);
                }

                stl1Data = stream.ToArray();
            }

            using (MemoryStream stream = new MemoryStream(stl1Data))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    stl2 = STL.Read(reader);
                }

                stl2Data = stream.ToArray();
            }

            Assert.IsTrue(stl1.Equals(stl2));
            Assert.IsTrue(stl1Data.SequenceEqual(stl2Data));
        }

        [TestMethod]
        public void Equality()
        {
            STL[] stls = new STL[2];

            for (int i = 0; i < stls.Length; i++)
            {
                using (Stream stream = GetData("ASCII.stl"))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        stls[i] = STL.Read(reader);
                    }
                }
            }

            Assert.IsTrue(stls[0].Equals(stls[1]));
        }

        private Stream GetData(string filename)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("QuantumConcepts.Formats.StereoLithography.Test.Data.{0}".FormatString(filename));
        }
    }
}
