using UnityEngine;

namespace NCGames
{
    [RequireComponent(typeof(Rigidbody))]
    public class BounceTest : MonoBehaviour
    {
        [SerializeField] private float bounceFactor = 1.0f; // Bounce factor to adjust the bounce height
        [SerializeField] private float maxSpeed = 10.0f; // Maximum speed limit
        private Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void OnCollisionEnter(Collision collision)
        {
            // Calculate the bounce direction and apply the bounce force
            Vector3 bounceDirection = Vector3.Reflect(rb.velocity, collision.contacts[0].normal);
            rb.velocity = bounceDirection * bounceFactor;

            // Clamp the velocity to the maximum speed
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }
    }
}