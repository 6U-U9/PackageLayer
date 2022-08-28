using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor
{
    public struct Package
    {
        public static Dictionary<string, Package> packages = new();
        public static Package NullPackage = new Package("");
        public static Package Create(string name, int? human = null)
        {
            if (!packages.ContainsKey(name))
            {
                Package p = new Package(name, human);
                packages[name] = p;
            }
            return packages[name];
        }
        public static Package Get(string name)
        {
            return packages[name];
        }
        public string name { get; set; }
        public int? human { get; set; }
        public List<string> category = new List<string>();
        public int relationCount { get { return dependency.Count + dependent.Count; } }

        public List<Package> dependency = new List<Package>();
        public List<Package> dependent = new List<Package>();

        private Package(string name, int? human = null)
        {
            this.name = name;
            this.human = human;
        }
        public override string ToString()
        {
            return name;
        }
        public override bool Equals(object? obj)
        {
            if (obj is Package)
                return name == ((Package)obj).name;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
        public static bool operator ==(Package p1, Package p2)
        {
            return p1.Equals(p2);
        }
        public static bool operator !=(Package p1, Package p2)
        {
            return !p1.Equals(p2);
        }
    }
}
