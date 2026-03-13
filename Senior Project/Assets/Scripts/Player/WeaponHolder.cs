using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHolder : MonoBehaviour
{
    //The 3 children of weapon holder
    private GameObject[] weaponHolders;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get the 3 children of weapon holder
        weaponHolders = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            weaponHolders[i] = transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Move transform rotation to mouse position
        if(Mouse.current != null && Camera.main != null)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            if(float.IsNaN(mousePosition.x) && float.IsNaN(mousePosition.y) || float.IsInfinity(mousePosition.x) || float.IsInfinity(mousePosition.y))
            {
                return; // Skip if mouse position is invalid
            }

            Vector3 world = Camera.main.ScreenToWorldPoint(mousePosition);
            world.z = transform.position.z;

            //Sets the aim's up direction to point towards the mouse position
            Vector2 dir = (world - transform.position).normalized;
            transform.up = -dir;

            bool isLeft = world.x < transform.parent.position.x;

            Vector3 localPos = transform.localPosition;
            localPos.x = isLeft ? -Mathf.Abs(localPos.x) : Mathf.Abs(localPos.x);
            transform.localPosition = localPos;

            Vector3 localScale = transform.localScale;
            localScale.x = isLeft ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }
    }

    public void SetWeaponHolder(int weaponIndex)
    {
        //Set the active state of the weapon holders based on the selected weaponIndex
        for (int i = 0; i < weaponHolders.Length; i++)
        {
            weaponHolders[i].SetActive(i == weaponIndex);
        }
    }

    public Transform GetHolderTransform()
    {
        //Return the transform of the active weapon holder
        for (int i = 0; i < weaponHolders.Length; i++)
        {
            if (weaponHolders[i].activeSelf)
            {
                var holderData = weaponHolders[i].GetComponent<GunHolderData>();
                return holderData.muzzlePosition;
            }
        }
        return transform; // Return the transform of the weapon holder parent itself if no active weapon holder is found
    }
}
