using UnityEngine;

namespace _Scripts
{
    public class PlayerCombatDebug : MonoBehaviour
    {
        private Player _player;
        private Animator _anim;

        private static readonly int Block = Animator.StringToHash("Block");
        private static readonly int IdleBlock = Animator.StringToHash("IdleBlock");

        private void Awake()
        {
            _player = GetComponent<Player>();
            _anim = GetComponent<Animator>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                _player.ChangeHealth(-1);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                _player.ChangeHealth(-999);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                _anim.SetTrigger(Block);
                _anim.SetBool(IdleBlock, true);
            }

            if (Input.GetKeyUp(KeyCode.K))
            {
                _anim.SetBool(IdleBlock, false);
            }
        }
    }
}