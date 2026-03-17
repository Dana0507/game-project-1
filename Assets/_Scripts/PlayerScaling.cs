using System.Collections;
using UnityEngine;

namespace _Scripts
{
    public class PlayerScaling : MonoBehaviour
    {
        [SerializeField] private float maxScale = 2f;
        [SerializeField] private float scaleDuration = 0.15f;
        [SerializeField] private ParticleSystem scaleParticles;
        [SerializeField] private AudioClip scaleSfx;

        private AudioSource _audioSource;
        private bool _isScaling;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Grow(float amount)
        {
            if (_isScaling) return;
            StartCoroutine(GrowRoutine(amount));
        }

        private IEnumerator GrowRoutine(float amount)
        {
            _isScaling = true;

            if (scaleParticles != null)
            {
                scaleParticles.Play();
            }

            if (_audioSource != null && scaleSfx != null)
            {
                _audioSource.PlayOneShot(scaleSfx);
            }

            Vector3 startScale = transform.localScale;
            float xSign = Mathf.Sign(startScale.x);

            float targetAbsX = Mathf.Abs(startScale.x) + Mathf.Abs(amount);
            float targetY = startScale.y + Mathf.Abs(amount);

            if (maxScale > 0f)
            {
                targetAbsX = Mathf.Min(targetAbsX, maxScale);
                targetY = Mathf.Min(targetY, maxScale);
            }

            float time = 0f;

            while (time < scaleDuration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / scaleDuration);

                float currentAbsX = Mathf.Lerp(Mathf.Abs(startScale.x), targetAbsX, t);
                float currentY = Mathf.Lerp(startScale.y, targetY, t);

                transform.localScale = new Vector3(xSign * currentAbsX, currentY, startScale.z);
                yield return null;
            }

            transform.localScale = new Vector3(xSign * targetAbsX, targetY, startScale.z);
            _isScaling = false;
        }
    }
}