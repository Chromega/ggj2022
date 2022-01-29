using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
   public static GameMgr Instance { get; private set; }

   public GolfBall golfBall;
   public Player player;
   List<Planetoid> planetoids = new List<Planetoid>();

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
}
