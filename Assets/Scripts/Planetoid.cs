using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planetoid : MonoBehaviour
{
   public GravitySource gravitySource;
   List<GolfGoal> goals = new List<GolfGoal>();

   public System.Action OnGoalCollected;

   private void Awake()
   {
      gameObject.layer = 8; //Planetoid
   }

   public void AddGoal(GolfGoal goal)
   {
      goals.Add(goal);
   }

   public void NotifyOnGoalCollected(GolfGoal goal)
   {
      OnGoalCollected?.Invoke();
   }

   public void GetGoalProgress(out int numCollected, out int total)
   {
      numCollected = 0;
      total = goals.Count;

      foreach (GolfGoal goal in goals)
      {
         if (goal.GetCollected())
            ++numCollected;
      }
   }
}
