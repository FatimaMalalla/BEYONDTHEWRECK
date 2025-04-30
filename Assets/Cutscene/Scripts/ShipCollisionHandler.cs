using UnityEngine;

public class ShipCollisionHandler : MonoBehaviour
{
    public AudioSource crashSound;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.name);
        if (other.CompareTag("Rock"))
        {
            crashSound.Play();
            StartCoroutine(FindObjectOfType<CameraShake>().Shake(0.5f, 0.3f));
        }
    }

}
