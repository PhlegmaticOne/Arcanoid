﻿using Game.Behaviors;
using Game.Systems.Health;
using UnityEngine;

namespace Game.PlayerObjects.BallObject.Behaviors.Bottom
{
    public class HealthRemoveBehaviour : IObjectBehavior<Ball>
    {
        private readonly HealthSystem _healthSystem;
        private float _healthToRemove;

        public HealthRemoveBehaviour(HealthSystem healthSystem) => _healthSystem = healthSystem;

        public void SetBehaviourParameters(int healthToRemove) => _healthToRemove = healthToRemove;

        public void Behave(Ball entity, Collision2D collision2D) => _healthSystem.LoseHealth();
    }
}