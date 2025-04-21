using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAiming : MonoBehaviour
{
    [SerializeField] private Transform turretTransform;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float BulletSpeed;
    [SerializeField] private float BulletDamage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(turretTransform)
        {
            Vector2 WorldAimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            turretTransform.up=WorldAimPos-(Vector2)turretTransform.position;
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(Bullet)
            {
                GameObject bul = Instantiate(Bullet, muzzle.position, muzzle.rotation);
                Rigidbody2D rb=bul.GetComponent<Rigidbody2D>();
                rb.AddForce(bul.transform.up*BulletSpeed,ForceMode2D.Impulse);
            }
        }
    }
}
