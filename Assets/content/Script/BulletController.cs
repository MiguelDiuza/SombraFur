using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    Rigidbody bulletRb;

    public float bulletPower = 0f;
    public float lifeTime = 4f;

    private float time = 0f;


    // Start is called before the first frame update
    void Start()
    {
        bulletRb = GetComponent<Rigidbody>();

        bulletRb.velocity = this.transform.forward * bulletPower;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time > lifeTime)
        {
            Destroy(this.gameObject);
        }
    }
}
