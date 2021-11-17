using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFX : MonoBehaviour
{

    public AudioSource takeSound;

    // Plays the sound for taking over an octagon
    public void take()
    {
        takeSound.Play();
    }

}
