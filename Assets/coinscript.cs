using UnityEngine;

public class coinscript : MonoBehaviour
{
    public AudioClip CoinSoundEffect;
    SoundEffectScript soundPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      GameObject sp_go = GameObject.Find("SoundEffectPlayer");
      soundPlayer = sp_go.GetComponent<SoundEffectScript>();
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

void OnTriggerEnter2D ()
{
    soundPlayer.Play(CoinSoundEffect);

    GameObject.Destroy(gameObject);
}



}
