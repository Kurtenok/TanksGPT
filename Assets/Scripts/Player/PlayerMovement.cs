using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("References")]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] private float movementSpeed=4;
    [SerializeField] private float turningRate =80f;
    void Start()
    {
        if(!rb)
        {
            TryGetComponent<Rigidbody2D>(out rb);
        }
        if(!bodyTransform)
        {
            bodyTransform=GetComponent<Transform>();
        }
    }
    int xMovement=0;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.D))
        {
            float zRotation = -turningRate*1*Time.deltaTime;
            bodyTransform.Rotate(0f,0f,zRotation);
        }
        if(Input.GetKey(KeyCode.A))
        {
            float zRotation = -turningRate*-1*Time.deltaTime;
            bodyTransform.Rotate(0f,0f,zRotation);
        }
        xMovement=0;
        if(Input.GetKey(KeyCode.W))
        {
            xMovement=1;
        }
        if(Input.GetKey(KeyCode.S))
        {
            xMovement=-1;
        }
    }

    void FixedUpdate()
    {
        rb.velocity=(Vector2)bodyTransform.up * xMovement*movementSpeed;
    }
    public float GetSpeed() => movementSpeed;

}
