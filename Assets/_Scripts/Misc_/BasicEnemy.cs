using _Scripts.Interfaces;
using _Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Misc_
{
    public class BasicEnemy : MonoBehaviour, IDamageable
    {
        [SerializeField] private EnemyData enemyData;
        [SerializeField] private GameObject bluePotionPrefab;
        [SerializeField] private GameObject redPotionPrefab;
        [SerializeField] private ParticleSystem deathParticles;
		[SerializeField] private GameObject attackBox;
        [SerializeField] private AudioClip footstepSound;
        [SerializeField] private AudioClip attackSound;
        private AudioSource _audioSource;
        private int _currHealth;
        private bool _isDead;
        [SerializeField] private Transform groundWallCheck;
        private bool _isFacingRight;
        public EnemyType enemyType;


        #region Components

        private Animator _anim;
        private Rigidbody2D _rb;
        private BoxCollider2D _collider;

        #endregion

        #region Animation Hashes

        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int Death = Animator.StringToHash("Death");

        #endregion

        #region Life Cycle Functions

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _currHealth = enemyData.health;
        }

        private void FixedUpdate()
        {
            _rb.velocity =
                new Vector2(_isFacingRight ? Mathf.Abs(enemyData.moveSpeed) : -Mathf.Abs(enemyData.moveSpeed),
                    _rb.velocity.y);

            // Wall Detection Raycast
            var wallDetect = Physics2D.Raycast(groundWallCheck.position, _isFacingRight ? Vector2.right : Vector2.left,
                enemyData.wallDetectionDistance, enemyData.groundLayer);

            // Edge Detection Raycast
            var edgeDetect = Physics2D.Raycast(groundWallCheck.position, Vector2.down,
                enemyData.edgeDetectionDistance, enemyData.groundLayer);

            if (wallDetect || !edgeDetect)
            {
                FlipCharacter();
            }
        }

        #endregion

        // flip logic
        private void FlipCharacter()
        {
            _isFacingRight = !_isFacingRight;
            var characterScale = transform.localScale;

            if (!_isFacingRight)
            {
                characterScale.x = -Mathf.Abs(transform.localScale.x);
            }
            else if (_isFacingRight)
            {
                characterScale.x = Mathf.Abs(transform.localScale.x);
            }

            transform.localScale = characterScale;
        }

		public void EnableAttackBox()
		{
    		attackBox.SetActive(true);
		}

		public void DisableAttackBox()
		{
    		attackBox.SetActive(false);
		}

        public void ChangeHealth(int amount)
        {
            // Restrict the changed health value between 0 and "maxHealth"
            _currHealth = Mathf.Clamp(_currHealth + amount, 0, enemyData.health);

            // If we're decreasing health and enemy health still hasn't reached 0, play the hit animation
            if (amount <= 0 && _currHealth != 0)
            {
                _anim.SetTrigger(Hit);
            }

            // If enemy's health reached 0, trigger the death animation
            if (_currHealth == 0)
            {
                EnemyDeath();
            }
        }
		/*
		public void HandleAttack(GameObject player)
		{
    		if (_isDead) return;

    		var playerController = player.GetComponent<PlayerController>();
    		var damageable = player.GetComponent<IDamageable>();

    		if (playerController != null && playerController.isShielding)
    		{
        		// Shield hit
        		_audioSource.PlayOneShot(enemyData.attackShieldSound);
    		}
    		else
    		{
        		// Flesh hit
        		_audioSource.PlayOneShot(enemyData.attackFleshSound);
        		if (damageable != null)
        		{
            		damageable.ChangeHealth(-1);
        		}
    		}
		}
		*/
        private void EnemyDeath()
        {
            _isDead = true;
            _anim.SetTrigger(Death);
            _rb.bodyType = RigidbodyType2D.Static; // Set enemy rigidbody to static so it doesn't get pushed by player
            _collider.isTrigger = true; // set enemy's collider to a trigger, allowing player to walk over the dead body
            if (deathParticles != null)
            {
                Instantiate(deathParticles, transform.position, Quaternion.identity); // smoke particle system
            }
            SpawnPotion(); // potion spawned
            Destroy(gameObject, 1f); // enemy's body is destroyed
        }
        
        private void SpawnPotion()
        {
            switch(enemyData.enemyType)
            {
                case EnemyType.Tank:
                    Instantiate(bluePotionPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity); // blue potion is spawned if enemy is tanked
                    break;

                case EnemyType.Fast:
                    Instantiate(redPotionPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity); // red potion is spawned if enemy is fast
                    break;
            }
        }
        
        #region Audio Functions

        // Called by animation event
        public void PlayFootstep()
        {
            if (footstepSound != null)
            {
                _audioSource.PlayOneShot(footstepSound);
            }
        }
     

        #endregion

        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.white;
        //     Gizmos.DrawRay(groundWallCheck.position, Vector2.down * enemyData.edgeDetectionDistance);
        //     Gizmos.DrawRay(groundWallCheck.position, _isFacingRight? Vector2.right : Vector2.left * enemyData.wallDetectionDistance);
        // }
    }
}