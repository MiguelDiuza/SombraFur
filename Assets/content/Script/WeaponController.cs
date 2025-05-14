using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Transform shootSpawn;
    public GameObject bulletPrefab;
    private GameObject currentBullet;
    private float fireRate = 0.5f;
    private float nextFireTime = 0f;

    private Inventario inventory;

    void Start()
    {
        inventory = GameObject.FindWithTag("Player").GetComponent<Inventario>();
        if (inventory == null)
        {
            Debug.LogWarning("Inventario no encontrado en el jugador.");
        }
    }

    void Update()
    {
        Debug.DrawLine(shootSpawn.position, shootSpawn.position + shootSpawn.forward * 10f, Color.red);
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 10f, Color.blue);

        RaycastHit cameraHit;

        // Calcula la dirección del disparo con raycast desde la cámara
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out cameraHit))
        {
            Vector3 shootDirection = cameraHit.point - shootSpawn.position;
            shootSpawn.rotation = Quaternion.LookRotation(shootDirection);
        }
        else
        {
            shootSpawn.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }

        // Disparo: solo si no hay una bala activa, se respeta el fireRate, y hay balas en el inventario
        if (Input.GetKey(KeyCode.Mouse0) && currentBullet == null && Time.time >= nextFireTime)
        {
            if (inventory != null && inventory.UseBullet())
            {
                nextFireTime = Time.time + fireRate;
                Shoot();
            }
            else
            {
                Debug.Log("No hay balas en el inventario");
            }
        }
    }

    void Shoot()
    {
        currentBullet = Instantiate(bulletPrefab, shootSpawn.position, shootSpawn.rotation);
    }

    public void BulletDestroyed()
    {
        currentBullet = null;
    }
}
