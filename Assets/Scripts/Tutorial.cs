using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
   public Planetoid startingPlanetoid;

   public MeshRenderer skybox;
   public Material substituteMaterial;

   // Start is called before the first frame update
   void Start()
   {
      startingPlanetoid.OnGoalCollected += OnGoalCollected;
   }

   void OnGoalCollected()
   {
      StartCoroutine(GoalCollectedCoroutine());
   }

   IEnumerator GoalCollectedCoroutine()
   {
      skybox.sharedMaterial = substituteMaterial;

      const float kTotalTime = 1.0f;
      float time = 0;
      while (time < kTotalTime)
      {
         skybox.material.SetFloat("_Alpha", 1.0f - time / kTotalTime);
         yield return null;
         time += Time.deltaTime;
      }
      skybox.gameObject.SetActive(false);
   }
}
