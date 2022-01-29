using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBall : MonoBehaviour
{
   public Planetoid lastPlanetoid;
   public TrailRenderer trail;
   public Rigidbody rb;

   float chargeTime;
   
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
   }

   private void OnCollisionEnter(Collision collision)
   {
      if (collision.collider.gameObject.layer == 8) //planetoid
      {
         lastPlanetoid = collision.collider.gameObject.GetComponent<Planetoid>();
      } else if (collision.collider.gameObject.layer == 7) //golf club
      {
         GetComponent<Rigidbody>().useGravity = true;
      }
   }

   public void Reset()
   {
      GetComponent<Rigidbody>().useGravity = false;
      transform.Find("Trail").GetComponent<TrailRenderer>().Clear();
   }
}
