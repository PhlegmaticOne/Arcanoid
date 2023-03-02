﻿using Abstracts.Popups;
using Abstracts.Popups.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Popups
{
    public class LosePopup : Popup
    {
        [SerializeField] private Button _restartButton;

        private IPopupManager _popupManager;

        public void Initialize(IPopupManager popupManager)
        {
            _popupManager = popupManager;
            _restartButton.onClick.AddListener(() => _popupManager.HidePopup());
        }
        
        public override void EnableInput()
        {
            EnableBehaviour(_restartButton);
        }

        public override void DisableInput()
        {
            DisableBehaviour(_restartButton);
        }

        public override void Reset()
        {
            RemoveAllListeners(_restartButton);
        }
    }
}