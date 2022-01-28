using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : Grabbable
{
   protected float positionalKp = 1500f;
   protected float positionalKd = 45f;

   protected float rotationalKp = 500f;
   protected float rotationalKd = 10f;

   protected Rigidbody rb;

   public bool preserveGrabPosition = true;
   public bool preserveGrabRotation = true;

   protected virtual void Start()
   {
      rb = GetComponent<Rigidbody>();
   }

   public override Vector3 GetGrabPosition()
   {
      throw new NotImplementedException();
   }
   public override bool ConstrainsPosition()
   {
      return false;
   }

   protected virtual Vector3 ComputeTargetPosition()
   {
      Vector3 pos = Vector3.zero;
      foreach (Grabber grabber in grabbers)
      {
         pos += (grabber.GetTargetPosition() + grabber.GetTargetRotation() * grabber.currentPosOffset)/grabbers.Count;
         //pos += grabber.GetTargetPosition()/grabbers.Count;
      }
      return pos;
   }

   protected virtual Quaternion ComputeTargetRotation()
   {
      if (grabbers.Count == 1)
      {
         foreach (Grabber g in grabbers)
         {
            return g.GetTargetRotation() * g.currentRotOffset;
         }
      }
      else if (grabbers.Count == 2)
      {
         Grabber g1 = null;
         Grabber g2 = null;
         foreach (Grabber g in grabbers)
         {
            if (g1 == null)
               g1 = g;
            else
               g2 = g;
         }

         Debug.Log("-1 xfm: " + transform.rotation);
         Vector3 currentGrabberAxisWorld = g1.GetTargetPosition() - g2.GetTargetPosition();
         Debug.Log("0 current grabber axis world: " + currentGrabberAxisWorld);
         currentGrabberAxisWorld.Normalize();
         Vector3 currentGrabberAxisLocal = transform.InverseTransformDirection(currentGrabberAxisWorld);

         Vector3 originalGrabberAxisLocal = g1.currentPosOffset - g2.currentPosOffset;
         originalGrabberAxisLocal.Normalize();
         Vector3 originalGrabberAxisWorld = transform.TransformDirection(originalGrabberAxisLocal);

         Quaternion g1TargetRot = Quaternion.identity;
         Quaternion g2TargetRot = Quaternion.identity;
         int idx = 0;
         foreach (Grabber g in grabbers)
         {
            bool debug = (g as DebugGrabber).log;
            Vector3 originalGrabberAxisGrabberSpace = Quaternion.Inverse(g.currentRotOffset) * originalGrabberAxisWorld;

            Vector3 originalGrabberAxisCurrentWorldSpace = g.GetTargetRotation() * originalGrabberAxisGrabberSpace;

            if (debug) Debug.Log("1 Tilted axis: " + originalGrabberAxisCurrentWorldSpace);

            Vector3 cross = Vector3.Cross(originalGrabberAxisWorld, originalGrabberAxisCurrentWorldSpace);
            float dot = Vector3.Dot(originalGrabberAxisWorld, originalGrabberAxisCurrentWorldSpace);
            float angle = Mathf.Atan2(cross.magnitude, dot);
            Vector3 axis = cross.normalized;
            if (debug) Debug.Log("2 Axis angle: " + axis + " " + (angle*Mathf.Rad2Deg));

            Quaternion correction = Quaternion.AngleAxis(-angle * Mathf.Rad2Deg, axis);

            //Quaternion targetRot = g.GetTargetRotation() * g.currentRotOffset * correction; //Check mult order
            Quaternion targetRot = correction * g.GetTargetRotation() * g.currentRotOffset; //Check mult order

            if (idx == 0)
               g1TargetRot = targetRot;
            else
               g2TargetRot = targetRot;
            ++idx;
         }
         Debug.Log("3 original world: " + originalGrabberAxisWorld);
         Debug.Log("4 current world: " + currentGrabberAxisWorld);

         Quaternion axisCorrection = Quaternion.FromToRotation(originalGrabberAxisWorld, currentGrabberAxisWorld);
         Quaternion blend = Quaternion.Slerp(g1TargetRot, g2TargetRot, .5f);

         Quaternion final = axisCorrection * blend;
         Debug.Log("5 final: " + final);
         return final;
      }
      else
      {
         throw new NotImplementedException();
      }
      return Quaternion.identity;
   }

   protected virtual Vector3 ComputeTargetVelocity()
   {
      foreach (Grabber g in grabbers)
      {
         return g.GetTargetVelocity();
      }
      return Vector3.zero;
   }

   protected virtual Vector3 ComputeTargetAngularVelocity()
   {
      foreach (Grabber g in grabbers)
      {
         return g.GetTargetAngularVelocity();
      }
      return Vector3.zero;
   }

   protected virtual bool ShouldTrackTarget()
   {
      return HasGrabber();
   }

   protected virtual float GetMaxForce()
   {
      return float.MaxValue;
   }

   protected virtual float GetMaxTorque()
   {
      return float.MaxValue;
   }
   protected virtual float GetSpringDistanceLimit()
   {
      return float.MaxValue;
   }

   public virtual void FixedUpdate()
   {
      if (ShouldTrackTarget())
      {
         Vector3 grabberPos = ComputeTargetPosition();
         Quaternion grabberRot = ComputeTargetRotation();
         Vector3 grabberAngularVelocity = ComputeTargetAngularVelocity();
         Vector3 grabberVelocity = ComputeTargetVelocity();

         Vector3 myPos = transform.position;
         Vector3 targetPos = ComputeTargetPosition();
         Vector3 targetVel = ComputeTargetVelocity();
         Vector3 displacement = (targetPos - myPos);
         if (displacement.magnitude > GetSpringDistanceLimit())
         {
            displacement *= GetSpringDistanceLimit() / displacement.magnitude;
         }
         Vector3 force = displacement * positionalKp - (rb.velocity - targetVel) * positionalKd;
         force = force * rb.mass;

         if (force.magnitude > GetMaxForce())
         {
            force *= GetMaxForce() / force.magnitude;
         }

         rb.AddForce(force);

         Quaternion myQuat = transform.rotation;
         Quaternion targetRot = ComputeTargetRotation();
         Vector3 targetAngularVel = ComputeTargetAngularVelocity(); ;

         Quaternion diff = targetRot * Quaternion.Inverse(myQuat);

         if (rb.isKinematic)
         {
            rb.position = targetPos;
            rb.rotation = targetRot;
         }
         else
         {
            Vector3 diffAxis;
            float diffAngle;
            diff.ToAngleAxis(out diffAngle, out diffAxis);
            while (diffAngle > 180)
               diffAngle -= 360;
            while (diffAngle < -180)
               diffAngle += 360;

            //Axis becomes NaN
            if (Mathf.Abs(diffAngle) < .01f)
               return;

            // the first multiplier is the porportional; the second is for dampening
            Vector3 angularAccelerationInWorldSpace = diffAngle * Mathf.Deg2Rad * diffAxis * rotationalKp - (rb.angularVelocity - targetAngularVel) * rotationalKd;
            Vector3 angularAccelerationInLocalSpace = transform.InverseTransformVector(angularAccelerationInWorldSpace);
            Vector3 angularAccelerationInTensorSpace = Quaternion.Inverse(rb.inertiaTensorRotation) * angularAccelerationInLocalSpace;
            Vector3 torqueInTensorSpace = Vector3.Scale(angularAccelerationInTensorSpace, rb.inertiaTensor);
            Vector3 torqueInLocalSpace = rb.inertiaTensorRotation * torqueInTensorSpace;
            Vector3 torqueInWorldSpace = transform.TransformVector(torqueInLocalSpace);

            if (torqueInWorldSpace.magnitude > GetMaxTorque())
            {
               torqueInWorldSpace *= GetMaxTorque() / torqueInWorldSpace.magnitude;
            }

            rb.AddTorque(torqueInWorldSpace);
         }
      }
   }

   public override void AddGrabber(Grabber grabber)
   {
      base.AddGrabber(grabber);

      Quaternion grabberInvQuat = Quaternion.identity;
      if (preserveGrabPosition || preserveGrabRotation)
      {
         grabberInvQuat = Quaternion.Inverse(grabber.GetTargetRotation());
      }

      if (preserveGrabPosition)
      {
         grabber.currentPosOffset = grabberInvQuat * (transform.position - grabber.GetTargetPosition());
      }
      else
      {
         grabber.currentPosOffset = Vector3.zero;
      }

      if (preserveGrabRotation)
      {
         grabber.currentRotOffset = grabberInvQuat * transform.rotation;
      }
      else
      {
         grabber.currentRotOffset = Quaternion.identity;
      }
      rb.useGravity = false;
   }

   public override void RemoveGrabber(Grabber grabber)
   {
      base.RemoveGrabber(grabber);

      if (!HasGrabber())
         rb.useGravity = true;
   }
}
