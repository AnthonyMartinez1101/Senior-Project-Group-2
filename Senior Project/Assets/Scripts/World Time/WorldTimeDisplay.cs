using TMPro;
using UnityEngine;

namespace WorldTime
{
    [RequireComponent(typeof(TMP_Text))]

    public class WorldTimeDisplay : MonoBehaviour
    {
        [SerializeField]
        private WorldTime _worldTime;

        private TMP_Text _text;

        private void Awake() // Use Awake to ensure TMP_Text is initialized before subscribing to events
        {
            _text = GetComponent<TMP_Text>();
            _worldTime.WorldTimeChanged += OnWorldTimeChanged;
        }

        private void OnDestroy()
        {
            _worldTime.WorldTimeChanged -= OnWorldTimeChanged;
        }

        private void OnWorldTimeChanged(object sender, System.TimeSpan newTime) // Event handler to update text based on time
        {
            _text.SetText(newTime.ToString(@"hh\:mm"));
        }
    }
}