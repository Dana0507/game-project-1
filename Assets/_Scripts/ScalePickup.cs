using UnityEngine;

namespace _Scripts
{
    public class ScalePickup : MonoBehaviour
    {
        [SerializeField] private float scaleAmount = 0.15f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerScaling scaling = other.GetComponent<PlayerScaling>();

            if (scaling == null) return;

            scaling.Grow(scaleAmount);
            Destroy(gameObject);
        }
    }
}