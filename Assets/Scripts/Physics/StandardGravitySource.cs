using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StandardGravitySource : GravitySource
{
   protected override void Start()
   {
      Physics.gravity = new Vector3(0, -9.81f, 0);
   }

   protected override Vector3 ComputeForce(Rigidbody rb)
   {
      throw new NotImplementedException();
   }
}
