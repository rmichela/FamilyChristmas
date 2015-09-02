using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace ChristmasWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Match(string id, string grandma)
        {
            string configGrandma = ConfigurationManager.AppSettings["grandma"];
            if (grandma.Equals(configGrandma, StringComparison.InvariantCultureIgnoreCase))
            {
                ViewBag.Grandma = grandma;
                if (id == null)
                {
                    return Who();
                }
                else
                {
                    return Match(id);
                }
            }
            else
            {
                return View("Error");
            }
        }

        private ActionResult Who()
        {
            ViewBag.Who = ReadFamily()
                        .Select(f => f.Name)
                        .OrderBy(n => n)
                        .Select(n => new SelectListItem
                        {
                            Text = n,
                            Value = n
                        }).ToArray();
            return View("Who");
        }

        private ActionResult Match(string forWho)
        {
            var rng = new Random(DateTime.UtcNow.Year);
            List<FamilyMember> family = ReadFamily().Shuffle(rng);
            while (Resequence(family)) { /* Keep trying */ }

            ViewBag.Who = forWho;
            ViewBag.Match = GetMatch(family, forWho);
            return View("Match");
        }

        private static List<FamilyMember> ReadFamily()
        {
            var family = new List<FamilyMember>();
            int group = 0;
            foreach (string line in LoadResource("Family.txt"))
            {
                if (line.Length == 0 || line.StartsWith("#"))
                {
                    continue;
                }

                group++;
                foreach (string name in line.Split(new[] { ',' }))
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

        private static string GetMatch(List<FamilyMember> family, string forWho)
        {
            var next = Next(family.Count);
            int i = family.FindIndex(f => f.Name == forWho);
            return family[next(i)].Name;
        }

        private static Func<int, int> Next(int size)
        {
            return (current) => (current + 1) % size;
        }

        private static IEnumerable<string> LoadResource(string resource)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChristmasWeb." + resource))
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        private class FamilyMember
        {
            public string Name { get; set; }
            public int Family { get; set; }
        }
    }

    static class ListExtensions
    {
        public static List<T> Shuffle<T>(this List<T> list, Random rng)
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

        public static void Swap<T>(this List<T> list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}