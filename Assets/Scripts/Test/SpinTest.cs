using System.Collections;
using UnityEngine;

namespace NCGames
{
    public class SpinTest : MonoBehaviour
    {
        private static readonly int RotateSpeedParameter = Animator.StringToHash("RotateSpeed");
        public Animator animator;
        [SerializeField] private float rotateSpeed;
        private float decreaseAmount;
        [SerializeField] private Vector2 minMaxRotateSpeed = new Vector2(7f, 10f);
        [SerializeField] private Vector2 minMaxDecreaseAmount = new Vector2(0.1f, 0.5f);
        private Coroutine spinCoroutine;

        public bool IsSpinning => spinCoroutine != null;

        private void InitializeSpin()
        {
            rotateSpeed = Random.Range(minMaxRotateSpeed.x, minMaxRotateSpeed.y);
            animator.SetFloat(RotateSpeedParameter, rotateSpeed);

            decreaseAmount = Random.Range(minMaxDecreaseAmount.x, minMaxDecreaseAmount.y);
        }

        public void StartSpin()
        {
            if (spinCoroutine == null)
            {
                InitializeSpin();
                spinCoroutine = StartCoroutine(Spin());
            }
        }

        IEnumerator Spin()
        {
            Debug.Log("Spin has started");
            while (rotateSpeed > 0)
            {
                rotateSpeed -= decreaseAmount * Time.deltaTime;
                animator.SetFloat(RotateSpeedParameter, rotateSpeed);

                yield return new WaitForEndOfFrame();
            }

            rotateSpeed = 0;
            animator.SetFloat(RotateSpeedParameter, rotateSpeed);
            Debug.Log("Spin is over");
            spinCoroutine = null;
        }
    }
}