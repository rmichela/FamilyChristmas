using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FamilyChristmas
{
    public static class Program
    {
        static void Main(string[] args)
        {
            List<FamilyMember> family = ReadFamily().Shuffle();
            while (Resequence(family)) {}
            PrintMatches(family);
        }

        private static List<FamilyMember> ReadFamily()
        {
            var family = new List<FamilyMember>();
            int group = 0;
            foreach (string line in File.ReadAllLines("Family.txt"))
            {
                if (line.Length == 0 || line.StartsWith("#"))
                {
                    continue;
                }

                group++;
                foreach (string name in line.Split(new[] {','}))
                {
                    family.Add(new FamilyMember { Name = name, Family = group });
                }
            }
            return family;
        }

        private static bool Resequence(List<FamilyMember> family)
        {
            var next = Next(family.Count);
            bool changed = false;

            for (int i = 0; i < family.Count; i++)
            {
                if (family[i].Family == family[next(i)].Family)
                {
                    family.Swap(next(i), next(next(i)));
                    changed = true;
                }
            }
            return changed;
        }

        private static void PrintMatches(List<FamilyMember> family)
        {
            string format = string.Format("{{0,-{0}}} => {{1}}", family.Max(f => f.Name.Length));

            var next = Next(family.Count);
            for (int i = 0; i < family.Count; i++)
            {
                Console.WriteLine(format, family[i].Name, family[next(i)].Name);
            }
        }

        private static Func<int, int> Next(int size)
        {
            return (current) => (current + 1)%size;
        }

        private static List<T> Shuffle<T>(this List<T> list)
        {
            var rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        private static void Swap<T>(this List<T> list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        private class FamilyMember
        {
            public string Name { get; set; }
            public int Family { get; set; }
        }
    }
}