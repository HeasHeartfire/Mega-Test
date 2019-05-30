using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    public float bulletSpeed = 0.5f;
    private float destructTime = 0f;
    private Rigidbody2D r2d;
    private Animator anim;

    private void Start()
    {
        r2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ShootBullet();
    }

    public void ShootBullet()
    {
        if(r2d != null)
        {
            r2d.velocity = new Vector2(transform.localScale.x * bulletSpeed, 0f);
            destructTime = Time.time + 2f;
        }
        
    }

    private void Update()
    {
        if(destructTime > 0)
        {
            if (destructTime < Time.time)
            {
                Destroy(gameObject);
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        anim.SetTrigger("Hit");
        r2d.velocity = Vector2.zero;
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
