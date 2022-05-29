using FluentAssertions;
using System.Reflection;
using System.Text;
using Xunit;

namespace QuantumConcepts.Formats.StereoLithography.Test
{
    public class STLDocumentTests
    {
        [Fact]
        public void Read__FromTextStream()
        {
            STLDocument stlString;

            using (var stream = GetData("ASCII.stl"))
            {
                stlString = STLDocument.Read(stream);
            }

            ValidateSTL(stlString);
        }

        [Fact]
        public void Read__FromBinaryStream()
        {
            STLDocument stlBinary;

            using (var stream = GetData("Binary.stl"))
            {
                stlBinary = STLDocument.Read(stream);
            }

            ValidateSTL(stlBinary);
        }

        [Fact]
        public void Read__FromTextReader()
        {
            STLDocument stl;

            using (var stream = GetData("ASCII.stl"))
            {
                using (var reader = new StreamReader(stream, Encoding.ASCII, true, 1024, true))
                {
                    stl = STLDocument.Read(reader);
                }
            }

            ValidateSTL(stl);
        }

        [Fact]
        public void Read__FromBinaryReader()
        {
            STLDocument stl;

            using (var stream = GetData("Binary.stl"))
            {
                using (var reader = new BinaryReader(stream))
                {
                    {
                        stl = STLDocument.Read(reader);
                    }
                }
            }

            ValidateSTL(stl);
        }

        [Fact]
        public void Read__FromString()
        {
            string stlText;
            STLDocument stl;

            using (var stream = GetData("ASCII.stl"))
            using (var reader = new StreamReader(stream))
            {
                stlText = reader.ReadToEnd();
            }

            stl = STLDocument.Read(stlText);

            ValidateSTL(stl);
        }

        [Fact]
        public void Read__FromStream__LeavesStreamOpen()
        {
            STLDocument stl;

            using (var stream = GetData("ASCII.stl"))
            {
                stl = STLDocument.Read(stream);

                try
                {
                    stream.ReadByte();
                }
                catch (ObjectDisposedException)
                {
                    throw new Exception("Stream is closed.");
                }
            }
        }

        [Fact]
        public void Open__FromFilePath()
        {
            STLDocument stl;

            using (var inStream = GetData("ASCII.stl"))
            {
                string tempFilePath = Path.GetTempFileName();

                using (var outStream = File.Create(tempFilePath))
                {
                    inStream.CopyTo(outStream);
                }

                stl = STLDocument.Open(tempFilePath);

                try
                {
                    File.Delete(tempFilePath);
                }
                catch { /* Ignore. */ }
            }

            ValidateSTL(stl);
        }

        [Fact]
        public void WriteText__ToStream__Then__Read__FromStream__ProducesSameDocument()
        {
            STLDocument stl1 = new STLDocument("WriteString", new List<Facet>()
            {
                new Facet(new Normal( 0.23f, 0, 1), new List<Vertex>()
                {
                    new Vertex( 0, 0, 0),
                    new Vertex(-10.123f, -10, 0),
                    new Vertex(-10.123f, 0, 0)
                }, 0)
            });
            STLDocument stl2;
            byte[] stl1Data;
            string stl1String;
            byte[] stl2Data;
            string stl2String;

            using (var stream = new MemoryStream())
            {
                stl1.WriteText(stream);
                stl1Data = stream.ToArray();
                stl1String = Encoding.ASCII.GetString(stl1Data);
            }

            using (var stream = new MemoryStream(stl1Data))
            {
                stl2 = STLDocument.Read(stream);
                stl2Data = stream.ToArray();
                stl2String = Encoding.ASCII.GetString(stl2Data);
            }

            CompareSTLs(stl1, stl2);
            Assert.Equal(stl1String, stl2String);
        }

        [Fact]
        public void WriteBinary__ToStream__Then__Read__FromStream__ProducesSameDocument()
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
            STLDocument stl2;
            byte[] stl1Data;
            byte[] stl2Data;

