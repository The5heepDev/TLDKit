using System;
using Harmony;
using UnityEngine;

namespace TLDKit {
    [HarmonyPatch(typeof(Utils), "GetInventoryIconTextureFromName")]
    internal class Patches {
        public static bool Prefix(string spriteName, ref Texture2D __result) {
            __result = (Texture2D)Resources.Load("InventoryGridIcons/" + spriteName);
            return false;
        }
    }

    [HarmonyPatch(typeof(Resources), "Load", new Type[] { typeof(string) })]
    internal class ResourcesLoadPatch {
        public static bool Prefix(string path, ref UnityEngine.Object __result) {
            if (!TLDKit.AssetManager.IsKnownAsset(path)) {
                return true;
            }
            __result = TLDKit.AssetManager.LoadAsset(path);
            return false;
        }
    }
}
