namespace Raifhack.SkepticalBeavers.Server.Model
{
    internal abstract class EventBase
    {
        public virtual string Type => GetType().Name;
    }
}