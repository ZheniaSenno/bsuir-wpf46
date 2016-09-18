using System;
using System.Collections.Generic;
using System.Collections;

namespace CreateArray
{
    static class Program
    {
        private static Array _CreateArray(Type basicType, int[] lengths, int last)
        {
            if (last == 0) return Array.CreateInstance(basicType, lengths[0]);

            Array x = _CreateArray(basicType, lengths, last - 1);
            Array result = Array.CreateInstance(x.GetType(), lengths[last]);
            if (result.Length > 0) result.SetValue(x, 0);
            for (int i = 1; i < result.Length; ++i) result.SetValue(_CreateArray(basicType, lengths, last - 1), i);
            return result;
        }

        public static Array CreateArray(Type basicType, params int[] lengths)
        {
            if (lengths.Length == 0) throw new ArgumentException("Zero-dimensional arrays are not allowed.", nameof(lengths));
            Array.Reverse(lengths);
            return _CreateArray(basicType, lengths, lengths.Length - 1);
        }

        public static IEnumerable<int[]> Generate(params int[] max)
        {
            if (max == null) throw new ArgumentNullException(nameof(max));
            if (Array.Exists(max, n => n <= 0)) throw new ArgumentException("Negative or zero values are not allowed", nameof(max));
            if (max.Length == 0) { yield return max; yield break; }
            int i = max.Length - 1;
            int[] result = new int[max.Length];
            while (result[0] < max[0])
            {
                yield return result;
                ++result[i];
                while (i > 0 && result[i] == max[i])
                {
                    result[i] = 0;
                    ++result[--i];
                }
                i = max.Length - 1;
            }
        }

        private static Array _CreateArray(Type basicType, int[][] lengths, int last)
        {
            if (last == 0) return Array.CreateInstance(basicType, lengths[0]);

            Array x = _CreateArray(basicType, lengths, last - 1);
            Array result = Array.CreateInstance(x.GetType(), lengths[last]);
            if (result.Length > 0)
            {
                result.SetValue(x, new int[result.Rank]);
                var en = Generate(lengths[last]).GetEnumerator(); en.MoveNext();
                while (en.MoveNext())
                    result.SetValue(_CreateArray(basicType, lengths, last - 1), en.Current);
            }
            return result;
        }

        public static Array CreateArray(Type basicType, params int[][] lengths)
        {
            if (lengths.Length == 0 || Array.Exists(lengths, n => n.Length == 0)) throw new ArgumentException("Zero-dimensional arrays are not allowed.", nameof(lengths));
            Array.Reverse(lengths);
            return _CreateArray(basicType, lengths, lengths.Length - 1);
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable ie)
        {
            foreach (var i in ie)
            {
                if (i is T)
                    yield return (T)i;
                else if (i is IEnumerable)
                    foreach (var j in ((IEnumerable)i).Flatten<T>()) yield return j;
                else
                    throw new InvalidOperationException("Enumeration encountered a value of non-conforming type.");
            }
        }

        static void Main(string[] args)
        {
            double[][][][] m1 = (double[][][][])CreateArray(typeof(double), 2, 3, 4, 2);
            m1[0][1][2][1] = 3;
            m1[1][2][0][0] = 5;
            double[,,,] m2 = new double[2, 3, 4, 2];
            m2[0, 1, 2, 1] = 3;
            m2[1, 2, 0, 0] = 5;
            double[,,][,] x = (double[,,][,])CreateArray(typeof(double), new[] { 2, 4, 1 }, new[] { 3, 2 });
            x[1, 2, 0][2, 1] = 3;
            x[0, 3, 0][0, 1] = 5;
            Console.WriteLine(string.Join(" ", m1.Flatten<double>()));
            Console.WriteLine(string.Join(" ", m2.Flatten<double>()));
            Console.WriteLine(string.Join(" ", x.Flatten<double>()));
            foreach (var a in Generate(3, 8, 4))
                Console.WriteLine(string.Join(" ", a));
            Console.WriteLine();
            foreach (var a in Generate(2, 2, 1, 3, 4))
                Console.WriteLine(string.Join(" ", a));
        }
    }
}
