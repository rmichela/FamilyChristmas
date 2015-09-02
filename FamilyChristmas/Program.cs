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
            var rngLy = new Random();
            List<FamilyMember> familyLy = ReadFamily().Shuffle(rngLy);
            while (Resequence(familyLy)) {}

            var rng = new Random();
            List<FamilyMember> family = ReadFamily().Shuffle(rng);
            int i = 0;
            while (Resequence(family, familyLy) && ++i < 100) {}

            PrintMatches(familyLy);
            Console.WriteLine("======= {0} =======", i);
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

        private static bool Resequence(List<FamilyMember> family, List<FamilyMember> familyLy = null)
        {
            var next = Next(family.Count);
            bool changed = false;

            for (int i = 0; i < family.Count; i++)
            {
                if (family[i].Family == family[next(i)].Family || (familyLy != null && family[next(i)].Name == familyLy[next(i)].Name))
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

        private static List<T> Shuffle<T>(this List<T> list, Random rng)
        {
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