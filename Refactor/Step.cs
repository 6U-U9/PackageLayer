using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor;

public abstract class Step<TI,TO>
{
    public virtual string StepDescription 
    {
        get { return ""; }
    }
    
    public virtual string DetailDescription
    {
        get { return ""; }
    }

    public virtual TO Process(TI input)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return StepDescription+" : "+ DetailDescription;
    }
}