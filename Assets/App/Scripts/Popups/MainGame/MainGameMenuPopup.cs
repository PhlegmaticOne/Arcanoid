﻿using System.Collections.Generic;
using Libs.Localization.Base;
using Libs.Localization.Components.Base;
using Libs.Localization.Context;
using Libs.Popups;
using Libs.Services;
using Popups.PackChoose;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Popups.MainGame
{
    public class MainGameMenuPopup : Popup, ILocalizable
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _continueButton;

        [SerializeField] private List<LocalizationBindableComponent> _bindableComponents;

        public IEnumerable<ILocalizationBindable> GetBindableComponents() => _bindableComponents;
        private ILocalizationManager _localizationManager;
        private LocalizationContext _localizationContext;

        private UnityAction _onRestart;
        private UnityAction _onContinue;
        private UnityAction _onBack;
        private UnityAction _activeAction;

        protected override void InitializeProtected(IServiceProvider serviceProvider)
        {
            _localizationManager = serviceProvider.GetRequiredService<ILocalizationManager>();
            ConfigureRestartButton();
            ConfigureContinueButton();
            ConfigureBackButton();
            
            _localizationContext = LocalizationContext
                .Create(_localizationManager)
                .BindLocalizable(this)
                .Refresh();
        }

        public override void EnableInput()
        {
            EnableBehaviour(_backButton);
            EnableBehaviour(_restartButton);
            EnableBehaviour(_continueButton);
        }

        public override void DisableInput()
        {
            DisableBehaviour(_backButton);
            DisableBehaviour(_restartButton);
            DisableBehaviour(_continueButton);
        }

        protected override void OnClosed() => _activeAction?.Invoke();

        public override void Reset()
        {
            _localizationContext.Flush();
            _localizationContext = null;
            _onRestart = null;
            _onContinue = null;
            _onBack = null;
            _activeAction = null;
            RemoveAllListeners(_backButton);
            RemoveAllListeners(_restartButton);
            RemoveAllListeners(_continueButton);
        }

        public void OnRestart(UnityAction action) => _onRestart = action;

        public void OnContinue(UnityAction action) => _onContinue = action;

        public void OnBack(UnityAction action) => _onBack = action;

        private void ConfigureBackButton()
        {
            _backButton.onClick.AddListener(() =>
            {
                _onBack?.Invoke();
                PopupManager.CloseAllPopupsInstant();
            });
        }
        
        private void ConfigureRestartButton()
        {
            _restartButton.onClick.AddListener(() =>
            {
                _activeAction = _onRestart;
                PopupManager.CloseLastPopup();
            });
        }
        
        private void ConfigureContinueButton()
        {
            _continueButton.onClick.AddListener(() =>
            {
                _activeAction = _onContinue;
                PopupManager.CloseLastPopup();
            });
        }
    }
}