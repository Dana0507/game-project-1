using _Scripts.Interfaces;
using _Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Misc_
{
    public class BasicEnemy : MonoBehaviour, IDamageable
    {
        [SerializeField] private EnemyData enemyData;

		[Header("Attacking")]
		[SerializeField] private GameObject attackBox;
		[SerializeField] private float attackCooldown = 1.5f;
		private float _lastAttackTime;
		[SerializeField] private float attackRange = 1.5f;
		private Transform _target;

		[Header("Potions")]
        [SerializeField] private GameObject bluePotionPrefab;
        [SerializeField] private GameObject redPotionPrefab;

		[Header("Effects")]
        [SerializeField] private ParticleSystem deathParticles;
		
		[Header("Audio")]
        [SerializeField] private AudioClip footstepSound;
        [SerializeField] private AudioClip attackSound;
        private AudioSource _audioSource;
		
		[Header("Enemy Stats")]
        private int _currHealth;
        private bool _isDead;
		private bool _isAttacking;
        [SerializeField] private Transform groundWallCheck;
        private bool _isFacingRight;
        public EnemyType enemyType;
		[SerializeField] private float detectionRange = 2f;
		[SerializeField] private LayerMask playerLayer;

		[Header("Enemy Patrol")]
		[SerializeField] private float patrolDistance = 5f;
		private Vector2 _startPosition;
		private float _lastFlipTime;
		[SerializeField] private float flipCooldown = 0.2f;
		
        #region Components

        private Animator _anim;
        private Rigidbody2D _rb;
        private BoxCollider2D _collider;

        #endregion

        #region Animation Hashes

		private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Hurt = Animator.StringToHash("Hurt");
        private static readonly int Death = Animator.StringToHash("Death");
		private static readonly int Attack = Animator.StringToHash("Attack");

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
			_startPosition = transform.position;
        }
		
		private void Update()
		{
    		if (_isDead || _isAttacking) return;

    		DetectPlayer();
			if (_target != null)
    		{
        		float distance = Vector2.Distance(transform.position, _target.position);

        		if (distance > attackRange)
        		{
            		ChasePlayer();
        		}
        		else
        		{
            		StopMoving();
            		TryAttack();
        		}
    		}
		}

        private void FixedUpdate()
        {
			if (_isDead || _isAttacking || _target != null)
    		{
        		_rb.velocity = new Vector2(0, _rb.velocity.y);
        		_anim.SetFloat(Speed, 0);
        		return;
    		}
            _rb.velocity =
                new Vector2(_isFacingRight ? Mathf.Abs(enemyData.moveSpeed) : -Mathf.Abs(enemyData.moveSpeed),
                    _rb.velocity.y);
			
			_anim.SetFloat(Speed, Mathf.Abs(_rb.velocity.x));

            // Edge Detection Raycast
            var edgeDetect = Physics2D.Raycast(groundWallCheck.position, Vector2.down,
                enemyData.edgeDetectionDistance, enemyData.groundLayer);

            if (!edgeDetect && Time.time - _lastFlipTime > flipCooldown)
			{
    			FlipCharacter();
    		_lastFlipTime = Time.time;
			}
			
			// Patrol distance check
    		float distanceFromStart = transform.position.x - _startPosition.x;

    		if (_isFacingRight && distanceFromStart >= patrolDistance && Time.time - _lastFlipTime > flipCooldown)
			{
    			FlipCharacter();
    			_lastFlipTime = Time.time;
			}
			else if (!_isFacingRight && distanceFromStart <= -patrolDistance && Time.time - _lastFlipTime > flipCooldown)
			{
    			FlipCharacter();
    			_lastFlipTime = Time.time;
			}

			if (Mathf.Abs(_rb.velocity.x) > 0.1f && !_isDead && !_isAttacking)
			{
    			if (!_audioSource.isPlaying)
    				{
        				_audioSource.clip = footstepSound;
        				_audioSource.loop = true;
        				_audioSource.Play();
    				}
			}
			else
			{
    			if (_audioSource.isPlaying && _audioSource.clip == footstepSound)
    			{
        			_audioSource.Stop();
    			}
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

		public void TriggerAttack()
        {
            if (_isDead || _isAttacking) return;

            if (Time.time - _lastAttackTime < attackCooldown)
                return;

            _lastAttackTime = Time.time;

            _isAttacking = true;
            _rb.velocity = Vector2.zero;

            _anim.SetTrigger(Attack);
        }

		// Called by animation event (attack hurt frame)
        public void DealDamage(GameObject player)
        {
            var damageable = player.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.ChangeHealth(-enemyData.attackDamage);
            }
        }

        // Called at end of attack animation
        public void EndAttack()
        {
            _isAttacking = false;
        }
		
		public void EnableAttackBox()
		{
    		Debug.Log("AttackBox ENABLED");
    		attackBox.SetActive(true);
		}

		public void DisableAttackBox()
		{
    		Debug.Log("AttackBox DISABLED");
    		attackBox.SetActive(false);
		}
		

        public void ChangeHealth(int amount)
        {
            // Restrict the changed health value between 0 and "maxHealth"
            _currHealth = Mathf.Clamp(_currHealth + amount, 0, enemyData.health);

            // If we're decreasing health and enemy health still hasn't reached 0, play the hurt animation
            if (amount <= 0 && _currHealth != 0)
            {
                _anim.SetTrigger(Hurt);
            }

            // If enemy's health reached 0, trigger the death animation
            if (_currHealth <= 0)
            {
                EnemyDeath();
            }
        }
		/*
		public void HandleAttack(GameObject player)
		{
    		if (_isDead || _isAttacking) return;

			if (Time.time - _lastAttackTime < attackCooldown)
        		return;
			_lastAttackTime = Time.time;

    		// var playerController = player.GetComponent<HeroKnight_AnimController>();
    		var damageable = player.GetComponent<IDamageable>();
			if (damageable != null)
        	{
            		damageable.ChangeHealth(-enemyData.attackDamage);
        	}
			
    		if (playerController != null && playerController.Block)
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
            		damageable.ChangeHealth(-enemyData.attackDamage);
        		}
    		}
			
			
		}
		*/
		private void DetectPlayer()
		{
			if (_target != null) return;
    		Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

    		if (player != null)
    		{
        		Debug.Log("Player detected!");
        		_target = player.transform;
    		}
		}
		
		private void ChasePlayer()
		{
    		if (_isAttacking) return;

    		float direction = _target.position.x > transform.position.x ? 1 : -1;

    		_rb.velocity = new Vector2(direction * enemyData.moveSpeed, _rb.velocity.y);
    		_anim.SetFloat(Speed, Mathf.Abs(_rb.velocity.x));

    		// Flip toward player
    		if ((_isFacingRight && direction < 0) || (!_isFacingRight && direction > 0))
    		{
        		FlipCharacter();
    		}
		}
	
		private void StopMoving()
		{
    		_rb.velocity = new Vector2(0, _rb.velocity.y);
    		_anim.SetFloat(Speed, 0);
		}

		private void TryAttack()
		{
    		if (_isAttacking) return;

    		if (Time.time - _lastAttackTime >= attackCooldown)
    		{
        		Debug.Log("In range → ATTACK");
        		TriggerAttack();
    		}
		}

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
        
		#region Potions
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
		#endregion
        
        #region Audio Functions

        private void HandleWalkingSound()
        {
            if (Mathf.Abs(_rb.velocity.x) > 0.1f)
            {
                if (!_audioSource.isPlaying)
                {
                    _audioSource.clip = footstepSound;
                    _audioSource.loop = true;
                    _audioSource.Play();
                }
            }
            else
            {
                StopWalkingSound();
            }
        }

        private void StopWalkingSound()
        {
            if (_audioSource.isPlaying && _audioSource.clip == footstepSound)
            {
                _audioSource.Stop();
            }
        }
		public void PlayAttackSound()
        {
            if (attackSound != null)
            {
                _audioSource.PlayOneShot(attackSound);
            }
        }
     

        #endregion

         private void OnDrawGizmos()
         {
             Gizmos.color = Color.white;
             Gizmos.DrawRay(groundWallCheck.position, Vector2.down * enemyData.edgeDetectionDistance);
			 Gizmos.color = Color.yellow;
    		 Gizmos.DrawWireSphere(transform.position, detectionRange);
         }
    }
}