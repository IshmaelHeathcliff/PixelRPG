using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressablesManager
{
    public static void LoadAssetsWithLabel<T>(AssetLabelReference label, Action<AsyncOperationHandle<IList<T>>> callback)
    {
        Addressables.LoadAssetsAsync<T>(label, _ => { }).Completed += callback;
    }
    public static void LoadAssetsWithLabel<T>(string label, Action<AsyncOperationHandle<IList<T>>> callback)
    {
        Addressables.LoadAssetsAsync<T>(label, _ => { }).Completed += callback;
    }

    public static void LoadAssetWithName<T>(string name, Action<AsyncOperationHandle<T>> callback)
    {
        Addressables.LoadAssetAsync<T>(name).Completed += callback;
    }

    public static void InstantiateWithName(string name, Action<AsyncOperationHandle<GameObject>> callback)
    {
        Addressables.InstantiateAsync(name).Completed += callback;
    }
}
