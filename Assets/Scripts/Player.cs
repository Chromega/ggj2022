using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
   public VRControllerInput leftController;
   public VRControllerInput rightController;

   // Start is called before the first frame update
   void Start()
   {

   }

   // Update is called once per frame
   void Update()
   {
      if (rightController.GetPrimaryButtonDown() || leftController.GetPrimaryButtonDown())
      {
         DoTeleport();
      }
      if (rightController.GetSecondaryButtonDown() || leftController.GetSecondaryButtonDown())
      {
         DoRecallBall();
      }
   }

   void DoTeleport()
   {
      GolfBall ball = GameMgr.Instance.golfBall;
      Vector3 ballPosition = ball.transform.position;
      Vector3 normal = ball.lastPlanetoid.gravitySource.ComputePlayerNormal(ballPosition);

      Quaternion facing = Quaternion.FromToRotation(Vector3.up, normal);
      Vector3 relativePosition = new Vector3(-.5f, 0, 0);

      transform.rotation = facing;
      transform.position = ballPosition + facing * relativePosition;
   }
   void DoRecallBall()
   {
      GolfBall ball = GameMgr.Instance.golfBall;
      Rigidbody rb = ball.GetComponent<Rigidbody>();
      Vector3 relativePosition = new Vector3(.5f, .1f, 0);
      Vector3 worldPosition = transform.TransformPoint(relativePosition);

      rb.position = worldPosition;
      rb.velocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
   }
}
