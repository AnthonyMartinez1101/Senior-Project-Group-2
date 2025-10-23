using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    public GameObject Melee;

    InputAction sytheAction;

    public Transform aim;
    public GameObject bullet;
    public float bulletForce = 10f;
    public float shootCooldown = 0.25f;
    float shootTimer = 0.5f;

    public bool noShootCooldown = false;

    //The script which is in the Melee child object
    private SlashMove slashMove;

    void Start()
    {
        sytheAction = InputSystem.actions.FindAction("Sythe");

        slashMove = Melee.GetComponent<SlashMove>();
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;

        if(sytheAction.WasPressedThisFrame())
        {
            OnMelee();
        }
    }

    public void OnShoot()
    {
        if(shootTimer > shootCooldown || noShootCooldown)
        {
            shootTimer = 0.0f;
            Quaternion rot = aim.rotation * Quaternion.Euler(0f, 0f, -90f);
            GameObject b = Instantiate(bullet, aim.position, rot);
            b.GetComponent<Rigidbody2D>().AddForce(-aim.up * bulletForce, ForceMode2D.Impulse);
            Destroy(b, 2.0f);
        }
        else
        {
            shootTimer += Time.deltaTime;
        }
    }

    private void OnMelee()
    {
        if(!slashMove.IsSlashing())
        {
            slashMove.ActivateSlash();
        }
    }
}
