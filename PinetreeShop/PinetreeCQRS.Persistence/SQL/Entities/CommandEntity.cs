﻿using System;

namespace PinetreeCQRS.Persistence.SQL.Entities
{
    public class CommandEntity
    {
        public int Id { get; set; }
        public string QueueName { get; set; }
        public Guid AggregateId { get; set; }
        public Guid CommandId { get; set; }
        public Guid CausationId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid ProcessId { get; set; }

        public string CommandPayload { get; set; }
    }
}
