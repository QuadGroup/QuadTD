using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

    public AudioSource audio;
    public AudioClip hitClip;
    public AudioClip deathClip;

    public void Hit()
    {
        audio.clip = hitClip;
        audio.Play();
    }
    public void Death()
    {
        //audio.clip = deathClip;
        audio.PlayOneShot(deathClip);
    }
}
