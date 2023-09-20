using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Items
{
    [CreateAssetMenu(fileName = "newItem", menuName = "SO/Items/New Item")]
    public class Item : ScriptableObject, ISerializationCallbackReceiver
    {
        [BoxGroup("Base")] [SerializeField] string guid;
        [BoxGroup("Base")] [SerializeField] string itemName;
        [BoxGroup("Base")] [PreviewField] [SerializeField] Sprite icon;
        [BoxGroup("Base")] [SerializeField] Vector2Int size = new Vector2Int(1, 1);
        
        static Dictionary<string, Item> _itemLookupCache;

        static Action<string> _onLoaded;

        static void LoadItems(AsyncOperationHandle<IList<Item>> context)
        {
            _itemLookupCache = new Dictionary<string, Item>();
            foreach (var item in context.Result)
            {
                if (_itemLookupCache.TryGetValue(item.guid, out var value))
                {
                    Debug.LogError($"Duplicate ID for objects: {value} and {item}");
                    continue;
                }

                _itemLookupCache[item.guid] = item;
            }
        }

        public static Item GetFromID(string itemID)
        {
            if (_itemLookupCache == null)
            {
                AddressablesManager.LoadAssetsWithLabel<Item>("Item", LoadItems).WaitForCompletion();
            }

            if (itemID == null || !_itemLookupCache.ContainsKey(itemID)) return null;
            return _itemLookupCache[itemID];
        }

        
        public string GetID()
        {
            return guid;
        }

        public string GetName()
        {
            return itemName;
        }

        public Sprite GetIcon()
        {
            return icon;
        }

        public Vector2Int GetSize()
        {
            return size;
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Generate and save a new UUID if this is blank.
            if (string.IsNullOrWhiteSpace(guid))
            {
                guid = System.Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Require by the ISerializationCallbackReceiver but we don't need
            // to do anything with it.
        }
    }
}
