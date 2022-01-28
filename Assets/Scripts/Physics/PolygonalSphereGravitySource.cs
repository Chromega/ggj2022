using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonalSphereGravitySource : GravitySource
{
   public float surfaceRadius;
   public float surfaceGForce;


   protected override Vector3 ComputeForce(Rigidbody rb)
   {
      Vector3 displacement = rb.position - transform.position;
      float distance = displacement.magnitude;

      RaycastHit hitInfo;
      Vector3 outwardGravityDirection;
      if (Physics.Raycast(rb.position, -displacement, out hitInfo, distance, 1 << 8))
      {
         Vector3 normal = hitInfo.normal;
         float angleDeflection = Vector3.Angle(displacement, normal); //degrees??? rads???
         if (angleDeflection < 30f)
         {
            outwardGravityDirection = normal;
         }
         else
         {
            outwardGravityDirection = displacement;
         }
      }
      else
      {
         outwardGravityDirection = displacement;
      }

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
