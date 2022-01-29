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

   // Use to calculate spawning a cloned ghost ball for convenience
   public float timeBeforeNewBallSpawns;
   Coroutine releasedCoroutine;
   float prevDistToPlayer = 0f;

   IEnumerator SpawnGhostBall()
   {
      yield return new WaitForSeconds(timeBeforeNewBallSpawns);
      GolfBall ghostBall = GameMgr.Instance.GetGhostBall();
      ghostBall.gameObject.SetActive(true);
      ghostBall.Reset();
   }

   bool isClosestBallToPlayer()
   {
      GolfBall closestBall = GameMgr.Instance.GetClosestBallToPlayer();
      return GameObject.ReferenceEquals(gameObject, closestBall.gameObject);
   }

   bool isActiveBall()
   {
      GolfBall gb = GameMgr.Instance.golfBall;
      return GameObject.ReferenceEquals(gameObject, gb.gameObject);
   }

   public void DelaySpawnGhost()
   {
      if (releasedCoroutine != null)
      {
         StopCoroutine(releasedCoroutine);
         releasedCoroutine = null;
      }
      releasedCoroutine = StartCoroutine(SpawnGhostBall());
   }

   // Update is called once per frame
   void Update()
   {
      Debug.Log(isClosestBallToPlayer());
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

         // Enable the golf ball to have gravity
         GetComponent<Rigidbody>().useGravity = true;

         // Set this ball as the active ball, and the other ball as the ghost ball
         GolfBall otherBall = GameMgr.Instance.GetOtherBall(this);
         GameMgr.Instance.golfBall = this;
         GameMgr.Instance.SetGhostBall(otherBall);
      }

      var emission = entryBurnFx.emission;
      emission.rateOverDistance = burnTimeRemaining > 0.0f ? 10 : 0;

      burnTimeRemaining -= Time.deltaTime;
      
      closestPlanetoid = GameMgr.Instance.GetClosestPlanetoid(transform.position);

      if (isActiveBall())
      {
         float distToPlayer = Vector3.Distance(GameMgr.Instance.player.transform.position, transform.position);
         if (prevDistToPlayer < 3f && distToPlayer > 3f)
         {
            // spawn the ghost ball back at the original player
            DelaySpawnGhost();
         }
         prevDistToPlayer = distToPlayer;
      }
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
      Rigidbody rb = GetComponent<Rigidbody>();
      transform.position = GameMgr.Instance.player.GetBallStartPosition();
      rb.velocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
      rb.useGravity = false;
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
