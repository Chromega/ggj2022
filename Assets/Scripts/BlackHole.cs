using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{

    Coroutine releasedCoroutine;
    
    private void OnTriggerEnter(Collider hit) {
        if ((hit.gameObject.layer == 6)) {
            hit.gameObject.transform.position = this.transform.position;
            hit.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            releasedCoroutine = StartCoroutine(waitTime());
            hit.GetComponent<Rigidbody>().velocity = new Vector3((Random.Range(-1, 1)*100), (Random.Range(-1, 1)*100), (Random.Range(-1, 1)*100));
        }   
    }

    private void OnTriggerExit() {
        if (releasedCoroutine != null)
      {
         StopCoroutine(releasedCoroutine);
         releasedCoroutine = null;
      }
    }

    IEnumerator waitTime()
   {
       yield return new WaitForSeconds(5.0f);
   }

}
