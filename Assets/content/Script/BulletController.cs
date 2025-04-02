using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody bulletRb;
    public float bulletPower = 10f;
    public float lifeTime = 4f;
    public GameObject bulletEffectPrefab; // Prefab del efecto de part�culas para la bala

    void Start()
    {
        bulletRb = GetComponent<Rigidbody>();
        bulletRb.velocity = transform.forward * bulletPower;

        // Generar efecto de part�culas al disparar
        if (bulletEffectPrefab != null)
        {
            GameObject bulletEffect = Instantiate(bulletEffectPrefab, transform.position, Quaternion.identity);
            Destroy(bulletEffect, 0.5f); // Destruir el efecto despu�s de 2 segundos
        }

        // Destruir la bala autom�ticamente despu�s del tiempo de vida
        StartCoroutine(DestroyBullet());
    }

    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(lifeTime);
        DestroyBulletNow();
    }

    private void OnCollisionEnter(Collision collision)
    {
        DestroyBulletNow();
    }

    private void DestroyBulletNow()
    {
        // Notifica al WeaponController que la bala ha sido destruida
        GameObject weapon = FindObjectOfType<WeaponController>()?.gameObject;
        if (weapon != null)
        {
            weapon.GetComponent<WeaponController>().BulletDestroyed();
        }

        Destroy(gameObject);
    }
}
