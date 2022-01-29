using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerVRToggle : MonoBehaviour
{
   public GameObject playerVR;
   public GameObject playerNonVR;
   bool isVREnabled = false;

   // Start is called before the first frame update
   void Start()
   {
      // Enable VR or non-VR modes
      var inputDevices = new List<UnityEngine.XR.InputDevice>();
      UnityEngine.XR.InputDevices.GetDevices(inputDevices);

      if (inputDevices.Count == 0)
      {
         isVREnabled = false;
         
      } else
      {
         isVREnabled = true;
      }
      Debug.Log(isVREnabled);
      if (isVREnabled)
      {
         // no VR headset
         GameObject.Find("PlayerNonVR").SetActive(false);
      } else
      {
         // no VR headset
         GameObject.Find("PlayerVR").SetActive(false);
      }
   }

   // Update is called once per frame
   void Update()
   {
   }
}
