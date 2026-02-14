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

    InputAction scytheAction;

    public Transform aim;
    public GameObject shootFlash;
    public GameObject pistolBullet;
    public GameObject ARBullet;
    public GameObject shotgunBullet;
    public float bulletForce = 10f;
    public float bulletYOffset = 0.0f;
    float shootTimer = 0.5f;

    public GameObject grenade;
    public float throwForce = 10f;
    public float airTime = 0.5f;
    public float explosionTimer = 2f;

    public bool noShootCooldown = false;

    private PlayerAudio playerAudio;

    public float shotgunSpreadAngle = 15f;


    //The script which is in the Melee child object
    private SlashMove slashMove;

    void Start()
    {
        scytheAction = InputSystem.actions.FindAction("Scythe");

        slashMove = Melee.GetComponent<SlashMove>();

        playerAudio = GetComponent<PlayerAudio>();
    }

    // Update is called once per frame
    void Update()
    {
        meleeTimer -= Time.deltaTime;
        shootTimer -= Time.deltaTime;
        if (scytheAction.WasPressedThisFrame())
        {
            OnMelee();
        }
    }

    public void OnShoot()
    {
        shootTimer = 0.25f;
        Shoot(pistolBullet);
    }

    public void OnShootBurst()
    {
        StartCoroutine(ShootBurst());
    }

    public void OnShootShotgun()
    {
        shootTimer = 1f;
        ShootShotgun();
    }

    IEnumerator ShootBurst()
    {
        shootTimer = 0.75f;
        for (int i = 0; i < 3; i++)
        {
            Shoot(ARBullet);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ShootShotgun()
    {
        Quaternion baseRot = aim.rotation * Quaternion.Euler(0f, 0f, -90f);
        Vector3 bulletSpawn = aim.position + new Vector3(0f, bulletYOffset, 0f);


        for (int i = -1; i <= 1; i++)
        {
            Quaternion rot = baseRot * Quaternion.Euler(0f, 0f, i * shotgunSpreadAngle);
            GameObject b = Instantiate(shotgunBullet, bulletSpawn, rot);
            Instantiate(shootFlash, bulletSpawn, rot);

            Vector2 dir = -((aim.rotation * Quaternion.Euler(0f, 0f, i * shotgunSpreadAngle)) * Vector3.up);
            b.GetComponent<Rigidbody2D>().AddForce(dir * bulletForce, ForceMode2D.Impulse);
            Destroy(b, 2.0f);
        }
        GameManager.Instance.CameraShake(1.5f, 0.1f);
    }

    private void Shoot(GameObject bullet)
    {
        Quaternion rot = aim.rotation * Quaternion.Euler(0f, 0f, -90f);
        Vector3 bulletSpawn = aim.position + new Vector3(0f, bulletYOffset, 0f);
        GameObject b = Instantiate(bullet, bulletSpawn, rot);
        Instantiate(shootFlash, bulletSpawn, rot);
        b.GetComponent<Rigidbody2D>().AddForce(-aim.up * bulletForce, ForceMode2D.Impulse);
        GameManager.Instance.CameraShake(1f, 0.1f);
        Destroy(b, 2.0f);
    }

    public bool CanShoot()
    {
        return shootTimer <= 0 || noShootCooldown;
    }

    public void OnThrowGrenade(float force)
    {
        Quaternion rot = aim.rotation * Quaternion.Euler(0f, 0f, -90f);
        GameObject g = Instantiate(grenade, aim.position, rot);
        Rigidbody2D rb = g.GetComponent<Rigidbody2D>();
        rb.AddForce(-aim.up * force * 20, ForceMode2D.Impulse);
        StartCoroutine(FlyingGrenade(rb));
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
            playerAudio.PlayScytheSwing();
        }
    }

    public bool IsMeleeing()
    {
        return currentMelee != null;
    }
}
