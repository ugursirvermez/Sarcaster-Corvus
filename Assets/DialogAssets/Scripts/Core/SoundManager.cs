using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager örnek { get; private set; }

    private AudioSource ses;

    private void Awake()
    {
        örnek = this;
        ses = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip sound)
    {
        ses.PlayOneShot(sound);
    }
}
