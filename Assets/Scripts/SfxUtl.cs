using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioClipPool
{
   public AudioClip[] clips;
   int lastClipIdx = -1;

   public AudioClip GetClip()
   {
      if (clips.Length > 1)
      {
         int idx = Random.Range(0, clips.Length);
         while (idx == lastClipIdx)
         {
            idx = Random.Range(0, clips.Length);
         }
         lastClipIdx = idx;
         return clips[lastClipIdx];
      }
      else
      {
         return clips[0];
      }
   }

}