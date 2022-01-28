using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGrabber : Grabber
{
   public bool log;
   bool alternate = false;
   protected override float GetGrab()
   {
      alternate = !alternate;
      if (currentlyHeldGrabbable)
         return 1.0f;
      else
         return alternate ? 0.0f : 1.0f;
   }
}
