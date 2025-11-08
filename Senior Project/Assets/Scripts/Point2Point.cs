using UnityEngine;
using System.Collections;

public class Point2Point : MonoBehaviour
{
    public Transform point;
    public float speed = 2.0f;
    public float pause = 2.0f;
    public bool loop = true;

    private Transform[] points;
    private int currentPointIndex = 0;
    private bool isStopped;

    void Start()
    {
        points = new Transform[point.childCount];
        for (int i = 0; i < point.childCount; i++)
        {
            points[i] = point.GetChild(i);
        }
    }


    void Update()
    {
        MoveToNextPoint();
    }

    void MoveToNextPoint()
    {
        Transform target = points[currentPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if(Vector2.Distance(transform.position, target.position) < 0.1f)
        {
           StartCoroutine(PauseAtPoint());
        }
    }

    IEnumerator PauseAtPoint()
    {
        isStopped = true;
        yield return new WaitForSeconds(pause);

        currentPointIndex = loop ? (currentPointIndex + 1)% points.Length : Mathf.Min(currentPointIndex + 1, points.Length - 1);
        isStopped = false;
    }
}
