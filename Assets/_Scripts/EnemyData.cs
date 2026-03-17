using UnityEngine;

namespace _Scripts
{
    public enum EnemyType
    {
        Base,
        Fast,
        Tank
    }
    
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public int health;
        public float moveSpeed;
		public int attackDamage;
		public float attackCooldown;
        public float edgeDetectionDistance;
        public LayerMask groundLayer;
        public EnemyType enemyType;
		
		// attack sound effects
		// public AudioClip attackFleshSound;
    	// public AudioClip attackShieldSound;
    }
}

