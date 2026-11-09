using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voyageur.Networking
{
    /// <summary>
    /// Maps placeable ItemSubType values to networked placable prefabs (must have NetworkObject).
    /// </summary>
    public class NetworkBuildingCatalog : MonoBehaviour
    {
        [Serializable]
        public class Entry
        {
            public ItemSubType ItemSubType;
            public GameObject Prefab;
        }

        [SerializeField] List<Entry> entries = new List<Entry>();

        static NetworkBuildingCatalog instance;

        void Awake()
        {
            instance = this;
        }

        void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        public static bool TryGetPrefab(ItemSubType itemSubType, out GameObject prefab)
        {
            prefab = null;
            if (instance == null)
            {
                return false;
            }

            for (int i = 0; i < instance.entries.Count; i++)
            {
                var entry = instance.entries[i];
                if (entry != null && entry.ItemSubType == itemSubType && entry.Prefab != null)
                {
                    prefab = entry.Prefab;
                    return true;
                }
            }

            return false;
        }

#if UNITY_EDITOR
        public void SetEntriesForEditor(List<Entry> newEntries)
        {
            entries = newEntries;
        }
#endif
    }
}
