using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
   public VRControllerInput leftController;
   public VRControllerInput rightController;
   public float snapTurnDeadZone = 0.125f;
   public float snapTurnDebounceTimeSeconds = 1f;
   float snapTurnDebounce = 0f;

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
      if (rightController.GetStickButtonDown() || leftController.GetStickButtonDown())
      {
         DoReload();
      }
      if (Mathf.Abs(rightController.GetStickPosition().x) > snapTurnDeadZone)
      {
         DoSnapTurn(rightController.GetStickPosition().x);
      }
      if (Mathf.Abs(leftController.GetStickPosition().x) > snapTurnDeadZone)
      {
         DoSnapTurn(leftController.GetStickPosition().x);
      }
      if (snapTurnDebounce > 0f)
      {
         snapTurnDebounce -= Time.deltaTime;
      }

  }

   void DoSnapTurn(float inputX)
   {
      Debug.Log(snapTurnDebounce);
      if (snapTurnDebounce <= 0f)
      {
         transform.Rotate(0, Mathf.Sign(inputX)*45, 0);
         snapTurnDebounce = snapTurnDebounceTimeSeconds;
      } else
      {
         snapTurnDebounce -= Time.deltaTime;
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
   void DoReload()
   {
      Scene scene = SceneManager.GetActiveScene();
      SceneManager.LoadScene(scene.name);
   }
}
