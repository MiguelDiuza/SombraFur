using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Transform shootSpawn;
    public GameObject bulletPrefab;
    private GameObject currentBullet; // Referencia a la bala actual
    private float fireRate = 0.5f; // Tiempo entre disparos (en segundos)
    private float nextFireTime = 0f;

    void Update()
    {
        Debug.DrawLine(shootSpawn.position, shootSpawn.position + shootSpawn.forward * 10f, Color.red);
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 10f, Color.blue);

        RaycastHit cameraHit;

        // Si el Raycast golpea un objeto, ajustamos la rotación del disparo
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out cameraHit))
        {
            Vector3 shootDirection = cameraHit.point - shootSpawn.position;
            shootSpawn.rotation = Quaternion.LookRotation(shootDirection);
        }
        else
        {
            // Si el Raycast no golpea nada, se usa la dirección de la cámara
            shootSpawn.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }

        // Disparar con clic izquierdo solo si no hay una bala en escena y respetando el fireRate
        if (Input.GetKey(KeyCode.Mouse0) && currentBullet == null && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate; // Resetea el tiempo para el próximo disparo
            Shoot();
        }
    }

    public void Shoot()
    {
        currentBullet = Instantiate(bulletPrefab, shootSpawn.position, shootSpawn.rotation);
    }

    public void BulletDestroyed()
    {
        currentBullet = null; // Se libera la referencia cuando la bala desaparece
    }
}
