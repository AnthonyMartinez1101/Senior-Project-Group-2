using UnityEngine;
using UnityEngine.UI;

public class FloatingHealth : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    private Camera cam;

    void Awake()
    {
        if (!slider) slider = GetComponentInChildren<Slider>();
        if (!cam) cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = cam.transform.rotation;
        transform.position = target.position + offset;
    }
    public void UpdateHealth(float currentVal, float maxVal)
    {
        slider.value = currentVal / maxVal;
    }

    public void SetMax()
    {
        slider.value = 1;
    }
}
