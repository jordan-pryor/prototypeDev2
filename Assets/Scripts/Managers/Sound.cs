using UnityEngine;
public class Sound : MonoBehaviour
{
    public float lifeTime = 0.5f;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}