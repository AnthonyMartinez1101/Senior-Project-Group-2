using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    public GameObject Melee;
    bool isAttacking = false;
    float atkDuration = 0.3f;
    float atkTimer = 0.0f;

    InputAction meleeAction;
    InputAction shootAction;

    public Transform aim;
    public GameObject bullet;
    public float bulletForce = 10f;
    float shootCooldown = 0.25f;
    float shootTimer = 0.5f;

    void Start()
    {
        meleeAction = InputSystem.actions.FindAction("Attack");
        shootAction = InputSystem.actions.FindAction("Attack 2");

        Melee.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        checkTimer();
        shootTimer += Time.deltaTime;

        if (meleeAction.WasPressedThisFrame())
        {
            OnAttack();
        }
        if(shootAction.WasPressedThisFrame())
        {
            OnShoot();
        }
    }

    void OnShoot()
    {
        if(shootTimer > shootCooldown)
        {
            shootTimer = 0.0f;
            GameObject b = Instantiate(bullet, aim.position, aim.rotation);
            b.GetComponent<Rigidbody2D>().AddForce(-aim.up * bulletForce, ForceMode2D.Impulse);
            Destroy(b, 2.0f);
        }
        else
        {
            shootTimer += Time.deltaTime;
        }
    }

    void OnAttack()
    {
        if(!isAttacking)
        {
            Melee.SetActive(true);
            isAttacking = true;
        }
    }

    void checkTimer()
    {
        if(isAttacking)
        {
            atkTimer += Time.deltaTime;
            if(atkTimer >= atkDuration)
            {
                Melee.SetActive(false);
                isAttacking = false;
                atkTimer = 0.0f;
            }
        }
    }
}
