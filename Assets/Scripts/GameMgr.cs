using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
   public static GameMgr Instance { get; private set; }

   public GolfBall golfBall;
   public Player player;
   List<Planetoid> planetoids = new List<Planetoid>();

   // The ghost golf ball is the 'reserve' ball. We will spawn it shortly after you hit your original ball.
   GolfBall ghostBall;
   public float distanceBeforeGhostBallSpawns = 2f;

   private void Awake()
   {
      Instance = this;

      // Enable fixed foveated rendering
      // per: https://forum.unity.com/threads/how-to-enable-fixed-foveat-rendering.1172960/
      Unity.XR.Oculus.Utils.SetFoveationLevel(3);
   }

   private void OnDestroy()
   {
      if (Instance == this)
         Instance = null;
   }

   public void AddPlanetoid(Planetoid p)
   {
      planetoids.Add(p);
   }

   public void ToggleBgMusic()
   {
      AudioSource audioSource = GetComponent<AudioSource>();
      if (!audioSource.isPlaying)
      {
         audioSource.Play();
      } else
      {
         audioSource.Stop();
      }
   }

   public Planetoid GetClosestPlanetoid(Vector3 pos)
   {
      Planetoid bestPlanetoid = null;
      float bestDistanceSq = float.PositiveInfinity;
      foreach (Planetoid p in planetoids)
      {
         float distSq = (p.transform.position - pos).sqrMagnitude;
         if (distSq < bestDistanceSq)
         {
            bestDistanceSq = distSq;
            bestPlanetoid = p;
         }
      }
      return bestPlanetoid;
   }


   public void Update() {

      // if the main ball is far away, then show the ghost ball
      float dist = Vector3.Distance(player.transform.position, golfBall.transform.position);
      if (dist > distanceBeforeGhostBallSpawns)
      {
         ghostBall.gameObject.SetActive(true);
      }
   }

   public void ReturnGhostBall()
   {
      ghostBall.gameObject.SetActive(false);
      ghostBall.transform.position = player.GetBallStartPosition();
      Rigidbody rb = ghostBall.GetComponent<Rigidbody>();
      rb.velocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
      rb.useGravity = false;
      ghostBall.trail.Clear();
   }

   public void SwapGhostBall()
   {
      // swap who's in which position
      GolfBall gb = golfBall;
      GolfBall ghost = ghostBall;
      golfBall = ghost;
      ghostBall = gb;

      golfBall.ToggleMaterial(true);
      golfBall.gameObject.SetActive(true);

      ghostBall.ToggleMaterial(false);
      ghostBall.gameObject.SetActive(false);
   }

   public GolfBall GetGhostBall()
   {
      return ghostBall;
   }

   void Start()
   {
      ghostBall = Instantiate(golfBall, transform.parent);
      ghostBall.ToggleMaterial(false);
      ghostBall.gameObject.SetActive(false);
   }

   public GolfBall GetClosestBallToPlayer()
   {
      // handle the case of initial spawn, where the ghost ball isn't active
      if (!ghostBall.gameObject.activeSelf)
      {
         return golfBall;
      }

      // return the closest ball the player (this is how we determine which ball gets "hit"
      float golfBallDist = Vector3.Distance(player.transform.position, golfBall.transform.position);
      float ghostBallDist = Vector3.Distance(player.transform.position, ghostBall.transform.position);
      if (golfBallDist < ghostBallDist)
      {
         return golfBall;
      }
      else
      {
         return ghostBall;
      }
   }

   public GolfBall GetOtherBall(GolfBall gb)
   {
      if (GameObject.ReferenceEquals(golfBall.gameObject, gb.gameObject))
      {
         return ghostBall;
      }
      else
      {
         return golfBall;
      }
   }
}