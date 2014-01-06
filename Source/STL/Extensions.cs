using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Formats.StereoLithography
{
    public static class Extensions
    {
        /// <summary>Shifts the vertices within the <paramref name="facets"/> enumerable by <paramref name="x"/>, <paramref name="y"/> and <paramref name="z"/>.</summary>
        /// <param name="facets">The vertices within these facets will be shifted.</param>
        /// <param name="x">The amount to shift the vertices along the X axis.</param>
        /// <param name="y">The amount to shift the vertices along the Y axis.</param>
        /// <param name="z">The amount to shift the vertices along the Z axis.</param>
        public static void Shift(this IEnumerable<Facet> facets, float x, float y, float z)
        {
            facets.Shift(new Vertex(x, y, z));
        }

        /// <summary>Shifts the vertices within the <paramref name="facets"/> enumerable by the X, Y and Z values in the <paramref name="shift"/> parameter.</summary>
        /// <param name="facets">The vertices to be shifted.</param>
        /// <param name="shift">The amount to shift each vertex.</param>
        public static void Shift(this IEnumerable<Facet> facets, Vertex shift)
        {
            facets.ForEach(f => f.Shift(shift));
        }

        /// <summary>Shifts the <paramref name="vertices"/> enumerable by <paramref name="x"/>, <paramref name="y"/> and <paramref name="z"/>.</summary>
        /// <param name="vertices">The vertices to be shifted.</param>
        /// <param name="x">The amount to shift the vertices along the X axis.</param>
        /// <param name="y">The amount to shift the vertices along the Y axis.</param>
        /// <param name="z">The amount to shift the vertices along the Z axis.</param>
        public static void Shift(this IEnumerable<Vertex> vertices, float x, float y, float z)
        {
            vertices.Shift(new Vertex(x, y, z));
        }

        /// <summary>Shifts the <paramref name="vertices"/> enumerable by the X, Y and Z values in the <paramref name="shift"/> parameter.</summary>
        /// <param name="vertices">The vertices to be shifted.</param>
        /// <param name="shift">The amount to shift each vertex.</param>
        public static void Shift(this IEnumerable<Vertex> vertices, Vertex shift)
        {
            vertices.ForEach(v => v.Shift(shift));
        }
    }
}
