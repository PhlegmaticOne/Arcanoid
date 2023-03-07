﻿using System;
using Libs.Pooling.Base;
using Libs.Popups.Animations.Base;
using Libs.Popups.Animations.Types;
using Libs.Popups.Base;
using Libs.Popups.Configurations;
using Libs.Popups.View;
using UnityEngine;
using UnityEngine.UI;
using IServiceProvider = Libs.Services.IServiceProvider;

namespace Libs.Popups
{
    /// <summary>
    /// Popup lifetime pipeline:
    /// Initialize (InistializeProtected) -> OnShowed -> EnableInput -> ... -> DisableInput -> OnClosed -> Reset
    /// </summary>
    public abstract class Popup : MonoBehaviour, IPoolable
    {
        [SerializeField] private PopupView _popupView;
        protected IPopupManager PopupManager;
        private IPopupAnimationsFactory<AppearAnimationType> _appearAnimationsFactory;
        private IPopupAnimationsFactory<DisappearAnimationType> _disappearAnimationsFactory;
        private Action _onCloseSpawnAction;
        
        private RectTransform _parentTransform;

        private PopupAnimationConfiguration _popupAnimationConfiguration;
        public PopupView PopupView => _popupView;
        public RectTransform RectTransform => transform as RectTransform;


        public void Initialize(IServiceProvider serviceProvider)
        {
            PopupManager = serviceProvider.GetRequiredService<IPopupManager>();
            InitializeProtected(serviceProvider);
        }

        
        public abstract void EnableInput();
        
        public abstract void DisableInput();
        
        protected abstract void InitializeProtected(IServiceProvider serviceProvider);
        
        
        public void Show(int sortingOrder, Action onShowed)
        {
            _popupView.SetSortOrder(sortingOrder);
            
            var popupAnimation = _appearAnimationsFactory
                .CreateAnimation(_popupAnimationConfiguration.AppearAnimationType, _parentTransform);
            
            popupAnimation.OnAnimationPlayed(() =>
            {
                onShowed?.Invoke();
                popupAnimation.Stop(this);
                OnShowed();
                EnableInput();
            });
            
            popupAnimation.Play(this, _popupAnimationConfiguration.AppearanceTime);
        }
        
        public void Close(Action onCloseAction)
        {
            var popupAnimation = _disappearAnimationsFactory
                .CreateAnimation(_popupAnimationConfiguration.DisappearAnimationType, _parentTransform);
            
            popupAnimation.OnAnimationPlayed(() =>
            {
                popupAnimation.Stop(this);
                CloseInstant();
                onCloseAction?.Invoke();
            });
            
            popupAnimation.Play(this, _popupAnimationConfiguration.DisappearanceTime);
        }

        public void CloseInstant()
        {
            DisableInput();
            OnClosed();
        }
        
        public void SetPopupConfiguration(PopupConfiguration popupConfiguration)
        {
            _popupAnimationConfiguration = popupConfiguration.PopupAnimationConfiguration;
            _popupView.SetSortingLayer(popupConfiguration.SortingLayerName);
        }

        public void SetParentTransform(RectTransform parentTransform)
        {
            _parentTransform = parentTransform;
        }

        public void SetAnimationFactories(
            IPopupAnimationsFactory<AppearAnimationType> appearAnimationsFactory,
            IPopupAnimationsFactory<DisappearAnimationType> disappearAnimationsFactory)
        {
            _appearAnimationsFactory = appearAnimationsFactory;
            _disappearAnimationsFactory = disappearAnimationsFactory;
        }

        protected virtual void OnShowed() { }

        protected virtual void OnClosed()
        {
            _onCloseSpawnAction?.Invoke();
            _onCloseSpawnAction = null;
        }
        
        public virtual void Reset() { }

        protected void OnCloseSpawn<TPopup>(Action<TPopup> withSetup = null) where TPopup : Popup
        {
            _onCloseSpawnAction = () =>
            {
                var popup = PopupManager.SpawnPopup<TPopup>();
                withSetup?.Invoke(popup);
            };
        }
        
        protected static void DisableBehaviour(Behaviour behaviour) => behaviour.enabled = false;
        protected static void EnableBehaviour(Behaviour behaviour) => behaviour.enabled = true;
        protected static void RemoveAllListeners(Button button) => button.onClick.RemoveAllListeners();
    }
}