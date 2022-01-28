using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfClub : Pickupable
{

   Grabber primaryGrabber;
   Grabber secondaryGrabber;

   Quaternion lastTargetRotation;
   Vector3 targetAngularVelocity;

   Vector3 lastTargetPosition;
   Vector3 targetVelocity;

   protected override Vector3 ComputeTargetPosition()
   {
      if (secondaryGrabber)
      {
         return primaryGrabber.GetTargetPosition() + primaryGrabber.GetTargetRotation() * primaryGrabber.currentPosOffset; //ehhhh
      }
      else if (primaryGrabber)
      {
         return primaryGrabber.GetTargetPosition() + primaryGrabber.GetTargetRotation() * primaryGrabber.currentPosOffset;
      }
      else
      {
         return transform.position;
      }
   }

   protected override Quaternion ComputeTargetRotation()
   {
      if (secondaryGrabber)
      {
         Vector3 newZ = -(secondaryGrabber.transform.position - primaryGrabber.transform.position);
         newZ.Normalize();

         Vector3 newX = -Vector3.Cross(primaryGrabber.transform.up, newZ); //damn left handed coordinates
         newX.Normalize();

         Vector3 newY = -Vector3.Cross(newZ, newX);
         newY.Normalize();

         return Quaternion.LookRotation(newZ, newY);
      }
      else if (primaryGrabber)
      {
         return primaryGrabber.GetTargetRotation() * primaryGrabber.currentRotOffset;
      }
      else
      {
         return transform.rotation;
      }
   }

   protected override Vector3 ComputeTargetVelocity()
   {
      return targetVelocity;
   }

   protected override Vector3 ComputeTargetAngularVelocity()
   {
      return targetAngularVelocity;
   }

   public override void FixedUpdate()
   {
      Quaternion targetRotation = ComputeTargetRotation();
      Quaternion deltaRotation = Quaternion.Inverse(lastTargetRotation) * targetRotation;
      lastTargetRotation = targetRotation;

      float angle;
      Vector3 axis;
      deltaRotation.ToAngleAxis(out angle, out axis);
      angle *= Mathf.Deg2Rad;
      targetAngularVelocity = angle * axis / Time.fixedDeltaTime;
      targetAngularVelocity = transform.TransformDirection(targetAngularVelocity);

      Vector3 targetPosition = ComputeTargetPosition();
      targetVelocity = (targetPosition - lastTargetPosition)/Time.fixedDeltaTime;
      lastTargetPosition = targetPosition;

      base.FixedUpdate();
   }

   public override void AddGrabber(Grabber grabber)
   {
      if (grabbers.Contains(grabber))
         return;

      if (!primaryGrabber)
         primaryGrabber = grabber;
      else
         secondaryGrabber = grabber;

      base.AddGrabber(grabber);
   }

   public override void RemoveGrabber(Grabber grabber)
   {
      if (primaryGrabber == grabber)
      {
         primaryGrabber = secondaryGrabber;
         secondaryGrabber = null;
      }
      if (secondaryGrabber == grabber)
         secondaryGrabber = null;

      base.RemoveGrabber(grabber);
   }

   private void OnCollisionEnter(Collision collision)
   {
      if (rb.isKinematic)
      {
         ContactPoint contact = collision.GetContact(0);

         Vector3 point = contact.point;
         Vector3 normal = -contact.normal;

         Vector3 comVelocity = ComputeTargetVelocity();
         Vector3 angularVelocity = ComputeTargetAngularVelocity();

         Vector3 pointVelocity = comVelocity + Vector3.Cross(angularVelocity, point - transform.position);

         //normal = pointVelocity.normalized; //NORMAL IS WHACK

         Vector3 relativeVelocity = -pointVelocity;
         Vector3 relativeVelocityAfterCollision = relativeVelocity - 2 * normal * Vector3.Dot(relativeVelocity, normal);

         Vector3 newWorldVelocity = relativeVelocityAfterCollision + pointVelocity;

         /*Debug.Log("-2 c " + comVelocity + "    " + angularVelocity);
         Debug.Log("-1 n " + normal);
         Debug.Log("0 wv " + pointVelocity);
         Debug.Log("1 rv " + relativeVelocity);
         Debug.Log("2 rv prime " + relativeVelocityAfterCollision);
         Debug.Log("3 wv prime " + newWorldVelocity);*/

         collision.rigidbody.velocity = newWorldVelocity;
      }
   }
}
