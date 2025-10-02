using System;
using System.Collections;
using UnityEngine;

namespace WorldTime
{
    public class WorldTime : MonoBehaviour
    {
        public event EventHandler<TimeSpan> WorldTimeChanged;

        [SerializeField]
        private float _dayLength; // Variable to set the length of a day in seconds
        private TimeSpan _currentTime; // Current time in the game world
        private float _minuteLength => _dayLength / WorldTimeConstant.MinutesInDay; // Length of a minute in seconds

        private void Start()
        {
            StartCoroutine(AddMinute()); // Start the coroutine to add minutes
        }

        private IEnumerator AddMinute()
        {
            _currentTime += TimeSpan.FromMinutes(1); // Increment the current time by one minute
            WorldTimeChanged?.Invoke(this, _currentTime); // Trigger the event to notify listeners of the time change
            yield return new WaitForSeconds(_minuteLength); // Wait for the length of a minute
            StartCoroutine(AddMinute()); // Repeat the process
        }
    }
}


