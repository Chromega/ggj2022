using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{

    Coroutine releasedCoroutine;

    private void OnTriggerEnter(Collider hit) {
        if ((hit.gameObject.layer == 6)) {
            hit.gameObject.transform.position = this.transform.position;
            hit.attachedRigidbody.velocity = new Vector3(0, 0, 0);
            releasedCoroutine = StartCoroutine(waitTime(hit.attachedRigidbody));
        }
    }

    private void OnTriggerExit() {
        if (releasedCoroutine != null)
            {
                StopCoroutine(releasedCoroutine);
                releasedCoroutine = null;
            }
    }

    IEnumerator waitTime(Rigidbody hitRigid)
   {
       yield return new WaitForSeconds(4.0f);

       hitRigid.velocity = new Vector3((Random.Range(-1, 1)*100), (Random.Range(-1, 1)*100), (Random.Range(-1, 1)*100));
   }

}
