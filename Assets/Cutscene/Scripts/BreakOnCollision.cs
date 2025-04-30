using UnityEngine;

public class BreakOnCollision : MonoBehaviour
{
    public GameObject[] breakPieces;
    public AudioSource crashSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rock"))
        {
            if (crashSound != null) crashSound.Play();

            foreach (GameObject piece in breakPieces)
            {
                piece.SetActive(true);
                piece.transform.parent = null;
                Rigidbody rb = piece.AddComponent<Rigidbody>();
                rb.AddExplosionForce(300, transform.position, 5);
            }

            Destroy(gameObject); // Or setActive(false)
        }
    }
}
