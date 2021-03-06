﻿using System;

namespace PinetreeCQRS.Infrastructure.Events
{
    public interface IProcessEventHandler
    {
        void HandleEvent<TEvent, TProcessManager>(TEvent evt)
            where TEvent : IEvent
            where TProcessManager : IProcessManager, new();

        void RegisterHandler<TEvent, TProcessManager>(Func<TProcessManager, TEvent, TProcessManager> handler)
            where TEvent : class, IEvent
            where TProcessManager : IProcessManager;
    }
}