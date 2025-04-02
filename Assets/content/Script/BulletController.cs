using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody bulletRb;
    public float bulletPower = 10f;
    public float lifeTime = 4f;

    void Start()
    {
        bulletRb = GetComponent<Rigidbody>();
        bulletRb.velocity = transform.forward * bulletPower;

        // Destruir la bala automáticamente después del tiempo de vida
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
