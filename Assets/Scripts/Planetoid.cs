using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planetoid : MonoBehaviour
{
   public GravitySource gravitySource;

   private void Awake()
   {
      gameObject.layer = 8; //Planetoid
   }
}
