using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSH.BallLauncher
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpringJoint2D))]
    public class Ball : MonoBehaviour
    {
        [SerializeField] private GameObject m_despawnEffectPrefab;
        [SerializeField] private float m_despawnDelay;

        [Space]

        [SerializeField] private GameObject m_collisionEffectPrefab;
        [SerializeField] private float m_collisisionDelay;

        [Space]

        [SerializeField] private Rigidbody2D m_rigidBody;
        [SerializeField] private SpringJoint2D m_springJoint;

        private float m_lastCollisionTime;

        public void SetPosition(Vector3 pos) => m_rigidBody.MovePosition(pos);
        public void SetVelocity(Vector3 vel) => m_rigidBody.velocity = vel;

        public void SetKinematic(bool isKinematic) => m_rigidBody.isKinematic = isKinematic;

        public void SetSpringJointEnabled(bool isEnabled) => m_springJoint.enabled = isEnabled;
        public void SetConnectedBody(Rigidbody2D connectedBody) => m_springJoint.connectedBody = connectedBody;

        public void StartDespawn() => Invoke(nameof(Despawn), m_despawnDelay);

        private void Despawn()
        {
            Instantiate(m_despawnEffectPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (Time.time < m_lastCollisionTime + m_collisisionDelay) return;

            GameObject _collisionEffect = Instantiate(m_collisionEffectPrefab, collision.contacts[0].point, Quaternion.identity);

            m_lastCollisionTime = Time.time;
        }
    }
}