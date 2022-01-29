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

   public GameObject playerVR;
   public GameObject playerNonVR;
   bool isVREnabled = false;

   // Start is called before the first frame update
   void Start()
   {
      Debug.Log("start");

      // Enable VR or non-VR modes
      var inputDevices = new List<UnityEngine.XR.InputDevice>();
      UnityEngine.XR.InputDevices.GetDevices(inputDevices);

      if (inputDevices.Count == 0)
      {
         isVREnabled = false;
         
      } else
      {
         isVREnabled = true;
      }

      if (isVREnabled)
      {
         // no VR headset
         GameObject.Find("PlayerNonVR").SetActive(false);
      } else
      {
         // no VR headset
         GameObject.Find("PlayerVR").SetActive(false);
      }
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
