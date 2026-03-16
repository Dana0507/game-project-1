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
        public int damage;
        public float moveSpeed;
        public float edgeDetectionDistance;
        public float wallDetectionDistance;
        public LayerMask groundLayer;
        public EnemyType enemyType;
		
		// attack sound effects
		public AudioClip attackFleshSound;
    	public AudioClip attackShieldSound;
    }
}

