using System;
using System.Collections.Generic;
using System.Globalization;

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

        /// <summary>Inverts the <see cref="Normal"/> within the <paramref name="facets"/> enumerable.</summary>
        /// <param name="facets">The facets to invert.</param>
        public static void Invert(this IEnumerable<Facet> facets)
        {
            facets.ForEach(f => f.Normal.Invert());
        }

        /// <summary>Iterates the provided enumerable, applying the provided action to each element.</summary>
        /// <param name="items">The items upon which to apply the action.</param>
        /// <param name="action">The action to apply to each item.</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    action(item);
                }
            }
        }

        /// <summary>Iterates the provided enumerable, applying the provided action to each element.</summary>
        /// <param name="items">The items upon which to apply the action.</param>
        /// <param name="predicate">The action to apply to each item.</param>
        public static bool All<T>(this IEnumerable<T> items, Func<int, T, bool> predicate)
        {
            if (items != null)
            {
                var index = 0;

                foreach (var item in items)
                {
                    if (!predicate(index, item))
                    {
                        return false;
                    }

                    index++;
                }
            }

            return true;
        }

        /// <summary>Checks if the provided value is null or empty.</summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the provided value is null or empty.</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>Interpolates the provided formatted string with the provided args using the default culture.</summary>
        /// <param name="format">The formatted string.</param>
        /// <param name="args">The values to use for interpolation.</param>
        public static string Interpolate(this string format, params object[] args)
        {
            return format.Interpolate(CultureInfo.InvariantCulture, args);
        }

        /// <summary>Interpolates the provided formatted string with the provided args.</summary>
        /// <param name="format">The formatted string.</param>
        /// <param name="culture">The culture info to use.</param>
        /// <param name="args">The values to use for interpolation.</param>
        public static string Interpolate(this string format, CultureInfo culture, params object[] args)
        {
            if (format != null)
            {
                return string.Format(culture, format, args);
            }

            return null;
        }
    }
}
