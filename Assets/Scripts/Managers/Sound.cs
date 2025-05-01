using UnityEngine;
public class Sound : MonoBehaviour
{
    public float lifeTime = 0.5f;
    public AudioClip[] audios;
    public AudioSource source;
    void Start()
    {
        if (audios.Length > 0)
        {
            source.clip = audios[Random.Range(0, audios.Length)];
            source.Play();
        }
        Destroy(gameObject, lifeTime);
    }
}