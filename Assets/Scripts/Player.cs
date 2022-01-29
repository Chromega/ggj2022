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

   Vector3 ballOffset = new Vector3(.5f, 0f, 0f);

   // Start is called before the first frame update
   void Start()
   {
   }

   // Update is called once per frame
   void Update()
   {
      if (rightController.GetPrimaryButtonDown() || leftController.GetPrimaryButtonDown() ||
         Input.GetMouseButtonDown(0))
      {
         DoTeleport();
      }
      if (rightController.GetSecondaryButtonDown() || leftController.GetSecondaryButtonDown() ||
         Input.GetMouseButtonDown(1))
      {
         DoRecallBall();
      }
      if (rightController.GetStickButtonDown() || leftController.GetStickButtonDown() ||
         Input.GetKeyDown(KeyCode.R))
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
      if (Input.GetKeyDown(KeyCode.LeftArrow))
      {
         DoSnapTurn(-1);
         snapTurnDebounce = 0;
      }
      if (Input.GetKeyDown(KeyCode.RightArrow))
      {
         DoSnapTurn(1);
         snapTurnDebounce = 0;
      }
      if (snapTurnDebounce > 0f)
      {
         snapTurnDebounce -= Time.deltaTime;
      }

  }

   void DoSnapTurn(float inputX)
   {
      if (snapTurnDebounce <= 0f)
      {
         transform.Rotate(0, Mathf.Sign(inputX)*45, 0);

         Vector3 offsetFromBall = transform.TransformVector(-ballOffset);
         transform.position = GameMgr.Instance.golfBall.transform.position + offsetFromBall;

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
      Vector3 relativePosition = -ballOffset;

      transform.rotation = facing;
      transform.position = ballPosition + facing * relativePosition;
   }
   void DoRecallBall()
   {
      GolfBall ball = GameMgr.Instance.golfBall;
      Rigidbody rb = ball.GetComponent<Rigidbody>();
      Vector3 relativePosition = ballOffset + .1f * Vector3.up;
      Vector3 worldPosition = transform.TransformPoint(relativePosition);

      rb.position = worldPosition;
      rb.velocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;

      ball.Reset();
   }
   void DoReload()
   {
      Scene scene = SceneManager.GetActiveScene();
      SceneManager.LoadScene(scene.name);
   }
}
