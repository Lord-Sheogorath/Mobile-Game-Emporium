using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JSH.BallLauncher
{
    public class BallHandler : MonoBehaviour
    {
        [SerializeField] private Ball m_ballPrefab;
        [SerializeField] private Rigidbody2D m_pivot;

        [Space]

        [SerializeField, Min(0)] private float m_maxDrawDistance;

        [Space]

        [SerializeField] private float m_delayDuration = 0.5f;
        [SerializeField] private float m_respawnDelay = 0.5f;

        private Camera m_mainCam;

        private Ball m_currentBall;

        private bool m_isDragging;

        private void OnDrawGizmos()
        {
            if (m_pivot == null) return;

            Color _col = Gizmos.color;
            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(m_pivot.position, m_maxDrawDistance);

            Gizmos.color = _col;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_mainCam = Camera.main;

            SpawnNewBall();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_currentBall == null) return;

            bool _isPressed = Touchscreen.current.primaryTouch.press.isPressed;

            if (_isPressed == false)
            {
                if (m_isDragging) LaunchBall();

                m_isDragging = false;

                return;
            }

            Vector2 _worldPos = GetWorldPosition();

            m_currentBall.SetKinematic(true);
            m_currentBall.SetVelocity(Vector2.zero);
            m_currentBall.SetPosition(_worldPos);

            m_isDragging = true;
        }

        private Vector2 GetWorldPosition()
        {
            Vector2 _touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            Vector2 _worldPos = m_mainCam.ScreenToWorldPoint(_touchPos);

            Vector2 _drawDir = (_worldPos - m_pivot.position).normalized;

            float _drawDist = Vector2.Distance(_worldPos, m_pivot.position);
            _drawDist = Mathf.Clamp(_drawDist, 0, m_maxDrawDistance);

            _worldPos = m_pivot.position + (_drawDir * _drawDist);

            return _worldPos;
        }

        private void LaunchBall()
        {
            m_currentBall.SetKinematic(false);

            Invoke(nameof(DetachBall), m_delayDuration);
        }
        private void DetachBall()
        {
            m_currentBall.SetSpringJointEnabled(false);

            m_currentBall.StartDespawn();
            m_currentBall = null;

            Invoke(nameof(SpawnNewBall), m_respawnDelay);
        }

        private void SpawnNewBall()
        {
            GameObject _ballInstance = Instantiate(m_ballPrefab.gameObject, m_pivot.position, Quaternion.identity);

            m_currentBall = _ballInstance.GetComponent<Ball>();
            m_currentBall.SetConnectedBody(m_pivot);
        }
    }
}