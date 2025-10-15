using UnityEngine;

public class SlashMove : MonoBehaviour
{
    private GameObject slashCollider;
    private Transform pointA;
    private Transform pointB;

    private bool slash;


    public float moveSpeed = 1f;

    void Awake()
    {
        //Assigns the objects
        slashCollider = transform.Find("Slash").gameObject;
        pointA = transform.Find("PointA");
        pointB = transform.Find("PointB");

        slashCollider.SetActive(false);

        slash = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(slash)
        {
            //Moves the collider from point A to point B
            slashCollider.transform.position = Vector3.MoveTowards(slashCollider.transform.position, pointB.position, moveSpeed * Time.deltaTime);

            //Once collider reaches point B, disable it and stop slashing
            if (slashCollider.transform.position == pointB.position)
            {
                slashCollider.SetActive(false);
                slash = false;
            }
        }
    }

    public void ActivateSlash()
    {
        //Activates the collider and sets its position to point A
        slashCollider.SetActive(true);
        slashCollider.transform.position = pointA.position;
        slash = true;
    }

    public bool IsSlashing() { return slash; }
}
