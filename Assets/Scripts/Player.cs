using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
   public VRControllerInput leftController;
   public VRControllerInput rightController;

   public Miniature miniature;

   public float snapTurnDeadZone = 0.125f;
   public float snapTurnDebounceTimeSeconds = 1f;
   float snapTurnDebounce = 0f;

   Vector3 ballOffset = new Vector3(.5f, 0f, 0f);

   public ParticleSystem dustExplosion;

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
         Vector3 currentBallSource = transform.TransformPoint(ballOffset);

         transform.Rotate(0, Mathf.Sign(inputX) * 45, 0);

         Vector3 offsetFromBall = transform.TransformVector(-ballOffset);
         transform.position = currentBallSource + offsetFromBall;

         snapTurnDebounce = snapTurnDebounceTimeSeconds;
         miniature.PlayerTransformUpdated();
      } else
      {
         snapTurnDebounce -= Time.deltaTime;
      }
   }

   void DoTeleport()
   {
      GolfBall ball = GameMgr.Instance.golfBall;
      Vector3 ballPosition = ball.transform.position;
      Vector3 normal = ball.closestPlanetoid.gravitySource.ComputePlayerNormal(ballPosition);

      Quaternion facing = Quaternion.FromToRotation(Vector3.up, normal);
      Vector3 relativePosition = -ballOffset;

      transform.rotation = facing;
      transform.position = ballPosition + facing * relativePosition;

      // Make sure to move the ghost ball with the player
      GameMgr.Instance.ReturnGhostBall();

      miniature.PlayerTransformUpdated();
   }
   void DoRecallBall()
   {
      GolfBall ball = GameMgr.Instance.golfBall;
      Rigidbody rb = ball.GetComponent<Rigidbody>();

      Vector3 worldPosition = GetBallStartPosition();

      rb.position = worldPosition;
      rb.velocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;

      ball.Reset(); // sets active to be true
      GameMgr.Instance.GetGhostBall().gameObject.SetActive(false); // disable ghost ball
   }
   void DoReload()
   {
      Scene scene = SceneManager.GetActiveScene();
      SceneManager.LoadScene(scene.name);
   }

   public Vector3 GetBallStartPosition()
   {
      //Vector3 relativePosition = ballOffset + .1f * Vector3.up; // needed when we had gravity
      Vector3 relativePosition = ballOffset;
      Vector3 worldPosition = transform.TransformPoint(relativePosition);
      return worldPosition;
   }

   public void DustEffect()
   {
      // spawn the dust explosion
      dustExplosion.transform.position = GameMgr.Instance.golfBall.GetComponent<Rigidbody>().position;
      dustExplosion.Play();
   }
}
