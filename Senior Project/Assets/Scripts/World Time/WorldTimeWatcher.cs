using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

namespace WorldTime
{
    public class WorldTimeWatcher : MonoBehaviour
    {
        [SerializeField]
        private WorldTime _worldTime;

        [SerializeField]
        private List<Schedule> _schedule;

        private void Start() // Subscribe to the WorldTimeChanged event
        {
            _worldTime.WorldTimeChanged += CheckSchedule;
        }

        private void OnDestroy() // Unsubscribe from the WorldTimeChanged event
        {
            _worldTime.WorldTimeChanged -= CheckSchedule;
        }

        private void CheckSchedule(object sender, System.TimeSpan newTime) // Check if there are any scheduled actions for the current time
        {
            var schedule = _schedule.FirstOrDefault(s => s.Hour == newTime.Hours && s.Minute == newTime.Minutes);
            schedule?._action?.Invoke();
        }

        [SerializeField]
        private class Schedule // Class to hold scheduled actions
        {
            public int Hour;
            public int Minute;
            public UnityEvent _action;
        }
    }
}