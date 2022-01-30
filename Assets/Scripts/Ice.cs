using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour
{
   public Collider iceCollider;
   public ParticleSystem meltParticleSystem;
   new public Renderer renderer;

   private void OnCollisionEnter(Collision collision)
   {
      if (collision.collider.gameObject.layer == 6) //golf ball
      {
         if (GolfBall.IsOnFire())
         {
            GolfBall.Extinguish();
            iceCollider.enabled = false;
            renderer.enabled = false;
            meltParticleSystem.Play();
         }
      }
   }
}
