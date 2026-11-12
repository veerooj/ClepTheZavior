using System;
using UnityEngine;

public class BasketDino : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<GravityObject>()?.gameObject.SetActive(false);
    }
}
