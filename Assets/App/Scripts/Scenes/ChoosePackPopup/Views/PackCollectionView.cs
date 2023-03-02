﻿using System.Collections.Generic;
using Scenes.MainGameScene.Configurations.Packs;
using UnityEngine;
using UnityEngine.Events;

namespace Scenes.ChoosePackPopup.Views
{
    public class PackCollectionView : MonoBehaviour
    {
        //ObjectPool for PackPreview
        [SerializeField] private RectTransform _viewsTransform;
        [SerializeField] private PackPreview _packPreview;

        private readonly List<PackPreview> _previews = new List<PackPreview>();
        private readonly List<PackConfiguration> _packConfigurations = new List<PackConfiguration>();
        public event UnityAction<PackConfiguration> PackClicked; 

        public void ShowPacks(IEnumerable<PackConfiguration> packConfigurations)
        {
            var i = 0;
            foreach (var packConfiguration in packConfigurations)
            {
                var packPreview = Instantiate(_packPreview, _viewsTransform);
                packPreview.Clicked += PackPreviewOnClicked;
                packPreview.UpdateView(i, packConfiguration);
                _previews.Add(packPreview);
                _packConfigurations.Add(packConfiguration);
                ++i;
            }
        }

        private void PackPreviewOnClicked(int index)
        {
            PackClicked?.Invoke(_packConfigurations[index]);
        }

        public void Clear()
        {
            foreach (var packPreview in _previews)
            {
                packPreview.Clicked -= PackPreviewOnClicked;
                Destroy(packPreview.gameObject);
            }
            
            _packConfigurations.Clear();
            _previews.Clear();
        }
    }
}