using UnityEngine;

public class DestroyBulletsOnCollision : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Bullet"))
        {
            GameObject.Destroy(other.gameObject);
        }
    }
}
