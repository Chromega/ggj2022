using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PlayerMove : MonoBehaviour
{

   void Update()
   {
      var x = Input.GetAxis("Horizontal") * 0.02f;
      var y = Input.GetAxis("Vertical") * 0.02f;
      var z = Input.GetAxis("TrueVertical") * 0.02f;

      Vector3 right = x * Camera.main.transform.right;
      Vector3 forward = Camera.main.transform.forward;
      forward.y = 0;
      forward.Normalize();
      forward *= y;
      Vector3 up = z * Vector3.up;

      transform.Translate(right + forward + up, Space.World);
   }
}