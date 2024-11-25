using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressablesManager
{
    public static void Release(AsyncOperationHandle handle)
    {
        if (handle.IsValid())
        {
            Addressables.Release(handle); 
        }
    }
}
