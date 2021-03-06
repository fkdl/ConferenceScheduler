﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer
{
    internal static class SessionExtensions
    {
        internal static IEnumerable<int> GetSessionIds(this IEnumerable<Session> sessions)
        {
            return sessions.Select(s => s.Id).Distinct().ToList();
        }

    }
}
