﻿using Abstracts.Pooling.Base;
using Abstracts.Pooling.Implementation;
using UnityEngine;

namespace Abstracts.Pooling
{
    public class UnityObjectPool<T> : ObjectPool<T> where T : MonoBehaviour, IPoolable
    {
        public UnityObjectPool(PrefabInfo<T> prefabInfo, int initialCapacity, int maxCapacity,
            bool destroyItemsOnOverflow = true) :
            base(new MonoFuncCreationStrategy<T>(prefabInfo), new MonoPoolableBehaviour<T>(),
                initialCapacity, maxCapacity, destroyItemsOnOverflow) { }
    }
}