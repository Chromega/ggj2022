using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGravitySource : GravitySource
{
   public float surfaceRadius;
   public float surfaceGForce;


   protected override Vector3 ComputeForce(Rigidbody rb)
   {
      Vector3 displacement = rb.position - transform.position;
      float distance = displacement.magnitude;

      if (distance > surfaceRadius)
      {
         //F(r) = -GMm/r^2 r_hat
         //F(r') = -GMm/(r')^2 r_hat
         //g_s = -GM/(r')^2 r_hat
         //F(r)=g_s*(r')^2/r^2 r_hat
         Vector3 force = surfaceGForce * (-9.81f) * surfaceRadius * surfaceRadius / (distance * distance * distance) * displacement * rb.mass;
         return force;
      }
      else
      {
         //Force goes down linearly on the interior
         Vector3 force = surfaceGForce * (-9.81f) * displacement / surfaceRadius * rb.mass;
         return force;
      }
   }

   public override Vector3 ComputePlayerNormal(Vector3 position)
   {
      Vector3 displacement = position - transform.position;
      return displacement.normalized;
   }
}
