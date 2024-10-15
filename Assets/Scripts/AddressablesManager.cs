using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressablesManager
{
    public static AsyncOperationHandle<IList<T>> LoadAssetsAsync<T>(AssetLabelReference label, 
        Action<AsyncOperationHandle<IList<T>>> onCompleted = null, 
        Action<T> callback = null)
    {
        var handle = Addressables.LoadAssetsAsync<T>(label, callback);
        handle.Completed += onCompleted;
        return handle;
    }
    public static AsyncOperationHandle<IList<T>> LoadAssetsAsync<T>(string label, 
        Action<AsyncOperationHandle<IList<T>>> onCompleted = null,
        Action<T> callback = null)
    {
        var handle = Addressables.LoadAssetsAsync<T>(label, callback);
        handle.Completed += onCompleted;
        return handle;
    }
    
    public static async UniTask<IList<T>> LoadAssets<T>(AssetReference label)
    {
        var handle = Addressables.LoadAssetsAsync<T>(label, _ => { });
        var assets = await handle;
        Addressables.Release(handle);
        return assets;
    }
    
    public static async UniTask<IList<T>> LoadAssets<T>(string label)
    {
        var handle = Addressables.LoadAssetsAsync<T>(label, _ => { });
        var assets = await handle;
        Addressables.Release(handle);
        return assets;
    }
    
    public static AsyncOperationHandle<T> LoadAssetAsync<T>(AssetReference name, Action<AsyncOperationHandle<T>> onCompleted = null)
    {
        var handle = Addressables.LoadAssetAsync<T>(name);
        handle.Completed += onCompleted;
        return handle;
    }

    public static AsyncOperationHandle<T> LoadAssetAsync<T>(string name, Action<AsyncOperationHandle<T>> onCompleted = null)
    {
        var handle = Addressables.LoadAssetAsync<T>(name);
        handle.Completed += onCompleted;
        return handle;
    }
    
    public static async UniTask<T> LoadAsset<T>(AssetReference name)
    {
        var handle = Addressables.LoadAssetAsync<T>(name);
        var asset = await handle;
        Addressables.Release(handle);
        return asset;
    }
    
    public static async UniTask<T> LoadAsset<T>(string name)
    {
        var handle = Addressables.LoadAssetAsync<T>(name);
        var asset = await handle;
        Addressables.Release(handle);
        return asset;
    }
    
    public static AsyncOperationHandle<GameObject> Instantiate(string name, Action<AsyncOperationHandle<GameObject>> onCompleted = null)
    {
        var handle = Addressables.InstantiateAsync(name);
        handle.Completed += onCompleted;
        return handle;
    }

    public static void Release(AsyncOperationHandle handle)
    {
        if (handle.IsValid())
        {
           Addressables.Release(handle); 
        }
    }
}
