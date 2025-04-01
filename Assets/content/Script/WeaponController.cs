using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform shootSpawn;

    public bool shooting = false;

    public GameObject bulletPrefab;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(shootSpawn.position, shootSpawn.forward * 10f, Color.red);
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.forward * 10f, Color.red);

        RaycastHit cameraHit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out cameraHit))
        {
            Vector3 shootDirection = cameraHit.point - shootSpawn.position;
            shootSpawn.rotation = Quaternion.LookRotation(shootDirection);

            if (Input.GetKey(KeyCode.Mouse0)) 
            {
                Shoot();
            }
        }


    }

    public void Shoot()
    {
        Instantiate(bulletPrefab, shootSpawn.position, shootSpawn.rotation);
    }
}
