using UnityEngine;
using _Scripts.Interfaces;
using _Scripts;
using _Scripts.Misc_;

namespace _Scripts.Misc_
{
    public class EnemyAttackBox : MonoBehaviour
    {
        [SerializeField] private BasicEnemy enemy;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Enemy"))
            {
                Debug.Log("Player detected in attack box");
                enemy.DealDamage(other.gameObject);
            }
        }
    }
}
