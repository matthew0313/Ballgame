using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] AudioSource bounceAudio;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(Mathf.Abs(collision.relativeVelocity.x) > 5.0f) bounceAudio.Play();
    }
}
