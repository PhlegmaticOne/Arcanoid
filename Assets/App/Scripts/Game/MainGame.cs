﻿using System;
using Game.Accessors;
using Game.Base;
using Game.Blocks;
using Game.Field;
using Game.Field.Builder;
using Game.Field.Helpers;
using Game.PlayerObjects.BallObject;
using Game.PlayerObjects.BallObject.Spawners;
using Game.PlayerObjects.ShipObject;
using Game.Systems.Control;
using Game.Systems.StateCheck;
using Libs.Pooling.Base;
using UnityEngine;

namespace Game
{
    public class MainGame : IGame<MainGameData, MainGameEvents>
    {
        private readonly IPoolProvider _poolProvider;
        private readonly IFieldBuilder _fieldBuilder;
        private readonly IObjectAccessor<GameField> _gameFieldAccessor;
        private readonly IObjectAccessor<BallsOnField> _ballsOnFieldAccessor;
        private readonly InteractableZoneSetter _interactableZoneSetter;
        private readonly ControlSystem _controlSystem;
        private readonly IBallSpawner _ballSpawner;
        private readonly Ship _ship;

        private GameField _gameField;
        private Ball _ball;
        private StateCheckSystem _stateCheckSystem;

        public MainGame(IPoolProvider poolProvider,
            IFieldBuilder fieldBuilder,
            IObjectAccessor<GameField> gameFieldAccessor,
            IObjectAccessor<BallsOnField> ballsOnFieldAccessor,
            InteractableZoneSetter interactableZoneSetter,
            ControlSystem controlSystem,
            IBallSpawner ballSpawner, 
            Ship ship)
        {
            _poolProvider = poolProvider;
            _fieldBuilder = fieldBuilder;
            _gameFieldAccessor = gameFieldAccessor;
            _ballsOnFieldAccessor = ballsOnFieldAccessor;
            _interactableZoneSetter = interactableZoneSetter;
            _controlSystem = controlSystem;
            _ballSpawner = ballSpawner;
            _ship = ship;
            Events = new MainGameEvents();
        }

        public MainGameEvents Events { get; }
        public event Action Won;
        public event Action Lost;
        
        public void StartGame(MainGameData data)
        {
            _gameField = _fieldBuilder.BuildField(data.LevelData);
            _gameFieldAccessor.Set(_gameField);
            _stateCheckSystem = new StateCheckSystem(_gameField);
            _stateCheckSystem.ActiveBlocksDestroyed += StateCheckSystemOnActiveBlocksDestroyed;
            _gameField.BlockRemoved += GameFieldOnBlockRemoved;
            var interactableBounds = _interactableZoneSetter.CalculateZoneBounds(_gameField.Bounds);
            _interactableZoneSetter.SetInteractableZone(interactableBounds);
            _controlSystem.SetInteractableBounds(interactableBounds);
            _controlSystem.Enable();
            _ship.Enable();
            _ball = _ballSpawner.CreateBall(new BallCreationContext(Vector2.zero, 5));
            var ballsOnField = new BallsOnField();
            ballsOnField.AddBall(_ball);
            _ballsOnFieldAccessor.Set(ballsOnField);
            _controlSystem.AddObjectToFollow(_ball);
        }

        private void GameFieldOnBlockRemoved(Block block)
        {
            Events.OnBlockDestroyed(new BlockDestroyedEventArgs
            {
                ActiveBlocksCount = _gameField.StartActiveBlocksCount,
                RemainBlocksCount = _gameField.ActiveBlocksCount
            });
        }

        private void StateCheckSystemOnActiveBlocksDestroyed() => Won?.Invoke();

        public void Pause()
        {
            _controlSystem.DisableInput();
            SetTimeScale(0);
        }

        public void Unpause()
        {
            _controlSystem.EnableInput();
            SetTimeScale(1);
        }

        public void Stop()
        {
            var blocksPool = _poolProvider.GetPool<Block>();
            var ballsPool = _poolProvider.GetPool<Ball>();
            
            foreach (var block in _gameField.Blocks)
            {
                if (block.IsDestroyed == false)
                {
                    blocksPool.ReturnToPool(block);
                }
            }
            
            _stateCheckSystem.ActiveBlocksDestroyed -= StateCheckSystemOnActiveBlocksDestroyed;
            _gameField.BlockRemoved -= GameFieldOnBlockRemoved;
            ballsPool.ReturnToPool(_ball);
            _ballsOnFieldAccessor.Get().Clear();
            _ballsOnFieldAccessor.Reset();
            _gameFieldAccessor.Reset();
            _controlSystem.Disable();
            SetTimeScale(1);
        }

        private void SetTimeScale(float scale)
        {
            Time.timeScale = scale;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }
}