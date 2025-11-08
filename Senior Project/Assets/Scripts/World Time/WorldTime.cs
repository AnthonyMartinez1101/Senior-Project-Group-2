using System;
using UnityEngine;

namespace WorldTime
{
    public enum Phase { Day, Night }

    public class WorldTime : MonoBehaviour
    {
        public event EventHandler<TimeSpan> WorldTimeChanged; // sends DISPLAY time remaining

        [Header("Phase display lengths (seconds)")]
        [SerializeField] private float dayLength = 180f;   // 3:00 shown
        [SerializeField] private float nightLength = 120f; // 2:00 shown

        [Header("Hold at 0:00 after phase switch (seconds)")]
        [SerializeField] private float transitionLength = 5f;    // stays showing 0:00

        public Phase CurrentPhase { get; private set; } = Phase.Day;

        // Display timer (what you show on screen)
        private float _displayRemaining;   // counts down to 0, then holds at 0 for transitionLength
        private float _holdRemaining;      // while >0, display stays 0:00

        // Actual phase/cycle timing (gameplay + lighting)
        private float _elapsedInPhase;     // real time in the current phase (includes hold at start)
        private float _elapsedInCycle;     // for 0..1 cycle percent

        private float ActualDayPhase => transitionLength + dayLength;   // day lasts 3:05 in gameplay
        private float ActualNightPhase => transitionLength + nightLength; // night lasts 2:05 in gameplay
        private float ActualCycle => ActualDayPhase + ActualNightPhase; // 310s total

        // 0->1 over the full (actual) cycle. Day occupies [0, ActualDayPhase/ActualCycle)
        public float NormalizedCycle => (_elapsedInCycle % ActualCycle) / ActualCycle;

        private void Start()
        {
            // Start of game: Day is active immediately, display starts at 3:00
            CurrentPhase = Phase.Day;
            _displayRemaining = dayLength;
            _holdRemaining = 0f;
            _elapsedInPhase = 0f;
            _elapsedInCycle = 0f;
            RaiseDisplay();
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            // --- Actual phase/cycle timing (gameplay & lighting) ---
            _elapsedInPhase += dt;
            _elapsedInCycle += dt;

            // Flip phase immediately when the *actual* phase length elapses
            float actualPhaseLen = (CurrentPhase == Phase.Day) ? ActualDayPhase : ActualNightPhase;
            if (_elapsedInPhase >= actualPhaseLen)
            {
                _elapsedInPhase -= actualPhaseLen; // carry overshoot safely
                CurrentPhase = (CurrentPhase == Phase.Day) ? Phase.Night : Phase.Day;

                // Start-of-phase display will hold at 0:00 for transitionLength,
                // but only after the previous countdown reaches 0.
                // If we happen to phase-flip while already in a hold (edge case),
                // ensure we’re holding for the new phase too:
                if (_displayRemaining <= 0f)
                    _holdRemaining = transitionLength;
            }

            // --- Display logic (what the UI shows) ---
            if (_displayRemaining > 0f)
            {
                _displayRemaining = Mathf.Max(0f, _displayRemaining - dt);
                if (_displayRemaining == 0f)
                {
                    // Countdown just hit 0  switch display to hold (phase was already flipped above if needed)
                    _holdRemaining = transitionLength;

                    // IMPORTANT: Phase change must be immediate at 0:00.
                    // If we reached 0 before the actual phase flip (rare), force it now:
                    // (This can happen if you change lengths at runtime.)
                    // No-op in normal flow because actualPhaseLen gate already flipped.
                }
            }
            else
            {
                // We are holding at 0:00
                if (_holdRemaining > 0f)
                {
                    _holdRemaining -= dt;
                    if (_holdRemaining <= 0f)
                    {
                        // Jump display to the next phase full length
                        _displayRemaining = (CurrentPhase == Phase.Day) ? dayLength : nightLength;
                        _holdRemaining = 0f;
                    }
                }
            }

            RaiseDisplay();
        }

        private void RaiseDisplay()
        {
            // While holding: show 0:00 for the whole hold
            double shownSeconds = (_holdRemaining > 0f) ? 0d : Math.Ceiling(_displayRemaining);
            if (shownSeconds < 0) shownSeconds = 0;
            WorldTimeChanged?.Invoke(this, TimeSpan.FromSeconds(shownSeconds));
        }
    }
}
