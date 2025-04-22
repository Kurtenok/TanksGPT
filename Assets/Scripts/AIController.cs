using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform turretTransform;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float BulletSpeed;
    [SerializeField] private float BulletDamage;
    [SerializeField] private float reloadTime=3;
    private float remainReloadingTime=0;

    private NavMeshAgent agent;

    private Transform player;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player= GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(remainReloadingTime>0)
        {
            remainReloadingTime-=Time.deltaTime;
        }
        if(player)
        {
            RotateTurret(player.position);
            Shoot();
        }
    }

    private void RotateTurret(Vector2 WorldPosition)
    {
        turretTransform.up=WorldPosition-(Vector2)turretTransform.position;
    }
    private void MoveTo(Vector2 WorldPosition)
    {
        if(!agent)
        return;
        
        agent.SetDestination(WorldPosition);
    }

     
    private void Shoot()
    {
        if(remainReloadingTime>0)
        return;

        GameObject bul = Instantiate(Bullet, muzzle.position, muzzle.rotation);
        Rigidbody2D rb=bul.GetComponent<Rigidbody2D>();
        rb.AddForce(bul.transform.up*BulletSpeed,ForceMode2D.Impulse);
        remainReloadingTime=reloadTime;
    }
}
