using System;
using System.Collections;
using NCGames.Events.Game;
using NCGames.Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NCGames
{
    /// <summary>
    /// Controls the roulette wheel's spinning behavior and animation.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class RouletteController : MonoBehaviour
    {
        private static readonly int RotateSpeedParameter = Animator.StringToHash("RotateSpeed");
        
        [Header("References")]
        [SerializeField] private Animator animator;
        
        [Header("Spin Configuration")]
        [SerializeField] private Vector2 minMaxRotateSpeed = new Vector2(7f, 10f);
        [SerializeField] private Vector2 minMaxDecreaseAmount = new Vector2(0.1f, 0.5f);
        
        // Runtime variables
        private float _rotateSpeed;
        private float _decreaseAmount;
        private Coroutine _spinCoroutine;
        
        /// <summary>
        /// Indicates whether the roulette wheel is currently spinning.
        /// </summary>
        public bool IsSpinning => _spinCoroutine != null;

        #region Unity Lifecycle Methods
        
        private void Awake()
        {
            // Ensure animator reference
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }
        
        #endregion

        #region Event Management
        
        private void SubscribeToEvents()
        {
            ServiceContainer.Instance.EventPublisherService.Subscribe<OnGameStartEvent>(OnGameStart);
        }

        private void UnsubscribeFromEvents()
        {
            ServiceContainer.Instance?.EventPublisherService.Unsubscribe<OnGameStartEvent>(OnGameStart);
        }
        
        private void OnGameStart(OnGameStartEvent e)
        {
            StartSpin();
        }
        
        #endregion

        #region Spin Control
        
        /// <summary>
        /// Initializes spin parameters with random values within configured ranges.
        /// </summary>
        private void InitializeSpin()
        {
            _rotateSpeed = Random.Range(minMaxRotateSpeed.x, minMaxRotateSpeed.y);
            animator.SetFloat(RotateSpeedParameter, _rotateSpeed);

            _decreaseAmount = Random.Range(minMaxDecreaseAmount.x, minMaxDecreaseAmount.y);
        }

        /// <summary>
        /// Starts the roulette spin if not already spinning.
        /// </summary>
        public void StartSpin()
        {
            if (_spinCoroutine == null)
            {
                InitializeSpin();
                _spinCoroutine = StartCoroutine(SpinRoutine());
            }
        }

        /// <summary>
        /// Handles the spinning behavior, gradually slowing down until stopping.
        /// </summary>
        private IEnumerator SpinRoutine()
        {
            Debug.Log("Roulette spin started");
            
            // Gradually slow down the spin
            while (_rotateSpeed > 0)
            {
                _rotateSpeed -= _decreaseAmount * Time.deltaTime;
                animator.SetFloat(RotateSpeedParameter, Mathf.Max(0, _rotateSpeed));
                yield return null;
            }

            // Ensure rotation is completely stopped
            _rotateSpeed = 0;
            animator.SetFloat(RotateSpeedParameter, 0);
            
            Debug.Log("Roulette spin completed");
            
            // Notify that the roulette has stopped spinning
            ServiceContainer.Instance.EventPublisherService.Publish(new OnRouletteStoppedEvent());
            _spinCoroutine = null;
        }
        
        #endregion
    }
}