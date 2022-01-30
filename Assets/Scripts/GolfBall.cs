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
   public ParticleSystem onFireFx;

   float chargeTime;

   float burnTimeRemaining;
   static double fireEndTime;

   public Material ghostMaterial;
   public Material originalMaterial;

   bool isClosestBallToPlayer()
   {
      GolfBall closestBall = GameMgr.Instance.GetClosestBallToPlayer();
      return GameObject.ReferenceEquals(gameObject, closestBall.gameObject);
   }

   bool isGhostBall()
   {
      GolfBall gb = GameMgr.Instance.GetGhostBall();
      return GameObject.ReferenceEquals(gameObject, gb.gameObject);
   }

   public void ToggleMaterial(bool setAsMainBall)
   {
      // Flip whether a ghost or regular material
      if (setAsMainBall)
      {
         foreach (MeshRenderer r in transform.GetChild(0).GetComponentsInChildren<MeshRenderer>())
         {
            r.sharedMaterial = originalMaterial;
         }
      } else
      {
         foreach (MeshRenderer r in transform.GetChild(0).GetComponentsInChildren<MeshRenderer>())
         {
            r.sharedMaterial = ghostMaterial;
         }
      }

   }

   public void CheckGhostCollision()
   {
      // Enable the golf ball to have gravity
      GetComponent<Rigidbody>().useGravity = true;

      // Set this ball as the active ball, and the other ball as the ghost ball
      if (isGhostBall())
      {
         GameMgr.Instance.SwapGhostBall();
      }

      // Respawn a new ghost ball
      GameMgr.Instance.ReturnGhostBall();
   }

   // Update is called once per frame
   void Update()
   {
      if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse2))
      {
         chargeTime += Time.deltaTime;
      }
      if (isClosestBallToPlayer() && (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse2)))
      {
         // Hit the golf ball
         Vector3 direction = Camera.main.transform.forward;
         rb.AddForce(direction * chargeTime * 3f, ForceMode.Impulse);
         chargeTime = 0;

         // Set this ball as the active ball, and the other ball as the ghost ball
         CheckGhostCollision();
      }

      {
         var emission = entryBurnFx.emission;
         emission.rateOverDistance = burnTimeRemaining > 0.0f ? 10 : 0;
      }

      {
         var emission = onFireFx.emission;
         emission.rateOverTime = IsOnFire() ? 40 : 0;
      }

      burnTimeRemaining -= Time.deltaTime;
      
      closestPlanetoid = GameMgr.Instance.GetClosestPlanetoid(transform.position);
   }

   public static bool IsOnFire()
   {
      return Time.realtimeSinceStartupAsDouble < fireEndTime;
   }

   public static void Extinguish()
   {
      fireEndTime = 0;
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
      if (collision.collider.gameObject.layer == 8 || collision.collider.gameObject.layer == 10) //planetoid
      {
         lastPlanetoid = collision.collider.gameObject.GetComponentInParent<Planetoid>();
      }
      else if (collision.collider.gameObject.layer == 7) //golf club
      {
         CheckGhostCollision();
      }
   }

   public void Reset()
   {
      Rigidbody rb = GetComponent<Rigidbody>();
      transform.position = GameMgr.Instance.player.GetBallStartPosition();
      rb.velocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
      rb.useGravity = false;
      gameObject.SetActive(true);
      trail.Clear();
   }

   private void OnTriggerEnter(Collider other)
   {
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

      if (other.tag == "Fire")
      {
         fireEndTime = Time.realtimeSinceStartupAsDouble + 60;
      }
   }
}
