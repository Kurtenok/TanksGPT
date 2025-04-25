using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBarFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        transform.position = target.position + offset;
        transform.rotation = initialRotation;
    }
}
