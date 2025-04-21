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

    private NavMeshAgent agent;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RotateTurret(Vector2 WorldPosition)
    {
        turretTransform.up=WorldPosition-(Vector2)turretTransform.position;
    }
    private void MoveTo(Vector2 WorldPosition)
    {
        //agent=
    }
}
