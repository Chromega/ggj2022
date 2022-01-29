using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
   private void LateUpdate()
   {
      //transform.rotation = Camera.main.transform.rotation; //Without locking z up

      Vector3 objToCamVec = transform.position - Camera.main.transform.position;

      Vector3 up = GameMgr.Instance.player.transform.up;
      float upAmount = Vector3.Dot(objToCamVec, up);
      objToCamVec -= up * upAmount;
      //objToCamVec.y = 0;

      Quaternion q = Quaternion.LookRotation(objToCamVec, up);
      //Quaternion q = Quaternion.FromToRotation(transform.forward, objToCamVec.normalized);

      transform.rotation = q;
   }
}
