using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private InteractScript interactScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactScript = GetComponentInParent<InteractScript>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var soil = other.GetComponent<SoilScript>();
        if (soil != null) interactScript.SetSoil(soil);
    }
}
