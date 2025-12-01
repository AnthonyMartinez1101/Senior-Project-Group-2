using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    public GameObject Melee;
    private GameObject currentMelee;
    public float meleeCooldown = 0.25f;
    private float meleeTimer = 0.25f;

    InputAction sytheAction;

    public Transform aim;
    public GameObject shootFlash;
    public GameObject bullet;
    public float bulletForce = 10f;
    public float bulletYOffset = 0.0f;
    public float shootCooldown = 0.25f;
    float shootTimer = 0.5f;

    public GameObject grenade;
    public float throwForce = 10f;
    public float throwCooldown = 0.5f;
    float throwTimer = 1f;
    public float airTime = 0.5f;
    public float explosionTimer = 2f;

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
        meleeTimer -= Time.deltaTime;
        shootTimer += Time.deltaTime;
        throwTimer += Time.deltaTime;

        if (sytheAction.WasPressedThisFrame())
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
            Vector3 bulletSpawn = aim.position + new Vector3(0f, bulletYOffset, 0f);
            GameObject b = Instantiate(bullet, bulletSpawn, rot);
            Instantiate(shootFlash, bulletSpawn, rot);
            b.GetComponent<Rigidbody2D>().AddForce(-aim.up * bulletForce, ForceMode2D.Impulse);
            GameManager.Instance.CameraShake(1f, 0.1f);
            Destroy(b, 2.0f);
        }
        else
        {
            shootTimer += Time.deltaTime;
        }
    }

    public void OnThrowGrenade()
    {
        // planting grenade for now
        if (throwTimer > throwCooldown || noShootCooldown)
        {
            throwTimer = 0.0f;
            Quaternion rot = aim.rotation * Quaternion.Euler(0f, 0f, -90f);
            GameObject g = Instantiate(grenade, aim.position, rot);
            Rigidbody2D rb = g.GetComponent<Rigidbody2D>();
            rb.AddForce(-aim.up * throwForce, ForceMode2D.Impulse);
            StartCoroutine(FlyingGrenade(rb));
        }
        else
        {
            throwTimer += Time.deltaTime;
        }
    }

    private IEnumerator FlyingGrenade(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(airTime);
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnMelee()
    {
        if(currentMelee == null && meleeTimer < 0)
        {
            currentMelee = Instantiate(Melee, aim.position, aim.rotation);
            meleeTimer = meleeCooldown;
        }
    }

    public bool IsMeleeing()
    {
        return currentMelee != null;
    }
}
