using System;
using System.Collections.Generic;
using System.Linq;

namespace Raifhack.SkepticalBeavers.Server.Model.Aggregates
{
    internal abstract class AggregateBase
    {
        protected readonly IList<EventBase> Changes = new List<EventBase>();

        public Guid Id { get; protected set; }

        /// <summary>
        /// Version indicates how many events have been applied
        /// </summary>
        public long Version { get; protected set; } = -1;

        public EventBase[] GetChanges() => Changes.ToArray();

        protected abstract void ModifyState(EventBase @event);
        public virtual void Apply(EventBase @event, bool recreating = false)
        {
            ModifyState(@event);
            ++Version;
            if(!recreating) Changes.Add(@event);
        }
    }

    internal abstract class AggregateBase<TModificationResult> : AggregateBase
    {
        protected abstract TModificationResult ModifyStateWithResult(EventBase @event);

        /// <inheritdoc />
        protected override void ModifyState(EventBase @event)
        {
            ModifyStateWithResult(@event);
        }

        /// <inheritdoc />
        public override void Apply(EventBase @event, bool recreating = false)
        {
            ApplyWithResult(@event, recreating);
        }

        public TModificationResult ApplyWithResult(EventBase @event, bool recreating = false)
        {
            var result = ModifyStateWithResult(@event);
            ++Version;
            if(!recreating) Changes.Add(@event);
            return result;
        }
    }
}