using System;
using NCGames.Events.Game;
using NCGames.Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NCGames
{
    /// <summary>
    /// Controls the ball behavior in the roulette game, including movement, physics, and game events.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class BallController : MonoBehaviour
    {
        [Header("Physics Configuration")]
        [SerializeField] [Range(0.5f, 2.0f)] private float bounceFactor = 1.0f;
        [SerializeField] [Range(5.0f, 15.0f)] private float maxSpeed = 10.0f;
        [SerializeField] [Range(0.5f, 2.0f)] private float minSpeed = 1f;
        [SerializeField] private PhysicMaterial ballMaterial;
        
        [Header("Initial Velocity")]
        [SerializeField] [Range(2.0f, 6.0f)] private float minInitialSpeed = 2.0f;
        [SerializeField] [Range(3.0f, 8.0f)] private float maxInitialSpeed = 5.0f;
        
        
        private Rigidbody _rigidbody;
        private Vector3 _defaultPosition;
        private bool _isRolling;

        #region Unity Lifecycle Methods

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        private void Start()
        {
            _defaultPosition = transform.position;
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void Update()
        {
            CheckBallMovement();
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleBallCollision(collision);
        }

        #endregion

        #region Event Management

        private void SubscribeToEvents()
        {
            ServiceContainer.Instance.EventPublisherService.Subscribe<OnGameStartEvent>(OnGameStarted);
            ServiceContainer.Instance.EventPublisherService.Subscribe<OnRouletteStoppedEvent>(OnRouletteStopped);
        }

        private void UnsubscribeFromEvents()
        {
            ServiceContainer.Instance?.EventPublisherService.Unsubscribe<OnGameStartEvent>(OnGameStarted);
            ServiceContainer.Instance?.EventPublisherService.Unsubscribe<OnRouletteStoppedEvent>(OnRouletteStopped);
        }

        private void OnGameStarted(OnGameStartEvent e)
        {
            StartBallRolling();
        }

        #endregion

        #region Ball Movement Control

        /// <summary>
        /// Monitors the ball's speed and publishes stop event when the ball stops moving.
        /// </summary>
        private void CheckBallMovement()
        {
            if (!_isRolling)
                return;

            if (_rigidbody.velocity.magnitude <= 0.01f)
            {
                StopBall();
            }
        }

        /// <summary>
        /// Calculates and applies bounce physics when the ball collides with objects.
        /// </summary>
        private void HandleBallCollision(Collision collision)
        {
            return;
            // Skip processing if ball is too slow or not rolling
            if (_rigidbody.velocity.magnitude <= minSpeed || !_isRolling)
                return;
                
            // Calculate bounce direction with physics reflection
            Vector3 bounceDirection = Vector3.Reflect(_rigidbody.velocity, collision.contacts[0].normal);
            
            // Apply bounce force with configured factor
            _rigidbody.velocity = bounceDirection * bounceFactor;

            // Clamp velocity to maximum speed
            if (_rigidbody.velocity.magnitude > maxSpeed)
            {
                _rigidbody.velocity = _rigidbody.velocity.normalized * maxSpeed;
            }
        }

        /// <summary>
        /// Resets the ball to initial position and applies a random initial velocity.
        /// </summary>
        private void StartBallRolling()
        {
            // Reset position and physics state
            ResetBall();
            
            // Clear any existing movement
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            
            // Enable physics and apply initial velocity
            _rigidbody.isKinematic = false;
            float initialSpeed = Random.Range(minInitialSpeed, maxInitialSpeed);
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            _rigidbody.velocity = randomDirection * initialSpeed;
            
            _isRolling = true;
        }

        /// <summary>
        /// Stops the ball and notifies the game system.
        /// </summary>
        private void StopBall()
        {
            _rigidbody.velocity = Vector3.zero;
            _isRolling = false;
            
            // Publish ball stopped event
            ServiceContainer.Instance.EventPublisherService.Publish(new OnBallStoppedEvent());
        }

        private void OnRouletteStopped( OnRouletteStoppedEvent e)
        {
            ballMaterial.bounciness = 0;
        }

        /// <summary>
        /// Resets the ball to its default position.
        /// </summary>
        private void ResetBall()
        {
            transform.position = _defaultPosition;
            _rigidbody.isKinematic = true;
            ballMaterial.bounciness = 3;
        }

        #endregion
    }
}