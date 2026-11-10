using System;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGravity : MonoBehaviour
{
    public static PlanetGravity instance { get; private set; }
    [SerializeField] private float gravityStrength = 15f;
    private readonly List<GravityObject> affectedObjects = new();


    private void Awake()
    {
        instance = this;
    }

    public void Register(GravityObject obj)
    {
        if (obj != null && !affectedObjects.Contains(obj))
            affectedObjects.Add(obj);
    }

    public void Unregister(GravityObject obj)
    {
        affectedObjects.Remove(obj);
    }

    private void FixedUpdate()
    {
        for (int i = affectedObjects.Count - 1; i >= 0; i--)
        {
            GravityObject obj = affectedObjects[i];

            if (obj == null)
            {
                affectedObjects.RemoveAt(i);
                continue;
            }

            Rigidbody rb = obj.Rigidbody;

            if (rb == null || rb.isKinematic)
                continue;

            Vector3 gravityDirection = (transform.position - rb.position).normalized;

            rb.AddForce(gravityDirection * gravityStrength, ForceMode.Acceleration);
            if (!obj.rotate) continue;
            Quaternion targetRotation = Quaternion.FromToRotation(rb.transform.up, -gravityDirection) * rb.rotation;

            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 5f * Time.fixedDeltaTime));
        }
    }
}