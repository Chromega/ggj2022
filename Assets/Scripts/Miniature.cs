using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miniature : Pickupable
{
   [System.Serializable]
   public class MaterialSubstitution
   {
      public Material original;
      public Material substitute;
   }

   Planetoid currentPlanetoid = null;

   public GameObject playerIcon;
   Planetoid planetoidCopy;
   GolfBall golfBallCopy;

   public MaterialSubstitution[] materialSubstitutions;

   public TMPro.TextMeshProUGUI planetProgressText;

   protected override void Start()
   {
      base.Start();
      golfBallCopy = Instantiate(GameMgr.Instance.golfBall.gameObject).GetComponent<GolfBall>();
      golfBallCopy.transform.parent = transform;
      golfBallCopy.GetComponent<Collider>().enabled = false;
      golfBallCopy.GetComponent<Rigidbody>().isKinematic = true;
      golfBallCopy.onFireFx.gameObject.SetActive(false);
      golfBallCopy.trail.enabled = false;
   }

   protected override void Update()
   {
      UpdatePlanetoid(GameMgr.Instance.golfBall.closestPlanetoid);

      if (!currentPlanetoid)
         return;

      Vector3 golfBallWorldPos = GameMgr.Instance.golfBall.transform.position;
      Vector3 golfBallPlanetoidPos = currentPlanetoid.transform.InverseTransformPoint(golfBallWorldPos);
      golfBallCopy.transform.localPosition = golfBallPlanetoidPos;


      Vector3 playerWorldPos = GameMgr.Instance.player.transform.position;
      //Quaternion playerWorldRot = GameMgr.Instance.player.transform.transform.rotation;
      Quaternion playerWorldRot = Camera.main.transform.transform.rotation;
      Vector3 playerPlanetoidPos = currentPlanetoid.transform.InverseTransformPoint(playerWorldPos);
      Quaternion playerPlanetoidRot = Quaternion.Inverse(currentPlanetoid.transform.rotation) * playerWorldRot;
      playerIcon.transform.localPosition = playerPlanetoidPos;
      playerIcon.transform.localRotation = playerPlanetoidRot;

      int numGoals;
      int numCollectedGoals;
      currentPlanetoid.GetGoalProgress(out numCollectedGoals, out numGoals);
      planetProgressText.text = numCollectedGoals + "/" + numGoals;

      base.Update();
   }

   void UpdatePlanetoid(Planetoid planetoid, bool force=false)
   {
      if (!planetoid)
         return;

      if (planetoid == currentPlanetoid && !force)
         return;

      currentPlanetoid = planetoid;

      playerIcon.transform.parent = transform;
      golfBallCopy.transform.parent = transform;
      if (planetoidCopy)
      {
         planetoidCopy.OnGoalCollected -= OnGoalCollected;
         Destroy(planetoidCopy.gameObject);
      }

      planetoidCopy = Instantiate(planetoid.gameObject).GetComponent<Planetoid>();
      planetoidCopy.isMiniature = true;
      planetoidCopy.transform.parent = transform;
      planetoidCopy.transform.localPosition = Vector3.zero;
      planetoidCopy.transform.localRotation = Quaternion.identity;
      planetoidCopy.transform.localScale = .02f*Vector3.one;



      foreach (MeshRenderer r in planetoidCopy.GetComponentsInChildren<MeshRenderer>())
      {
         //Gross!  but want to skip reskinning goals
         if (r.name == "White_Dwarf")
            continue;
         foreach (MaterialSubstitution ms in materialSubstitutions)
         {
            if (r.sharedMaterial = ms.original)
               r.sharedMaterial = ms.substitute;
         }
      }

      foreach (Rigidbody rb in planetoidCopy.GetComponentsInChildren<Rigidbody>())
      {
         rb.isKinematic = true;
      }
      foreach (Collider c in planetoidCopy.GetComponentsInChildren<Collider>())
      {
         c.enabled = false;
      }
      planetoidCopy.GetComponent<Collider>().enabled = true;
      planetoidCopy.gameObject.layer = 9; //only grabbable

      golfBallCopy.transform.parent = planetoidCopy.transform;
      golfBallCopy.transform.localScale = 5*Vector3.one;
      playerIcon.transform.parent = planetoidCopy.transform;

      planetoid.OnGoalCollected += OnGoalCollected;
   }

   void OnGoalCollected()
   {
      UpdatePlanetoid(currentPlanetoid, true);
   }

   public void PlayerTransformUpdated()
   {
      transform.rotation = currentPlanetoid.transform.rotation;
   }

}
