using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wormhole : MonoBehaviour
{

    Coroutine releasedCoroutine;

    public ParticleSystem warpFX;
    public Wormhole warpLocation;
    public bool canWarp = true;
    public Vector3 scaleChange;
    public Vector3 originalScale;
    public AudioClip sfxWarp;
    void Start() {
        originalScale = gameObject.transform.localScale;
        scaleChange = new Vector3((originalScale.x)*-0.00025f, (originalScale.y)*-0.00025f, (originalScale.z)*-0.00025f);
    }

    void Update() {

        //print(gameObject.name);
        gameObject.transform.localScale += scaleChange;
        gameObject.transform.Rotate(0.0f, 0.0f, 0.05f);

        if (gameObject.transform.localScale.y < (originalScale.y * (0.75f)) || gameObject.transform.localScale.y > (originalScale.y * (1.0f)))
        {
            scaleChange =- scaleChange;
        }
    }

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
        GetComponent<AudioSource>().PlayOneShot(sfxWarp);
   }

}
