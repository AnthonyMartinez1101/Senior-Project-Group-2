using UnityEngine;
using UnityEngine.UI;

public class FloatingHealth : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private CanvasGroup canvasGroup; // used to fade/hide UI without disabling it
    private Camera cam;

    // Optional: tiny threshold to avoid float wobble at exactly 1.0
    private const float FullEpsilon = 0.0001f;

    void Awake()
    {
        if (!slider) slider = GetComponentInChildren<Slider>(true);
        if (!canvasGroup)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        if (!cam) cam = Camera.main;

        RefreshVisibility();
    }

    void Update()
    {
        transform.rotation = cam.transform.rotation;
        transform.position = target.position + offset;
    }

    public void UpdateHealth(float currentVal, float maxVal)
    {
        float norm = (maxVal > 0f) ? Mathf.Clamp01(currentVal / maxVal) : 0f;
        slider.value = norm;
        RefreshVisibility();
    }

    public void SetMax()
    {
        slider.value = 1f;
        RefreshVisibility();
    }

    private void RefreshVisibility()
    {
        bool isFull = (1f - slider.value) <= FullEpsilon;
        if (isFull)
        {
            canvasGroup.alpha = 0f; // hide
        }
        else
        {
            canvasGroup.alpha = 1f; // show
        }


    }
}
