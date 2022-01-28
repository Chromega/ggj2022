using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GravitySource : MonoBehaviour
{
   protected abstract Vector3 ComputeForce(Rigidbody rb);
   
   protected virtual void Start()
   {
      Physics.gravity = Vector3.zero;
   }

   void OnTriggerStay(Collider c)
   {
      if (c.attachedRigidbody && !c.attachedRigidbody.isKinematic && c.attachedRigidbody.useGravity)
      {
         Vector3 force = ComputeForce(c.attachedRigidbody);
         c.attachedRigidbody.AddForce(force);
      }
   }
}
