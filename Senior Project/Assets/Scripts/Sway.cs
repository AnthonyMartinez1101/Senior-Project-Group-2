using UnityEngine;
using UnityEngine.UIElements;

public class Sway : MonoBehaviour
{
    [SerializeField] private float swaySpeed = 1f;
    [SerializeField] private float swayAmount = 1f;

    private Quaternion startRotation;
    private float offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startRotation = transform.localRotation;
        offset = Random.Range(0f, 100f);
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.Sin(Time.time * swaySpeed + offset) * swayAmount;
        transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, angle);
    }
}
