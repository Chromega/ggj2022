using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfGoal : MonoBehaviour
{
   public GameObject visual;
   public ParticleSystem explodeFx;

   Planetoid planetoid;

   bool collected = false;

   private void Awake()
   {
      planetoid = GetComponentInParent<Planetoid>();
      planetoid.AddGoal(this);
   }
   private void OnTriggerEnter(Collider other)
   {
      if (other.gameObject.layer == 6) //Golf ball
      {
         visual.SetActive(false);
         explodeFx.Play();
         collected = true;
         planetoid.NotifyOnGoalCollected(this);
      }
   }

   public bool GetCollected()
   {
      return collected;
   }
}
