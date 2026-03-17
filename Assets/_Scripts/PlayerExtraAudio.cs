using UnityEngine;

namespace _Scripts
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerExtraAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip swordSwooshSfx;
        [SerializeField] private AudioClip jumpSfx;
        [SerializeField] private AudioClip landSfx;
        [SerializeField] private AudioClip hurtSfx;
        [SerializeField] private AudioClip blockSfx;
        [SerializeField] private AudioClip deathSfx;

        private AudioSource _audioSource;
        private Player _player;

        private bool _initialized;
        private bool _wasGrounded;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _player = GetComponent<Player>();
        }

        private void LateUpdate()
        {
            if (_player == null) return;

            if (!_initialized)
            {
                _wasGrounded = _player.isGrounded;
                _initialized = true;
                return;
            }

            if (!_wasGrounded && _player.isGrounded)
            {
                PlayLandSFX();
            }

            _wasGrounded = _player.isGrounded;
        }

        public void PlaySwordSFX()
        {
            if (swordSwooshSfx == null) return;
            _audioSource.PlayOneShot(swordSwooshSfx);
        }

        public void PlayJumpSFX()
        {
            if (jumpSfx == null) return;
            _audioSource.PlayOneShot(jumpSfx);
        }

        private void PlayLandSFX()
        {
            if (landSfx == null) return;
            _audioSource.PlayOneShot(landSfx);
        }
        
        public void PlayHurtSFX()
        {
            if (hurtSfx == null) return;
            _audioSource.PlayOneShot(hurtSfx);
        }

        public void PlayBlockSFX()
        {
            if (blockSfx == null) return;
            _audioSource.PlayOneShot(blockSfx);
        }
        
        public void PlayDeathSFX()
        {
            if (deathSfx == null) return;
            _audioSource.PlayOneShot(deathSfx);
        }
    }
}