using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfClub : Pickupable
{
   public GameObject rootToClone;

   GameObject visual;

   Grabber primaryGrabber;
   Grabber secondaryGrabber;

   Quaternion lastTargetRotation;
   Vector3 targetAngularVelocity;

   Vector3 lastTargetPosition;
   Vector3 targetVelocity;

   [HideInInspector]
   public ClubRack clubRack;

   bool magnetizeToRack = true;
   Coroutine releasedCoroutine;

   public AudioClipPool whooshSfx;
   public AudioSource audioSource;

   public AudioClipPool sfxPickup;

   float timeSinceLastWhoosh = 999f;

   protected override void Start()
   {
      base.Start();

      rb.useGravity = false;

      visual = Instantiate(rootToClone);
      visual.name = "Visual";
      foreach (Collider c in visual.GetComponentsInChildren<Collider>())
      {
         c.enabled = false;
      }
      visual.transform.parent = transform;
      visual.transform.localPosition = Vector3.zero;
      visual.transform.localRotation = Quaternion.identity;
      visual.transform.localScale = Vector3.one;

      foreach (MeshRenderer r in rootToClone.GetComponentsInChildren<MeshRenderer>())
      {
         r.enabled = false;
      }
   }

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
      else if (magnetizeToRack)
      {
         return clubRack.GetClubWorldPosition(this);
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
      else if (magnetizeToRack)
      {
         return clubRack.GetClubWorldRotation(this);
      }
      else
      {
         return transform.rotation;
      }
   }

   protected override Vector3 ComputeTargetVelocity()
   {
      if (magnetizeToRack)
         return Vector3.zero;
      else
         return targetVelocity;
   }

   protected override Vector3 ComputeTargetAngularVelocity()
   {
      if (magnetizeToRack)
         return Vector3.zero;
      else
         return targetAngularVelocity;
   }

   protected override bool ShouldTrackTarget()
   {
      return HasGrabber() || magnetizeToRack;
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

   protected override void Update()
   {
      if (HasGrabber())
      {
         visual.transform.position = ComputeTargetPosition();
         visual.transform.rotation = ComputeTargetRotation();

         float angularSpeed = ComputeTargetAngularVelocity().magnitude;
         if (timeSinceLastWhoosh > .5f && angularSpeed > 20.0f)
         {
            audioSource.PlayOneShot(whooshSfx.GetClip());
            timeSinceLastWhoosh = 0.0f;
         }
      }
      timeSinceLastWhoosh += Time.deltaTime;

      base.Update();
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

      visual.transform.parent = null;
      SetCollisionLayer(7);

      if (releasedCoroutine != null)
      {
         StopCoroutine(releasedCoroutine);
         releasedCoroutine = null;
      }
      magnetizeToRack = false;

      // play a pick up sound
      audioSource.PlayOneShot(sfxPickup.GetClip());
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

      if (!HasGrabber())
      {
         visual.transform.parent = transform;
         visual.transform.localPosition = Vector3.zero;
         visual.transform.localRotation = Quaternion.identity;
         visual.transform.localScale = Vector3.one;
         SetCollisionLayer(0);



         releasedCoroutine = StartCoroutine(OnReleased());
      }
   }

   void SetCollisionLayer(int layer)
   {
      gameObject.layer = layer;
      foreach (Collider c in rb.GetComponentsInChildren<Collider>())
         c.gameObject.layer = layer;
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

   IEnumerator OnReleased()
   {
      yield return new WaitForSeconds(3.0f);
      rb.useGravity = false;
      magnetizeToRack = true;
      SetCollisionLayer(9);
   }

   protected override float GetSpringDistanceLimit()
   {
      if (magnetizeToRack)
      {
         return 1f;
      }
      else
      {
         return Mathf.Infinity;
      }
   }
}
