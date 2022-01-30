using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGravitySource : PolygonalSphereGravitySource
{
   public override Vector3 ComputePlayerNormal(Vector3 position)
   {
      Vector3 localPoint = transform.InverseTransformPoint(position);
      float absX = Mathf.Abs(localPoint.x);
      float absY = Mathf.Abs(localPoint.y);
      float absZ = Mathf.Abs(localPoint.z);

      if (absX > absY && absX > absZ)
      {
         if (localPoint.x > 0)
            return transform.right;
         else
            return -transform.right;
      }
      else if (absY > absZ)
      {
         if (localPoint.y > 0)
            return transform.up;
         else
            return -transform.up;
      }
      else
      {
         if (localPoint.z > 0)
            return transform.forward;
         else
            return -transform.forward;
      }
   }

   protected override Vector3 ComputeForce(Rigidbody rb)
   {
      Vector3 displacement = rb.position - transform.position;
      float distance = displacement.magnitude;
      
      Vector3 outwardGravityDirection = ComputePlayerNormal(rb.position);
      Debug.Log(outwardGravityDirection);

      outwardGravityDirection.Normalize();


      if (distance > surfaceRadius)
      {
         //F(r) = -GMm/r^2 r_hat
         //F(r') = -GMm/(r')^2 r_hat
         //g_s = -GM/(r')^2 r_hat
         //F(r)=g_s*(r')^2/r^2 r_hat
         Vector3 force = surfaceGForce * (-9.81f) * surfaceRadius * surfaceRadius / (distance * distance) * outwardGravityDirection * rb.mass;
         return force;
      }
      else
      {
         //Force goes down linearly on the interior
         Vector3 force = surfaceGForce * (-9.81f) * distance / surfaceRadius * outwardGravityDirection * rb.mass;
         return force;
      }
   }
}
