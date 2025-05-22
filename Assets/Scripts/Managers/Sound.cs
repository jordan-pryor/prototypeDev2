using UnityEngine;
public class Sound : MonoBehaviour
{
    public float lifeTime = 0.5f;             // Time before the sound object is destroyed
    public AudioClip[] audios;               // List of possible audio clips
    public AudioSource source;               // Audio source component used to play sound

    void Start()
    {
        if (audios.Length > 0)
        {
            // Select and play a random audio clip
            source.clip = audios[Random.Range(0, audios.Length)];
            source.Play();
        }

        // Destroy this game object after its lifetime
        Destroy(gameObject, lifeTime);
    }
}