using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubRack : MonoBehaviour
{
   public GolfClub[] clubs;

   Dictionary<GolfClub, Vector3> clubLocalPosition = new Dictionary<GolfClub, Vector3>();
   Dictionary<GolfClub, Quaternion> clubLocalRotation = new Dictionary<GolfClub, Quaternion>();

   // Start is called before the first frame update
   void Start()
   {
      foreach (GolfClub club in clubs)
      {
         club.clubRack = this;
         clubLocalPosition[club] = club.transform.localPosition;
         clubLocalRotation[club] = club.transform.localRotation;
         club.transform.parent = null;
      }
   }

   public Vector3 GetClubWorldPosition(GolfClub club)
   {
      return transform.TransformPoint(clubLocalPosition[club]);
   }

   public Quaternion GetClubWorldRotation(GolfClub club)
   {
      return transform.rotation * clubLocalRotation[club];
   }

   private void Update()
   {
      Vector3 pos = transform.parent.InverseTransformPoint(Camera.main.transform.position);
      pos.y = 0;

      transform.localPosition = pos;

      Vector3 facing = -transform.parent.InverseTransformDirection(Camera.main.transform.forward);
      facing.y = 0;
      transform.localRotation = Quaternion.LookRotation(facing.normalized, Vector3.up);
   }
}
