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

   private void Awake()
   {
      Instance = this;
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
   public GolfBall GetGhostBall()
   {
      return ghostBall;
   }

   public void SetGhostBall(GolfBall gb)
   {
      ghostBall = gb;
   }

   void Start()
   {
      ghostBall = Instantiate(golfBall, transform.parent);
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