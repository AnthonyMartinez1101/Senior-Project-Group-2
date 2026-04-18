using UnityEngine;
using System.Collections.Generic;

public class ElectricEnemy : MonoBehaviour
{
    [System.Serializable]
    private class LinkEntry
    {
        public ElectricEnemy other;
        public ElectricLink link;
    }
    [SerializeField] private ElectricSensor sensor;

    [SerializeField] private int maxLinks = 3;
    [SerializeField] private float refreshInt = 0.25f;

    [SerializeField] private ElectricLink linkPrefab;

    // List of current links
    [SerializeField] private List<LinkEntry> links = new List<LinkEntry>();

    private void Start()
    {
        InvokeRepeating(nameof(RefreshLinks), 0f, refreshInt);
    }

    private void OnDisable()
    {
        CancelInvoke();
        for (int i = links.Count - 1; i >= 0; i--)
        {
            if(links[i].link != null)
                Destroy(links[i].link.gameObject);
        }
        links.Clear();
    }

    private void RefreshLinks()
    {
        CleanupBrokenLinks();
        if(sensor == null) return;
        if(links.Count >= maxLinks) return;
        for(int i = 0; i < sensor.Nearby.Count && links.Count < maxLinks; i++)
        {
            ElectricEnemy other = sensor.Nearby[i];
            if(other == null) continue;
            if(HasLinkTo(other)) continue;
            if(other.links.Count >= other.maxLinks) continue;
            if(GetInstanceID() > other.GetInstanceID()) continue; // Avoid duplicate links
            ElectricLink link = Instantiate(linkPrefab);
            link.Init(transform, other.transform);
            links.Add(new LinkEntry { other = other, link = link });
            other.links.Add(new LinkEntry { other = this, link = link });
        }
    }

    private void CleanupBrokenLinks()
    {
        for (int i = links.Count - 1; i >= 0; i--)
        {
            var e = links[i];
            if(e == null || e.other == null || e.link == null)
            {
                links.RemoveAt(i);
                continue;
            }
        }
    }

    private bool HasLinkTo(ElectricEnemy other)
    {
        for (int i = 0; i < links.Count; i++)
        {
            if (links[i].other == other) return true;
        }
        return false;
    }
}