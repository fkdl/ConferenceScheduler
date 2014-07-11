﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer
{
    internal class PresenterAvailablityCollection: List<PresenterAvailability>
    {
        ICollection<int> _presenterIds;
        ICollection<int> _timeslotIds;

        public PresenterAvailablityCollection(IEnumerable<Presenter> presenters, IEnumerable<int> timeslotIds)
        {
            Load(presenters, timeslotIds);
        }

        public bool IsFeasible
        {
            get { return GetFeasibility(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public IEnumerable<int> GetAvailableTimeslotIds(IEnumerable<Presenter> presenters)
        {
            IEnumerable<int> availableSlots = _timeslotIds.ToList();
            foreach (var presenter in presenters)
                availableSlots = availableSlots.Intersect(GetAvailableTimeslotIds(presenter.Id));
            return availableSlots;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public IEnumerable<int> GetAvailableTimeslotIds(int presenterId)
        {
            return this.Where(pa => pa.PresenterId == presenterId && pa.IsAvailable).Select(pa => pa.TimeslotId).ToList();
        }

        private void Load(IEnumerable<Entities.Presenter> presenters, IEnumerable<int> timeslotIds)
        {
            _presenterIds = new List<int>();
            _timeslotIds = new List<int>();
            foreach (var presenter in presenters)
            {
                _presenterIds.Add(presenter.Id);
                foreach (var timeslotId in timeslotIds)
                {
                    _timeslotIds.Add(timeslotId);
                    this.Add(new PresenterAvailability() 
                    { 
                        PresenterId = presenter.Id,
                        TimeslotId = timeslotId,
                        IsAvailable = !presenter.IsUnavailableInTimeslot(timeslotId)
                    });
                }
            }
        }

        private bool GetFeasibility()
        {
            bool result = true;
            foreach (var presenterId in _presenterIds)
            {
                if (this.Count(a => a.PresenterId == presenterId && a.IsAvailable) == 0)
                    result = false;
            }
            return result;
        }

        internal void RemovePresentersFromSlots(Assignment assignment, Session session)
        {
            foreach (var presenter in session.Presenters)
	        {
                var items = this.Where(j => j.PresenterId == presenter.Id && j.TimeslotId == assignment.TimeslotId);
                foreach (var item in items)
                    item.IsAvailable = false;
	        }
        }
    }
}
