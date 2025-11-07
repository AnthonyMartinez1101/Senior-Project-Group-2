using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace WorldTime
{
    public class WorldLight : MonoBehaviour
    {
        private Light2D _light;

        [SerializeField] private WorldTime _worldTime;
        [SerializeField] private Gradient _gradient;

        private void Awake()
        {
            _light = GetComponent<Light2D>();
            _worldTime.WorldTimeChanged += OnWorldTimeChanged;
        }

        private void OnDestroy()
        {
            _worldTime.WorldTimeChanged -= OnWorldTimeChanged;
        }

        private void OnWorldTimeChanged(object sender, System.TimeSpan _)
        {
            _light.color = _gradient.Evaluate(_worldTime.NormalizedCycle);
        }
    }
}
