using UnityEngine;
using _Scripts.Interfaces;
using _Scripts;

namespace _Scripts.Misc_
{
    public class EnemyAttackBox : MonoBehaviour
    {
        [SerializeField] private BasicEnemy enemy;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                enemy.HandleAttack(other.gameObject);
            }
        }
    }
}
