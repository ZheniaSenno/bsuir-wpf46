using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RangeCollection
{
    public class Helper
    {
        public int Addend { get; set; }

        public IEnumerator<int> GetEnumerator()
        {
            int start = 0;
            while (true) yield return (start += Addend);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var col = new RangeCollection(Tuple.Create(1, 3), Tuple.Create(2, 5), Tuple.Create(16, 20)) { { 35, 37 }, "50 58", "53 62" };
            Console.WriteLine(string.Join(" ", col));
            col.Add(4, 18);
            Console.WriteLine(string.Join(" ", col));
            col.Add(21, 27);
            Console.WriteLine(string.Join(" ", col));
            Console.WriteLine();

            var h = new Helper();
            foreach (int x in h)
            {
                Console.WriteLine(x);
                ++h.Addend;
                if (x > 100) break;
            }
            Console.WriteLine();

            Type nested = typeof(Helper).GetNestedTypes(BindingFlags.NonPublic)[0];
            Console.WriteLine($"{nested.Name} : {string.Join(", ", nested.GetInterfaces().Select(i => i.Name))}");
            foreach (var f in nested.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                Console.WriteLine($"{(f.IsStatic ? "static" : "")} {f.FieldType.Name} {f.Name}");
            Console.WriteLine();

            foreach (var n in typeof(RangeCollection).GetNestedTypes(BindingFlags.NonPublic))
            {
                Console.WriteLine($"{n.Name} : {string.Join(", ", n.GetInterfaces().Select(i => i.Name))}");
                foreach (var f in n.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                    Console.WriteLine($"{(f.IsStatic ? "static" : "")} {f.FieldType.Name} {f.Name}");
                Console.WriteLine();
            }
        }
    }
}