            using (var stream = new MemoryStream())
            {
                stl1.WriteBinary(stream);
                stl1Data = stream.ToArray();
            }

            using (var stream = new MemoryStream(stl1Data))
            {
                stl2 = STLDocument.Read(stream);
                stl2Data = stream.ToArray();
            }

            CompareSTLs(stl1, stl2, true);
            Assert.True(stl1Data.SequenceEqual(stl2Data));
        }

        [Fact]
        public void SaveAsText()
        {
            STLDocument stl;
            STLDocument stlText;
            string stlTextPath = Path.GetTempFileName();

            using (var stream = GetData("ASCII.stl"))
            {
                stl = STLDocument.Read(stream);
            }

            stl.SaveAsText(stlTextPath);
            stlText = STLDocument.Open(stlTextPath);

            ValidateSTL(stlText);

            try { File.Delete(stlTextPath); }
            catch { }
        }

        [Fact]
        public void SaveAsBinary()
        {
            STLDocument stl;
            STLDocument stlBinary;
            string stlBinaryPath = Path.GetTempFileName();

            using (var stream = GetData("ASCII.stl"))
            {
                stl = STLDocument.Read(stream);
            }

            stl.SaveAsBinary(stlBinaryPath);
            stlBinary = STLDocument.Open(stlBinaryPath);

            ValidateSTL(stlBinary);

            try { File.Delete(stlBinaryPath); }
            catch { }
        }

        [Fact]
        public void CopyAsText()
        {
            STLDocument stlStringFrom;
            STLDocument stlStringTo;
            STLDocument stlBinaryFrom;
            STLDocument stlBinaryTo;

            using (var inStream = GetData("ASCII.stl"))
            using (var outStream = new MemoryStream())
            {
                stlStringFrom = STLDocument.Read(inStream);
                stlStringTo = STLDocument.CopyAsText(inStream, outStream);
            }

            Assert.NotNull(stlStringFrom);
            Assert.NotNull(stlStringTo);
            Assert.Equal(stlStringFrom, stlStringTo);

            using (var inStream = GetData("Binary.stl"))
            using (var outStream = new MemoryStream())
            {
                stlBinaryFrom = STLDocument.Read(inStream);
                stlBinaryTo = STLDocument.CopyAsText(inStream, outStream);
            }

            Assert.NotNull(stlBinaryFrom);
            Assert.NotNull(stlBinaryTo);
            Assert.Equal(stlBinaryFrom, stlBinaryTo);
        }

        [Fact]
        public void CopyAsBinary()
        {
            STLDocument stlStringFrom;
            STLDocument stlStringTo;
            STLDocument stlBinaryFrom;
            STLDocument stlBinaryTo;

            using (var inStream = GetData("ASCII.stl"))
            using (var outStream = new MemoryStream())
            {
                stlStringFrom = STLDocument.Read(inStream);
                stlStringTo = STLDocument.CopyAsBinary(inStream, outStream);
            }

            Assert.NotNull(stlStringFrom);
            Assert.NotNull(stlStringTo);
            Assert.Equal(stlStringFrom, stlStringTo);

            using (var inStream = GetData("Binary.stl"))
            using (var outStream = new MemoryStream())
            {
                stlBinaryFrom = STLDocument.Read(inStream);
                stlBinaryTo = STLDocument.CopyAsBinary(inStream, outStream);
            }

            Assert.NotNull(stlBinaryFrom);
            Assert.NotNull(stlBinaryTo);
            Assert.Equal(stlBinaryFrom, stlBinaryTo);
        }

