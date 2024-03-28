using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
    
    public static async UniTask<IList<T>> LoadAssetsWithLabel<T>(string label)
    {
        var assets = await Addressables.LoadAssetsAsync<T>(label, _ => { });
        return assets;
    }

    public static AsyncOperationHandle<T> LoadAssetWithName<T>(string name, Action<AsyncOperationHandle<T>> callback)
    {
        var handler = Addressables.LoadAssetAsync<T>(name);
        handler.Completed += callback;
        return handler;
    }
    
    public static async UniTask<T> LoadAssetWithName<T>(string label)
    {
        var asset = await Addressables.LoadAssetAsync<T>(label);
        return asset;
    }
    
    public static AsyncOperationHandle<GameObject> InstantiateWithName(string name, Action<AsyncOperationHandle<GameObject>> callback)
    {
        var handler = Addressables.InstantiateAsync(name);
        handler.Completed += callback;
        return handler;
    }
}
