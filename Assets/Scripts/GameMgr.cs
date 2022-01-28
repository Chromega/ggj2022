using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
   public static GameMgr Instance { get; private set; }

   public GolfBall golfBall;

   private void Awake()
   {
      Instance = this;
   }

   private void OnDestroy()
   {
      if (Instance == this)
         Instance = null;
   }
}