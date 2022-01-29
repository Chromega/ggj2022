using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBall : MonoBehaviour
{
   public Planetoid lastPlanetoid;
   public Planetoid closestPlanetoid;
   public TrailRenderer trail;
   public Rigidbody rb;

   public ParticleSystem entryBurnFx;

   float chargeTime;

   float burnTimeRemaining;
   
   // Update is called once per frame
   void Update()
   {
      if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse2))
      {
         chargeTime += Time.deltaTime;
      }
      if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse2))
      {
         Vector3 direction = Camera.main.transform.forward;

         rb.AddForce(direction*chargeTime*3f, ForceMode.Impulse);
         chargeTime = 0;
         GetComponent<Rigidbody>().useGravity = true;
      }

      var emission = entryBurnFx.emission;
      emission.rateOverDistance = burnTimeRemaining > 0.0f ? 10 : 0;

      burnTimeRemaining -= Time.deltaTime;

      closestPlanetoid = GameMgr.Instance.GetClosestPlanetoid(transform.position);
   }

   private void FixedUpdate()
   {
      if (burnTimeRemaining > 0.0f)
      {
         Vector3 force = -rb.velocity * .1f;
         rb.AddForce(force);
      }
   }

   private void OnCollisionEnter(Collision collision)
   {
      if (collision.collider.gameObject.layer == 8) //planetoid
      {
         lastPlanetoid = collision.collider.gameObject.GetComponentInParent<Planetoid>();
      }
      else if (collision.collider.gameObject.layer == 7) //golf club
      {
         GetComponent<Rigidbody>().useGravity = true;
      }
   }

   public void Reset()
   {
      GetComponent<Rigidbody>().useGravity = false;
      trail.Clear();
   }

   private void OnTriggerStay(Collider other)
   {
      GravitySource gs = other.GetComponent<GravitySource>();
      if (gs)
      {
         if (gs.canDoEntryBurn)
         {

            Planetoid p = gs.GetComponentInParent<Planetoid>();
            if (p != lastPlanetoid)
            {
               if (gs.IsInBurnRange(transform.position))
               {
                  burnTimeRemaining = .2f;

               }
            }
         }
      }
   }
}
