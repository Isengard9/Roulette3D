using System;
using NCGames.Events.Game;
using NCGames.Services;
using UnityEditor.Rendering;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NCGames
{
    [RequireComponent(typeof(Rigidbody))]
    public class BallController : MonoBehaviour
    {
        [SerializeField] private float bounceFactor = 1.0f; // Bounce factor to adjust the bounce height
        [SerializeField] private float maxSpeed = 10.0f; // Maximum speed limit
        [SerializeField] private float minSpeed = 1f; // Minimum speed limit
        private Rigidbody rb;
        private Vector3 defaultPosition;
        private bool hasStarted;
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            defaultPosition = rb.transform.position;
        }

        private void Update()
        {
            if(!hasStarted)
                return;

            if (rb.velocity.magnitude <= 0f)
            {
                rb.velocity = Vector3.zero;
                hasStarted = false;
                ServiceContainer.Instance.EventPublisherService.Publish(new OnBallStoppedEvent());
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            // Calculate the bounce direction and apply the bounce force
            if (rb.velocity.magnitude <= minSpeed)
                return;
            Vector3 bounceDirection = Vector3.Reflect(rb.velocity, collision.contacts[0].normal);
            rb.velocity = bounceDirection * bounceFactor;

            // Clamp the velocity to the maximum speed
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }

        private void OnEnable()
        {
            ServiceContainer.Instance.EventPublisherService.Subscribe<OnGameStartEvent>(OnGameStarted);
        }

        private void OnDisable()
        {
            ServiceContainer.Instance?.EventPublisherService.Unsubscribe<OnGameStartEvent>(OnGameStarted);
        }

        private void OnGameStarted(OnGameStartEvent e)
        {
            ResetBall();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.velocity = Vector3.one * Random.Range(2, 5);
            hasStarted = true;
        }

        private void ResetBall()
        {
            rb.transform.position = defaultPosition;
            rb.isKinematic = true;
        }
    }
}