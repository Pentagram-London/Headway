using UnityEngine;
using System.Collections;

public class FootStepSFX : MonoBehaviour {

    public AudioSource footStepSource;
    public AudioClip[] footStepSFX;

    public mCharacterController character;

    // This is triggered from events within the animation itself, placed in sync with the animation's footfalls.
    public void Play()
    {
        /*
         *  Audio files used are not avaliable in the public domain, so I've disabled this code for the source distribution.
         * 
        if (character.walkAnimBlend > 0.1)
            footStepSource.PlayOneShot(footStepSFX[Random.Range(0, footStepSFX.Length - 1)], 1.0f);
        */

    }

}
