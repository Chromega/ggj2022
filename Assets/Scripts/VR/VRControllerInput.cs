using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.Oculus;

public class VRControllerInput : MonoBehaviour
{
   public enum Side
   {
      Right,
      Left
   }

   public Side side;
   private List<InputDevice> qualifyingDevices;

   bool primaryButtonDown;
   bool lastPrimaryButtonDown;

   bool secondaryButtonDown;
   bool lastSecondaryButtonDown;

   bool stickDown;
   bool lastStickDown;

   private void Awake()
   {
      qualifyingDevices = new List<InputDevice>();
   }

   void OnEnable()
   {
      List<InputDevice> allDevices = new List<InputDevice>();
      InputDevices.GetDevices(allDevices);
      foreach (InputDevice device in allDevices)
         InputDevices_deviceConnected(device);

      InputDevices.deviceConnected += InputDevices_deviceConnected;
      InputDevices.deviceDisconnected += InputDevices_deviceDisconnected;
   }

   private void OnDisable()
   {
      InputDevices.deviceConnected -= InputDevices_deviceConnected;
      InputDevices.deviceDisconnected -= InputDevices_deviceDisconnected;
      qualifyingDevices.Clear();
   }

   private void InputDevices_deviceConnected(InputDevice device)
   {
      if (device.isValid)
      {
         if ((device.characteristics & InputDeviceCharacteristics.HeldInHand) != 0)
         {
            if (side == Side.Left && (device.characteristics & InputDeviceCharacteristics.Left) != 0)
            {
               qualifyingDevices.Add(device);
            }
            if (side == Side.Right && (device.characteristics & InputDeviceCharacteristics.Right) != 0)
            {
               qualifyingDevices.Add(device);
            }
         }
      }
   }

   private void InputDevices_deviceDisconnected(InputDevice device)
   {
      if (qualifyingDevices.Contains(device))
         qualifyingDevices.Remove(device);
   }

   public float GetGrip()
   {
      foreach (var device in qualifyingDevices)
      {
         float grip;
         device.TryGetFeatureValue(CommonUsages.grip, out grip);
         return grip;
      }
      return 0f;
   }

   public float GetTrigger()
   {
      foreach (var device in qualifyingDevices)
      {
         float trigger;
         device.TryGetFeatureValue(CommonUsages.trigger, out trigger);
         return trigger;
      }
      return 0f;
   }

   public bool GetThumbTouch()
   {
      foreach (var device in qualifyingDevices)
      {
         bool thumbTouchStick;
         device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out thumbTouchStick);
         bool thumbTouchA;
         device.TryGetFeatureValue(CommonUsages.primaryTouch, out thumbTouchA);
         bool thumbTouchB;
         device.TryGetFeatureValue(CommonUsages.secondaryTouch, out thumbTouchB);

         bool thumbTouch = thumbTouchStick || thumbTouchA || thumbTouchB;
         return thumbTouch;
      }
      return false;
   }

   bool GetButtonPress(InputFeatureUsage<bool> usage)
   {
      foreach (var device in qualifyingDevices)
      {
         bool button;
         device.TryGetFeatureValue(usage, out button);
         return button;
      }
      return false;
   }


   public void DoHapticImpulse(float amplitude, float durationSeconds)
   {
      foreach (var device in qualifyingDevices)
      {
         device.SendHapticImpulse(0, amplitude, durationSeconds);
      }
   }

   public Vector2 GetStickPosition()
   {
      Vector2 stickPos = Vector2.zero;
      foreach (var device in qualifyingDevices)
      {
         Vector2 thisStickPos;
         device.TryGetFeatureValue(CommonUsages.primary2DAxis, out thisStickPos);
         stickPos += thisStickPos;
      }
      return stickPos;
   }

   private void Update()
   {
      lastPrimaryButtonDown = primaryButtonDown;
      primaryButtonDown = GetButtonPress(CommonUsages.primaryButton);

      lastSecondaryButtonDown = secondaryButtonDown;
      secondaryButtonDown = GetButtonPress(CommonUsages.secondaryButton);

      lastStickDown = stickDown;
      stickDown = GetButtonPress(CommonUsages.primary2DAxisClick);
   }

   public bool GetPrimaryButton()
   {
      return primaryButtonDown;
   }

   public bool GetPrimaryButtonDown()
   {
      return primaryButtonDown && !lastPrimaryButtonDown;
   }

   public bool GetPrimaryButtonUp()
   {
      return !primaryButtonDown && lastPrimaryButtonDown;
   }

   public bool GetSecondaryButton()
   {
      return secondaryButtonDown;
   }

   public bool GetSecondaryButtonDown()
   {
      return secondaryButtonDown && !lastSecondaryButtonDown;
   }

   public bool GetSecondaryButtonUp()
   {
      return !secondaryButtonDown && lastSecondaryButtonDown;
   }

   public bool GetStickButton()
   {
      return stickDown;
   }

   public bool GetStickButtonDown()
   {
      return stickDown && !lastStickDown;
   }

   public bool GetStickButtonUp()
   {
      return !stickDown && lastStickDown;
   }
}
