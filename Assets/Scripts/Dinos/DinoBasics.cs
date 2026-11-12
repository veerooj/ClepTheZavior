using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoBasics : MonoBehaviour
{
   private Rigidbody _rigidbody;
   [SerializeField]private float movingCooldownMin,movingCooldownMax,rotateCooldownMin,rotateCooldownMax,rotatingForce, movingForce;
   private void Awake()
   {
      _rigidbody = GetComponent<Rigidbody>();
   }
   
   private void Start()
   {
      StartCoroutine(Moving());
   }
   private IEnumerator Moving()
   {
      float rand = Random.Range(movingCooldownMin,movingCooldownMax);
      yield return new WaitForSeconds(rand);
      _rigidbody.AddForce(transform.forward * movingForce, ForceMode.Impulse);
      StartCoroutine(Rotating());
   }

   private IEnumerator Rotating()
   {
      float rand =  Random.Range(rotateCooldownMin,rotateCooldownMax);
      yield return new WaitForSeconds(rand);
      var randforce = Random.Range(-rotatingForce, rotatingForce);
      _rigidbody.AddTorque(transform.up * randforce, ForceMode.Impulse);
      StartCoroutine(Moving());
   }

   public void OnStanding()
   {
      StartCoroutine(Moving());
   }
   
}
