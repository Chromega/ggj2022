using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GravitySource : MonoBehaviour
{
   protected abstract Vector3 ComputeForce(Rigidbody rb);

   public abstract Vector3 ComputePlayerNormal(Vector3 position);

   public virtual bool IsInBurnRange(Vector3 position)
   {
      return false;
   }

   public bool canDoEntryBurn = true;
   
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
