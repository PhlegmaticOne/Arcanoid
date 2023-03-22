﻿using Game.GameEntities.PlayerObjects.Base;
using Libs.Behaviors;
using UnityEngine;

namespace Game.GameEntities.PlayerObjects.BallObject
{
    public class Ball : BehaviorObject<Ball>, IStartMovable
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private CircleCollider2D _circleCollider2D;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _rageColor;
        private bool _isRage;

        private float _startSpeed;
        private Vector2 _direction = Vector2.up;

        public void Initialize(float initialSpeed) => _startSpeed = initialSpeed;
        public Collider2D GetCollider() => _circleCollider2D;

        public void StartMove() => SetSpeed(_direction * _startSpeed);

        public Transform GetTransform() => transform;
        public Vector2 GetSpeed() => _rigidbody2D.velocity;
        public void SetStartSpeed(float startSpeed) => _startSpeed = startSpeed;
        public void SetDirection(Vector2 direction) => _direction = direction;
        public void SetSpeed(Vector2 speed)
        {
            if (_rigidbody2D.bodyType != RigidbodyType2D.Static)
            {
                _rigidbody2D.velocity = speed;
            }
        }

        public void AddDeltaSpeed(float deltaSpeed)
        {
            var ballSpeed = GetSpeed();
            var ballSpeedMagnitude = ballSpeed.magnitude;
            var newSpeed = (deltaSpeed + ballSpeedMagnitude) * ballSpeed.normalized;
            SetSpeed(newSpeed);
        }

        public void CopyToBall(Ball ball)
        {
            InstallOnCollisionBehaviorsTo(ball);
            ball.ChangeRageMode(_isRage);
        }

        public void ChangeRageMode(bool isRage)
        {
            _isRage = isRage;
            _trailRenderer.gameObject.SetActive(isRage);
            var color = isRage ? _rageColor : Color.white;
            _spriteRenderer.color = color;
        }

        protected override void ResetProtected()
        {
            _startSpeed = 0;
            SetSpeed(Vector2.zero);

            if (_isRage)
            {
                ChangeRageMode(false);
            }
        }

        protected override bool CanBeDestroyedOnDestroyCollision() => true;
    }
}