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
        agent.updateRotation=false;
        agent.updateUpAxis=false;
        player= GameObject.FindGameObjectWithTag("Player").transform;

        agent.SetDestination(player.position);
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
        /*if (agent.velocity.sqrMagnitude > 0.01f) // Если агент двигается
        {
            float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle); // Поворот по Z в 2D
        }*/

        if (agent.velocity.sqrMagnitude > 0)
        {
            float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
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
