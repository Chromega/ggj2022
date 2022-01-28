using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Grabbable : MonoBehaviour
{
   // inherit from this class if you're a grabbable object
   bool isGrabbable = true;

   protected HashSet<Grabber> grabbers = new HashSet<Grabber>();
   //protected Outline outline = null;
   public abstract Vector3 GetGrabPosition();
   public abstract bool ConstrainsPosition();

   public HashSet<Grabber> grabbersWeAreTheBestFor = new HashSet<Grabber>();
   
   public int maxNumGrabbers = 1;

   /*protected override void Awake()
   {
      base.Awake();


      outline = gameObject.GetComponent<Outline>();
      if (!outline)
         outline = gameObject.AddComponent<Outline>();
   }*/

   /*protected virtual void Start()
   {
      Resetable r = GetComponent<Resetable>();
      if (r)
      {
         r.OnReset += R_OnReset;
      }

      outline.OutlineWidth = 25f;
      outline.OutlineMode = Outline.Mode.OutlineVisible;
      outline.enabled = false;
   }*/

   public bool HasGrabber()
   {
      //return grabber != null;
      return grabbers.Count > 0;
   }

   public virtual void AddGrabber(Grabber grabber)
   {
      grabbers.Add(grabber);
   }

   public virtual void RemoveGrabber(Grabber grabber)
   {
      grabbers.Remove(grabber);
   }

   public virtual bool CanBeGrabbed()
   {
      return isGrabbable;
   }

   public bool CanAcceptMoreGrabbers()
   {
      return grabbers.Count < maxNumGrabbers;
   }

   public virtual void SetCanBeGrabbed(bool b)
   {
      if (isGrabbable == b)
         return;
      isGrabbable = b;

      if (!isGrabbable && HasGrabber())
      {
         foreach (Grabber g in grabbers)
            g.Release();
      }
   }

   public void SetIsBestGrabbable(bool isBestGrabbable, Grabber grabber)
   {
      if (isBestGrabbable)
      {
         grabbersWeAreTheBestFor.Add(grabber);
      }
      else
      {
         grabbersWeAreTheBestFor.Remove(grabber);
      }

      Color color = Color.black;
      foreach (Grabber g in grabbersWeAreTheBestFor)
      {
         color += g.grabbableColor;
      }
      /*
      outline.OutlineColor = color;
      outline.enabled = grabbersWeAreTheBestFor.Count > 0;*/
   }
}
