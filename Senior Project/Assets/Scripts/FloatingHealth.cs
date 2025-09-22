using UnityEngine;
using UnityEngine.UI;

public class FloatingHealth : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    public void UpdateHealth(float currentVal, float maxVal)
    {
        slider.value = currentVal / maxVal;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = cam.transform.rotation;
        transform.position = target.position + offset;
    }
}
