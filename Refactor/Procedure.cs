using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor
{
    public abstract class Procedure
    {
        public abstract void Execute();
        public abstract List<string> Description();
    }
}
