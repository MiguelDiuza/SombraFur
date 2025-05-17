using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody bulletRb;
    public float bulletPower = 10f;
    public float lifeTime = 4f;

    public GameObject bulletEffectPrefab;     // Efecto al disparar la bala
    // Ya no necesitamos estas referencias aquí, el EnemyHitHandler las tiene
    // public GameObject bloodImpactEffect;       // Efecto de sangre (impacto)
    // public GameObject bloodSprayEffect;         // Efecto de sangre (chorrito)

    private bool hasHitEnemy = false; // Nueva variable para controlar si ya golpeó al enemigo

    void Start()
    {
        bulletRb = GetComponent<Rigidbody>();
        bulletRb.velocity = transform.forward * bulletPower;

        // Efecto visual al salir la bala
        if (bulletEffectPrefab != null)
        {
            GameObject bulletEffect = Instantiate(bulletEffectPrefab, transform.position, Quaternion.identity);
            Destroy(bulletEffect, 0.5f);
        }

        // Destruir la bala después de un tiempo
        StartCoroutine(DestroyBullet());
    }

    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(lifeTime);
        DestroyBulletNow();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy") && !hasHitEnemy)
        {
            hasHitEnemy = true; // Marca que ya golpeó al enemigo
            EnemyHitHandler hitHandler = other.GetComponent<EnemyHitHandler>();
            if (hitHandler != null)
            {
                // Calculamos el punto de impacto más cercano
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = -transform.forward;

                hitHandler.HandleHit(hitPoint, hitNormal);

                // ¡No destruyas la bala aquí inmediatamente!
                // DestroyBulletNow();
            }
            else
            {
                // Si no hay EnemyHitHandler, destruye la bala
                DestroyBulletNow();
            }
        }
        // Si golpea algo más que no es el enemigo, destruye la bala inmediatamente
        else if (!other.CompareTag("Player") && !other.CompareTag("Untagged") && !other.CompareTag("enemy"))
        {
            DestroyBulletNow();
        }
    }

    private void DestroyBulletNow()
    {
        GameObject weapon = FindObjectOfType<WeaponController>()?.gameObject;
        if (weapon != null)
        {
            weapon.GetComponent<WeaponController>().BulletDestroyed();
        }

        Destroy(gameObject);
    }
}