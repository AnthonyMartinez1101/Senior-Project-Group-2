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

        //Default offset which fixes the health bar position
        offset = new Vector3(0, -0.3f, 0);
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
