using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.Oculus;

public class HandAnimator : MonoBehaviour
{
   public VRControllerInput controllerInput;
   public Animator animator;

   private float lastThumb;

   void Update()
   {
      float grip = controllerInput.GetGrip();
      grip = Mathf.Lerp(.5f, 1f, grip);

      bool thumbTouch = controllerInput.GetThumbTouch();

      float index = controllerInput.GetTrigger();
      //bool indexTouch;
      //Debug.Log(device.TryGetFeatureValue(OculusUsages.indexTouch, out indexTouch));
      //index = indexTouch ? 1 : 0;
      index = Mathf.Lerp(.5f, 1f, index);

      lastThumb = Mathf.MoveTowards(lastThumb, thumbTouch ? 1 : 0, 8f * Time.deltaTime);

      animator.SetFloat("Thumb", lastThumb);
      animator.SetFloat("Index", index);
      animator.SetFloat("Grip", grip);
   }
}
