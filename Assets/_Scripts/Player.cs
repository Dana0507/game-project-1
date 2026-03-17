using System.Collections.Generic;
using _Scripts.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;
using _Scripts.Misc_;

namespace _Scripts
{
    public class Player : MonoBehaviour, IDamageable
    {
        private float _xInput;
        [SerializeField] private float speed;

        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private GameObject groundCheck;
        [SerializeField] private float groundCheckRadius = 0.4f;
        [HideInInspector] public bool isGrounded;

        [SerializeField] private List<AudioClip> footstepsSfx;

        [Space(5), Header("Health")]
        [SerializeField] private int maxHealth;
        private int _currHealth;

        [Space(5), Header("Combat")]
        [SerializeField] private Transform attackBox;
        public Vector2 attackSize;
        public LayerMask damageableLayer;
        [SerializeField] private int baseAttackDamage = 1;

        private Rigidbody2D _rb;
        private Animator _anim;
        private AudioSource _audioSource;
        private AdvancedJump _advancedJump;
        private bool _isDead;

        private static readonly int AnimState = Animator.StringToHash("AnimState");
        private static readonly int Attack1 = Animator.StringToHash("Attack1");
        private static readonly int Grounded = Animator.StringToHash("Grounded");
        private static readonly int AirSpeedY = Animator.StringToHash("AirSpeedY");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Hurt = Animator.StringToHash("Hurt");
        private static readonly int Death = Animator.StringToHash("Death");
        private static readonly int NoBlood = Animator.StringToHash("noBlood");

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
            _advancedJump = GetComponent<AdvancedJump>();
        }

        private void Start()
        {
            _currHealth = maxHealth;
            _anim.SetBool(NoBlood, false);
        }

        private void Update()
        {
            if (_isDead) return;
            _xInput = Input.GetAxisRaw("Horizontal");

            _anim.SetInteger(AnimState, _xInput != 0 ? 1 : 0);

            isGrounded = Physics2D.OverlapCircle(
                groundCheck.transform.position,
                groundCheckRadius,
                groundLayer
            );

            _anim.SetBool(Grounded, isGrounded);
            _anim.SetFloat(AirSpeedY, _rb.velocity.y);

            if (Input.GetButtonDown("Jump"))
            {
                _anim.SetTrigger(Jump);
            }

            CheckCharacterFlip();

            if (Input.GetKeyDown(KeyCode.J))
            {
                _anim.SetTrigger(Attack1);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                GameManager.instance.PauseResumeGame();
            }
        }

        private void FixedUpdate()
        {
            if (_isDead) return;
            _rb.velocity = new Vector2(_xInput * speed, _rb.velocity.y);
        }

        public void FootstepSFX()
        {
            if (!isGrounded) return;
            if (footstepsSfx == null || footstepsSfx.Count == 0) return;

            var clip = footstepsSfx[Random.Range(0, footstepsSfx.Count)];
            _audioSource.PlayOneShot(clip);
        }

        private void CheckCharacterFlip()
        {
            var characterScale = transform.localScale;

            if (_xInput < 0)
            {
                characterScale.x = -Mathf.Abs(transform.localScale.x);
            }
            else if (_xInput > 0)
            {
                characterScale.x = Mathf.Abs(transform.localScale.x);
            }

            transform.localScale = characterScale;
        }

        public void PerformAttack()
        {
            var hitObjects = Physics2D.OverlapBoxAll(attackBox.position, attackSize, 0, damageableLayer);

            foreach (var hitObject in hitObjects)
            {
                var damageable = hitObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.ChangeHealth(-Mathf.Abs(baseAttackDamage));
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (attackBox == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackBox.transform.position, attackSize);
        }

        public virtual void ChangeHealth(int amount)
        {
            if (_isDead) return;

            _currHealth = Mathf.Clamp(_currHealth + amount, 0, maxHealth);

            if (amount < 0 && _currHealth > 0)
            {
                _anim?.SetTrigger(Hurt);
            }

            if (_currHealth == 0)
            {
                Die();
            }
        }
        
        private void Die()
        {
            if (_isDead) return;

            _isDead = true;

            _rb.velocity = Vector2.zero;

            if (_advancedJump != null)
            {
                _advancedJump.enabled = false;
            }

            _anim.SetBool(NoBlood, false);
            _anim.SetTrigger(Death);
        }
    }
}