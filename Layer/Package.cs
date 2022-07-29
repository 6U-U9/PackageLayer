using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer
{
    public struct Package
    {
        public string name { get; set; }
        public Package(string name)
        {
            this.name = name;  
        }
        public override string ToString()
        {
            return name;
        }
        public override bool Equals(object? obj)
        {
            if(obj is Package)
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
