using UnityEngine;
using System.Collections.Generic;

public class ElectricSensor : MonoBehaviour
{
    public readonly List<ElectricEnemy> Nearby = new List<ElectricEnemy>();
    private ElectricEnemy owner;

    private void Awake()
    {
        owner = GetComponentInParent<ElectricEnemy>();

        Collider2D col = GetComponent<Collider2D>();
        if(col != null)
            col.excludeLayers = LayerMask.GetMask("Player");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ElectricEnemy linker = other.GetComponentInParent<ElectricEnemy>();
        if(linker == null) return;
        if(linker == owner) return;
        if(!Nearby.Contains(linker))
        {
            Nearby.Add(linker);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ElectricEnemy linker = other.GetComponentInParent<ElectricEnemy>();
        if(linker == null) return;
        Nearby.Remove(linker);
    }
}
