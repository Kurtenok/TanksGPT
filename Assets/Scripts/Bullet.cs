using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    public float Damage;
    void Start()
    {
        Invoke("DestroyBullet",2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DestroyBullet()
    {
        Destroy(this.gameObject);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        Health health;
        if(collision.gameObject.TryGetComponent<Health>(out health))
        {
            health.TakeDamage(Damage);
        }
        DestroyBullet();
    }
}
