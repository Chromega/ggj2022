using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PlayerNonVRMove : MonoBehaviour
{
   public Camera camera;

   void Start()
   {
   }

   void Update()
   {
      var x = Input.GetAxis("Horizontal") * 0.02f;
      var y = Input.GetAxis("Vertical") * 0.02f;
      var z = Input.GetAxis("TrueVertical") * 0.02f;

      Vector3 right = x * camera.transform.right;
      Vector3 forward = camera.transform.forward;
      forward.y = 0;
      forward.Normalize();
      forward *= y;
      Vector3 up = z * Vector3.up;

      transform.Translate(right + forward + up, Space.World);
   }
}