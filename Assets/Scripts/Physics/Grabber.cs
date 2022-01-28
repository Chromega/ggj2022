using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
   public GameObject visual;
   public VRControllerInput controllerInput;
   public Color grabbableColor = Color.blue;

   protected bool wasHolding = false;

   Vector3 lastPosition = Vector3.zero;
   Quaternion lastRotation = Quaternion.identity;
   Vector3 velocity = Vector3.zero;
   Vector3 angularVelocity = Vector3.zero;

   HashSet<Grabbable> overlappedGrabbables = new HashSet<Grabbable>();
   protected Grabbable currentlyHeldGrabbable;
   protected Grabbable bestOverlappedGrabbable = null;

   [HideInInspector]
   public Vector3 currentPosOffset;
   [HideInInspector]
   public Quaternion currentRotOffset;

   public Vector3 GetTargetPosition()
   {
      return transform.position;
   }

   public Quaternion GetTargetRotation()
   {
      return transform.rotation;
   }

   public Vector3 GetTargetVelocity()
   {
      return velocity;
   }

   public Vector3 GetTargetAngularVelocity()
   {
      return angularVelocity;
   }

   bool IsHoldingGrabbable()
   {
      return currentlyHeldGrabbable != null;
   }

   // Update is called once per frame
   public virtual void LateUpdate()
   {
      if (visual == null)
      {
         // This was for grabbing objects that are fixed to a location. Redraw the hand locked to that object.
         visual = transform.childCount > 0 ? transform.GetChild(0).gameObject : null;
         if (visual)
            visual.layer = 12;
      }

      //snap hand onto grabbable that constrain positions
      if (visual)
      {
         if (IsHoldingGrabbable() && currentlyHeldGrabbable.ConstrainsPosition())
         {
            visual.transform.position = currentlyHeldGrabbable.GetGrabPosition();
         }
         else
         {
            visual.transform.localPosition = Vector3.zero;
         }
      }
      float grabAmount = GetGrab();
      if (grabAmount > .75f && !wasHolding)
      {
         TryGrab();
         wasHolding = true;
      }
      if (grabAmount < .5f)
      {
         if (IsHoldingGrabbable())
            Release();
         wasHolding = false;
      }

      velocity = (transform.position - lastPosition) / Time.deltaTime;

      Quaternion difference = transform.rotation * Quaternion.Inverse(lastRotation);
      Vector3 diffAxis;
      float diffAngle;
      difference.ToAngleAxis(out diffAngle, out diffAxis);
      if (diffAngle > 180)
         diffAngle -= 360;
      else if (diffAngle < -180)
         diffAngle += 360;

      if (Mathf.Abs(diffAngle) > .01f)
      {
         angularVelocity = diffAngle * Mathf.Deg2Rad / Time.deltaTime * diffAxis;
      }
      else
      {
         angularVelocity = Vector3.zero;
      }
      lastPosition = transform.position;
      lastRotation = transform.rotation;
   }

   void TryGrab()
   {
      Grabbable bestGrabbable = FindBestPotentialGrabbable();

      if (bestGrabbable != null)
      {
         DoGrab(bestGrabbable);
      }
   }

   Grabbable FindBestPotentialGrabbable()
   {
      //Null if you already have something
      if (currentlyHeldGrabbable != null)
         return null;

      Grabbable bestGrabbable = null;
      float bestDistanceSquared = Mathf.Infinity;

      overlappedGrabbables.RemoveWhere((grabbable) => { return !grabbable; });

      foreach (Grabbable g in overlappedGrabbables)
      {
         if (!g.CanAcceptMoreGrabbers())
            continue;
         if (!g.CanBeGrabbed())
            continue;

         Vector3 grabbablePosition;
         if (g.ConstrainsPosition())
            grabbablePosition = g.GetGrabPosition();
         else
            grabbablePosition = g.transform.position;

         float distanceSq = (transform.position - grabbablePosition).sqrMagnitude;

         if (distanceSq < bestDistanceSquared)
         {
            bestGrabbable = g;
            bestDistanceSquared = distanceSq;
         }
      }
      return bestGrabbable;
   }

   private void Update()
   {
      Grabbable newBestGrabbable = FindBestPotentialGrabbable();
      if (newBestGrabbable == bestOverlappedGrabbable)
         return;

      if (bestOverlappedGrabbable != null)
         bestOverlappedGrabbable.SetIsBestGrabbable(false, this);

      bestOverlappedGrabbable = newBestGrabbable;

      if (bestOverlappedGrabbable != null)
         bestOverlappedGrabbable.SetIsBestGrabbable(true, this);
   }

   void DoGrab(Grabbable grabbable)
   {
      if (controllerInput)
         controllerInput.DoHapticImpulse(.5f, .1f);

      grabbable.AddGrabber(this);
      currentlyHeldGrabbable = grabbable;
   }

   private void OnDestroy()
   {
   }

   public void ForceGrab(Grabbable grabbable)
   {
      if (IsHoldingGrabbable())
      {
         Release();
      }
      DoGrab(grabbable);
   }

   public void Release()
   {
      if (currentlyHeldGrabbable)
      {
         currentlyHeldGrabbable.RemoveGrabber(this);
         currentlyHeldGrabbable = null;
      }
   }

   void OnTriggerEnter(Collider c)
   {

      Grabbable newGrabbable = c.GetComponent<Grabbable>();
      if (!newGrabbable)
         newGrabbable = c.GetComponentInParent<Grabbable>();

      overlappedGrabbables.Add(newGrabbable);
   }

   void OnTriggerExit(Collider c)
   {
      Grabbable newGrabbable = c.GetComponent<Grabbable>();
      if (!newGrabbable)
         newGrabbable = c.GetComponentInParent<Grabbable>();

      overlappedGrabbables.Remove(newGrabbable);
   }

   protected virtual float GetGrab()
   {
      return controllerInput.GetGrip();
   }

   public virtual float GetTrigger()
   {
      return controllerInput.GetTrigger();
   }

}
