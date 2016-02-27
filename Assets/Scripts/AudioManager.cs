using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    public AudioSource audio;
    public AudioClip prepearingStageClip;
    public AudioClip battleStageClip;

    public void PrepearingStage()
    {
        audio.Stop();
        audio.clip =  prepearingStageClip;
        audio.Play();
    }
    public void BattleStage()
    {
        audio.Stop();
        audio.clip =  battleStageClip;
        audio.Play();
    }
}
