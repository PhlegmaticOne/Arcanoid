﻿using Game.Base;
using Game.Field.Builder;
using Game.Field.Helpers;
using Game.PlayerObjects.BallObject.Spawners;
using Game.PlayerObjects.ShipObject;
using Game.Systems.Control;
using Libs.InputSystem;
using Libs.Pooling.Base;
using UnityEngine;

namespace Game
{
    public class MainGameFactory : IGameFactory<MainGameRequires, MainGame>
    {
        private readonly IInputSystem _inputSystem;
        private readonly IFieldBuilder _fieldBuilder;
        private readonly IPoolProvider _poolProvider;
        private readonly IBallSpawner _ballSpawner;

        private MainGameRequires _mainGameRequires;
        
        public MainGameFactory(IFieldBuilder fieldBuilder,
            IPoolProvider poolProvider,
            IBallSpawner ballSpawner,
            IInputSystem inputSystem)
        {
            _fieldBuilder = fieldBuilder;
            _poolProvider = poolProvider;
            _ballSpawner = ballSpawner;
            _inputSystem = inputSystem;
        }
        
        public void SetupGameRequires(MainGameRequires gameRequires)
        {
            _mainGameRequires = gameRequires;
        }

        public MainGame CreateGame()
        {
            var controlSystem = _mainGameRequires.ControlSystem;
            var ship = _mainGameRequires.Ship;
            controlSystem.Initialize(_inputSystem, ship, _mainGameRequires.Camera);
            return new MainGame(_poolProvider, _fieldBuilder, 
                _mainGameRequires.InteractableZoneSetter,
                controlSystem, _ballSpawner, ship);
        }
    }

    public class MainGameRequires : IGameRequires
    {
        public ControlSystem ControlSystem { get; set; }
        public Ship Ship { get; set; }
        public InteractableZoneSetter InteractableZoneSetter { get; set; }
        public Camera Camera { get; set; }
    }
}