﻿using Newtonsoft.Json;
using PinetreeCQRS.Infrastructure;
using PinetreeCQRS.Infrastructure.Commands;
using PinetreeCQRS.Infrastructure.Events;
using PinetreeCQRS.Infrastructure.Repositories;
using PinetreeCQRS.Infrastructure.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PinetreeShop.Domain.Tests
{
    public abstract class ProcessManagerTestBase<TProcessManager> where TProcessManager : IProcessManager, new()
    {
        protected TestEventStore _eventStore = new TestEventStore();
        protected TestCommandQueue _commandQueue = new TestCommandQueue();
        protected List<Tuple<Type, IEvent>> _preConditions = new List<Tuple<Type, IEvent>>();
        protected ProcessManagerRepository _processManagerRepository;

        protected abstract IProcessEventHandler BuildApplication();

        protected void TearDown()
        {
            _preConditions.Clear();
        }

        protected void Given(params Tuple<Type, IEvent>[] existingEvents)
        {
            _preConditions = existingEvents.ToList();

        }

        protected void WhenProcessed<TEvent>(TEvent command)
            where TEvent : IEvent
        {
            var handler = BuildApplication();
            handler.HandleEvent<TEvent, TProcessManager>(command);
        }

        protected void Then(params ICommand[] expectedCommands)
        {
            var latestCommands = _commandQueue.LatestCommands;
            var expectedCommandsList = expectedCommands != null
                ? expectedCommands.ToList()
                : new List<ICommand>();

            Assert.Equal(expectedCommandsList.Count, latestCommands.Count());

            var latestAndExpected = latestCommands.Zip(expectedCommandsList, (l, e) => new { L = l, E = e });

            foreach (var le in latestAndExpected)
            {
                Assert.True(ObjectsAreEqual(le.L, le.E));
            }
        }

        protected void Then(params IEvent[] expectedEvents)
        {
            var latestEvents = _eventStore.LatestEvents.ToList();
            var expectedEventsList = expectedEvents != null
                ? expectedEvents.ToList()
                : new List<IEvent>();

            Assert.Equal(latestEvents.Count, expectedEventsList.Count);

            var latestAndExpected = latestEvents
                .Zip(expectedEventsList, (l, e) => new { L = l, E = e });

            foreach (var le in latestAndExpected)
            {
                Assert.True(ObjectsAreEqual(le.L, le.E));
            }
        }

        private bool ObjectsAreEqual(object evt1, object evt2)
        {
            var evtId = Guid.NewGuid();
            var now = DateTime.Now;

            // copy events to compare
            var obj1 = (dynamic)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(evt1));
            var obj2 = (dynamic)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(evt2));

            // since we're creating expected products manually, these won't ever match
            obj1.Metadata.Date = null;
            obj2.Metadata.Date = null;

            // this guid is created automatically, and we're comparing different objects, these will never match
            obj1.Metadata.EventId = null;
            obj2.Metadata.EventId = null;
            obj1.Metadata.CommandId = null;
            obj2.Metadata.CommandId = null;

            var json1 = JsonConvert.SerializeObject(obj1);
            var json2 = JsonConvert.SerializeObject(obj2);

            return json1 == json2;
        }

        protected List<Tuple<Type, IEvent>> _initialEvents = new List<Tuple<Type, IEvent>>();

        protected Guid AddPreviousEvent<TAggregate>(IEvent evt)
        {
            _initialEvents.Add(new Tuple<Type, IEvent>(typeof(TAggregate), evt));
            return evt.Metadata.ProcessId;
        }

        protected Guid AddProcessedEvent<TAggregate>(IEvent evt, Guid? processId = null)
        {
            if (processId == null) processId = evt.Metadata.ProcessId;

            AddPreviousEvent<TAggregate>(evt);
            _initialEvents.Add(new Tuple<Type, IEvent>(typeof(TProcessManager), new EventProcessed(processId.Value, evt)));

            return evt.Metadata.ProcessId;
        }
    }
}