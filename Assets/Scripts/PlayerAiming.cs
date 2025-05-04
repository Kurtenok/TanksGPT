using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAiming : MonoBehaviour
{
    [SerializeField] private Transform turretTransform;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float BulletSpeed = 10f;
    [SerializeField] private float BulletDamage = 80f;
    [SerializeField] private float reloadTime = 2f;

    private float remainReloadingTime = 0f;

    void Update()
    {
        if (turretTransform)
        {
            Vector2 worldAimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            turretTransform.up = worldAimPos - (Vector2)turretTransform.position;
        }

        if (remainReloadingTime > 0)
        {
            remainReloadingTime -= Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0) && remainReloadingTime <= 0)
        {
            if (Bullet)
            {
                GameObject bul = Instantiate(Bullet, muzzle.position, muzzle.rotation);
                Rigidbody2D rb = bul.GetComponent<Rigidbody2D>();
                rb.AddForce(bul.transform.up * BulletSpeed, ForceMode2D.Impulse);
                remainReloadingTime = reloadTime;
            }
        }
    }

    public float GetDamage() => BulletDamage;
    public float GetReloadTime() => reloadTime;
    public float GetRemainingReload() => Mathf.Max(0f, remainReloadingTime);
}
