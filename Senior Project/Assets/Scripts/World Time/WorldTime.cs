using System;
using UnityEngine;

namespace WorldTime
{
    public enum Phase { Day, Night }

    public class WorldTime : MonoBehaviour
    {
        public event EventHandler<TimeSpan> WorldTimeChanged; // sends remaining time in current phase

        [Header("Phase Lengths (seconds)")]
        [SerializeField] private float _dayLengthSeconds = 180f;   // 3:00
        [SerializeField] private float _nightLengthSeconds = 120f; // 2:00

        public Phase CurrentPhase { get; private set; } = Phase.Day;

        private float _elapsedInPhase = 0f;
        private float _elapsedInCycle = 0f;
        private float TotalCycle => _dayLengthSeconds + _nightLengthSeconds;

        // 0->1 over the whole 5:00 cycle (Day: 0->0.6, Night: 0.6->1.0)
        public float NormalizedCycle => Mathf.Clamp01(_elapsedInCycle / TotalCycle);

        private void Update()
        {
            float dt = Time.deltaTime;
            _elapsedInPhase += dt;
            _elapsedInCycle += dt;

            float phaseLen = (CurrentPhase == Phase.Day) ? _dayLengthSeconds : _nightLengthSeconds;

            // swap phase when current one ends
            if (_elapsedInPhase >= phaseLen)
            {
                _elapsedInPhase -= phaseLen; // carry any overshoot
                CurrentPhase = (CurrentPhase == Phase.Day) ? Phase.Night : Phase.Day;
            }

            // wrap full cycle at 5:00
            if (_elapsedInCycle >= TotalCycle)
            {
                _elapsedInCycle -= TotalCycle;
            }

            // send remaining time in *current* phase (ceil so display hits 0 cleanly)
            double remaining = Math.Max(0, phaseLen - _elapsedInPhase);
            WorldTimeChanged?.Invoke(this, TimeSpan.FromSeconds(Math.Ceiling(remaining)));
        }
    }
}
