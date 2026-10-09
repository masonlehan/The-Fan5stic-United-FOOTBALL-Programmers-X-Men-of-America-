using UnityEngine;

public class SoundEffectScript : MonoBehaviour
{
    
    AudioSource audioPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

   public void Play(AudioClip ac)
   {
    audioPlayer.PlayOneShot(ac);
   }
}