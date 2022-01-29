using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wormhole : MonoBehaviour
{

    public ParticleSystem warpFX;
    public Wormhole warpLocation;
    public bool canWarp = true;

    private void OnTriggerEnter(Collider hit) {
        if ((hit.gameObject.layer == 6) && (canWarp == true)) {
            //Debug.Log("test");
            hit.gameObject.transform.position = warpLocation.transform.position;
            warpLocation.canWarp = false;
        }   
    }

    private void OnTriggerExit() {
        this.canWarp = true;
        warpFX.Play();
    }

}
