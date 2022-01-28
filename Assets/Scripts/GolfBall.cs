using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBall : MonoBehaviour
{
   [HideInInspector]
   public Planetoid lastPlanetoid;

   // Start is called before the first frame update
   void Start()
   {

   }

   // Update is called once per frame
   void Update()
   {

   }

   private void OnCollisionEnter(Collision collision)
   {
      if (collision.collider.gameObject.layer == 8) //planetoid
      {
         lastPlanetoid = collision.collider.gameObject.GetComponent<Planetoid>();
      }
   }
}