        [Fact]
        public void Equalz()
        {
            var stls = new STLDocument[2];

            for (int i = 0; i < stls.Length; i++)
            {
                using (var stream = GetData("ASCII.stl"))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        stls[i] = STLDocument.Read(reader);
                    }
                }
            }

            Assert.Equal(stls[0], stls[1]);
        }

        [Fact]
        public void AppendFacets()
        {
            STLDocument stl1;
            STLDocument stl2;
            int facetCount = 0;

            using (var stream = GetData("ASCII.stl"))
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

        [Fact]
        public void ShiftFacets()
        {
            STLDocument stl1;
            STLDocument stl2;
            Vertex shift = new Vertex(100, -100, 50);

            using (var stream = GetData("ASCII.stl"))
            {
                stl1 = STLDocument.Read(stream);
                stl2 = STLDocument.Read(stream);
            }

            stl2.Facets.Shift(shift);

            for (int f = 0; f < stl1.Facets.Count; f++)
            {
                for (int v = 0; v < stl1.Facets[f].Vertices.Count; v++)
                {
                    Assert.Equal(stl1.Facets[f].Vertices[v].X, stl2.Facets[f].Vertices[v].X - shift.X);
                    Assert.Equal(stl1.Facets[f].Vertices[v].Y, stl2.Facets[f].Vertices[v].Y - shift.Y);
                    Assert.Equal(stl1.Facets[f].Vertices[v].Z, stl2.Facets[f].Vertices[v].Z - shift.Z);
                }
            }
        }

        [Fact]
        public void InvertFacets()
        {
            STLDocument stl1;
            STLDocument stl2;

            using (var stream = GetData("ASCII.stl"))
            {
                stl1 = STLDocument.Read(stream);
                stl2 = STLDocument.Read(stream);
            }

            stl2.Facets.Invert();

            for (int f = 0; f < stl1.Facets.Count; f++)
            {
                for (int v = 0; v < stl1.Facets[f].Vertices.Count; v++)
                {
                    Assert.Equal(stl1.Facets[f].Normal.X, (stl2.Facets[f].Normal.X * -1));
                    Assert.Equal(stl1.Facets[f].Normal.Y, (stl2.Facets[f].Normal.Y * -1));
                    Assert.Equal(stl1.Facets[f].Normal.Z, (stl2.Facets[f].Normal.Z * -1));
                }
            }
        }

        private Stream GetData(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var interpolatedFilename = $"Tests.Data.{filename}";
            var stream = assembly.GetManifestResourceStream(interpolatedFilename);

            if (stream == null)
            {
                throw new Exception($"Failed to load resource stream: {interpolatedFilename}");
            }
            else
            {
                return stream;
            }
        }

        private void ValidateSTL(STLDocument stl, int expectedFacetCount = 12)
        {
            Assert.NotNull(stl);
            Assert.Equal(expectedFacetCount, stl.Facets.Count);

            foreach (var facet in stl.Facets)
                Assert.Equal(3, facet.Vertices.Count);
        }

        private void CompareSTLs(STLDocument doc1, STLDocument doc2, bool isBinary = false)
        {
            CompareSTLsLeftToRight(doc1, doc2, isBinary);
            CompareSTLsLeftToRight(doc2, doc1, isBinary);
        }

        private void CompareSTLsLeftToRight(STLDocument left, STLDocument right, bool isBinary = false)
        {
            if (!isBinary) right.Name.Should().Be(left.Name);

            right.Facets.Count.Should().Be(left.Facets.Count);

            for (var f = 0; f < left.Facets.Count; f++)
            {
                var leftFacet = left.Facets[f];
                var rightFacet = right.Facets[f];

                if (isBinary) rightFacet.AttributeByteCount.Should().Be(leftFacet.AttributeByteCount);

                rightFacet.Normal.X.Should().Be(leftFacet.Normal.X);
                rightFacet.Normal.Y.Should().Be(leftFacet.Normal.Y);
                rightFacet.Normal.Z.Should().Be(leftFacet.Normal.Z);

                for (var v = 0; v < leftFacet.Vertices.Count; v++)
                {
                    var leftVertice = leftFacet.Vertices[v];
                    var rightVertice = rightFacet.Vertices[v];

                    rightVertice.X.Should().Be(leftVertice.X, $"vertices at index {v} should be equal");
                    rightVertice.Y.Should().Be(leftVertice.Y, $"vertices at index {v} should be equal");
                    rightVertice.Z.Should().Be(leftVertice.Z, $"vertices at index {v} should be equal");
                }
            }
        }
    }
}
