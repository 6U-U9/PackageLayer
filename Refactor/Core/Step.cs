namespace Refactor.Core;

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

    public virtual string ChineseDescription
    {
        get { return ""; }
    }

    public virtual TO Process(TI input)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return ChineseDescription;
    }
}