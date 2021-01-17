using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Newtonsoft.Json;
using Raifhack.SkepticalBeavers.Server.Model;
using Raifhack.SkepticalBeavers.Server.Model.Aggregates;

namespace Raifhack.SkepticalBeavers.Server.Services
{
    internal sealed class AggregateRepository
    {
        private readonly EventStoreClient _client;

        public AggregateRepository(EventStoreClient client)
        {
            _client = client;
        }

        public async Task SaveAsync<T>(T aggregate, CancellationToken cancellationToken = default)
            where T : AggregateBase
        {
            var streamName = GetStreamName(typeof(T), aggregate.Id);

            var events = aggregate.GetChanges()
                .Select(e => new EventData(Uuid.NewUuid(),
                    e.Type,
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e)),
                    Encoding.UTF8.GetBytes(e.GetType().FullName ?? string.Empty)))
                .ToList();

            await _client.AppendToStreamAsync(streamName, StreamState.Any, events,
                cancellationToken: cancellationToken);
        }

        public async Task<T> LoadAsync<T>(Guid aggregateId, CancellationToken cancellationToken = default) where T : AggregateBase, new()
        {
            if (aggregateId == Guid.Empty)
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(aggregateId));

            var aggregate = new T();
            var streamName = GetStreamName(typeof(T), aggregateId);

            try
            {
                await foreach (var @event in _client.ReadStreamAsync(Direction.Forwards, streamName,
                    StreamPosition.Start, cancellationToken: cancellationToken))
                {
                    var e = DeserializeEvent(@event);
                    aggregate.Apply(e, true);
                }
            }
            catch(StreamNotFoundException _){}

            return aggregate;
        }

        public static EventBase DeserializeEvent(ResolvedEvent @event)
        {
            // get type of the event from metadata
            var eventType = Type.GetType(Encoding.UTF8.GetString(@event.OriginalEvent.Metadata.Span));

            if(eventType == null) throw new ApplicationException("Unknown event type");

            // deserialize into appropriate object based on retrieved type
            var e = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(@event.OriginalEvent.Data.Span), eventType) as EventBase;

            if(e == null) throw new ApplicationException("Event deserialization issue");

            return e;
        }

        private static string GetStreamName(MemberInfo aggregateType, Guid id)
        {
            return $"{id:N}";
        }
    }
}