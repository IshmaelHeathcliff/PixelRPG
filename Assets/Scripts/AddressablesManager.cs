using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressablesManager
{
    public static AsyncOperationHandle<IList<T>> LoadAssetsWithLabel<T>(AssetLabelReference label, Action<AsyncOperationHandle<IList<T>>> callback)
    {
        var handler = Addressables.LoadAssetsAsync<T>(label, _ => { });
        handler.Completed += callback;
        return handler;
    }
    public static AsyncOperationHandle<IList<T>> LoadAssetsWithLabel<T>(string label, Action<AsyncOperationHandle<IList<T>>> callback)
    {
        var handler = Addressables.LoadAssetsAsync<T>(label, _ => { });
        handler.Completed += callback;
        return handler;
    }

    public static AsyncOperationHandle<T> LoadAssetWithName<T>(string name, Action<AsyncOperationHandle<T>> callback)
    {
        var handler = Addressables.LoadAssetAsync<T>(name);
        handler.Completed += callback;
        return handler;
    }

    public static AsyncOperationHandle<GameObject> InstantiateWithName(string name, Action<AsyncOperationHandle<GameObject>> callback)
    {
        var handler = Addressables.InstantiateAsync(name);
        handler.Completed += callback;
        return handler;
    }
}
