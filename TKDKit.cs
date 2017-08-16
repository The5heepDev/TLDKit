using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace TLDKit {
    public class MapManager : MonoBehaviour {
        public static Vector3 FindCache() {
            LoadScene[] shit = FindObjectsOfType(typeof(LoadScene)) as LoadScene[];
            if (shit != null) {
                foreach (LoadScene each in shit) {
                    if (each.m_SceneToLoad.StartsWith("PrepperCache")) {
                        return each.gameObject.transform.localPosition;
                    }
                }
            }
            return Vector3.zero;
        }

        public static void AddToMap(Panel_Map map, String sceneName, string locName, string ico, Vector3 position) {
            Traverse panel_Map = Traverse.Create(map);
            Traverse worldPositionToMapPosition = panel_Map.Method("WorldPositionToMapPosition", new Type[] { typeof(String), typeof(Vector3) });
            Vector3 mapPosition = worldPositionToMapPosition.GetValue<Vector3>(new object[] { sceneName, position });

            MapElementSaveData mapElementSaveData = new MapElementSaveData();
            mapElementSaveData.m_LocationNameLocID = locName;
            mapElementSaveData.m_SpriteName = ico;
            mapElementSaveData.m_BigSprite = false;
            mapElementSaveData.m_IsDetail = true;
            mapElementSaveData.m_NameIsKnown = true;
            mapElementSaveData.m_PositionOnMap = (Vector2)mapPosition;
            mapElementSaveData.m_ActiveMissionLocIDs = new List<string>();
            mapElementSaveData.m_ActiveMissionTimerIDs = new List<string>();
            mapElementSaveData.m_ActiveMissionIDs = new List<string>();

            Transform child = map.m_DetailEntryPoolParent.GetChild(0);
            child.GetComponent<MapIcon>().DoSetup(mapElementSaveData, map.m_DetailEntryActiveObjects, 1000, MapIcon.MapIconType.TopIcon);

            Dictionary<Transform, MapElementSaveData> m_TransformToMapData = panel_Map.Field("m_TransformToMapData").GetValue<Dictionary<Transform, MapElementSaveData>>();
            m_TransformToMapData.Add(child, mapElementSaveData);
        }
    }
    public class AssetManager {
        private static Dictionary<string, AssetBundle> knownAssetBundles = new Dictionary<string, AssetBundle>();
        private static Dictionary<string, AssetBundle> knownAssetNames = new Dictionary<string, AssetBundle>();
        private static Dictionary<string, string> knownAssetShortNames = new Dictionary<string, string>();

        public static bool IsKnownAsset(string name) {
            if (name == null) {
                return false;
            }

            return GetFullAssetName(name) != null;
        }

        public static void RegisterAssetBundle(string relativePath) {
            string modDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assetBundlePath = Path.Combine(modDirectory, relativePath);

            Debug.Log("Loading mod asset bundle '" + relativePath + "' from path '" + assetBundlePath + "'.");

            AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
            if (!assetBundle) {
                Debug.LogError("Could not load asset bundle from '" + assetBundlePath + "'. Make sure the file exists and was created with the correct version of Unity.");
                return;
            }

            knownAssetBundles.Add(relativePath, assetBundle);

            string message = "Registered asset bundle '" + relativePath + "' with the following assets\n";
            foreach (string eachAssetName in assetBundle.GetAllAssetNames()) {
                UnityEngine.Object asset = assetBundle.LoadAsset(eachAssetName);

                string shortName = GetAssetShortName(eachAssetName, asset.name);
                knownAssetShortNames.Add(shortName, eachAssetName);
                knownAssetNames.Add(eachAssetName, assetBundle);

                message += "  " + shortName + " => " + eachAssetName + "\n";
            }

            Debug.Log(message);
        }

        public static UnityEngine.Object LoadAsset(string name) {
            string fullAssetName = GetFullAssetName(name);

            AssetBundle assetBundle;
            if (knownAssetNames.TryGetValue(fullAssetName, out assetBundle)) {
                return assetBundle.LoadAsset(fullAssetName);
            }

            Debug.LogError("Unknown asset " + name + ". Did you forget to register an asset bundle?");
            return null;
        }

        private static string GetAssetShortName(string assetPath, string assetName) {
            string result = assetPath.ToLower();

            if (result.StartsWith("assets/")) {
                result = result.Substring("assets/".Length);
            }

            int index = result.LastIndexOf(assetName.ToLower());
            if (index != -1) {
                result = result.Substring(0, index + assetName.Length);
            }

            return result;
        }

        private static string GetFullAssetName(string name) {
            string lowerCaseName = name.ToLower();
            if (knownAssetNames.ContainsKey(lowerCaseName)) {
                return lowerCaseName;
            }

            if (knownAssetShortNames.ContainsKey(lowerCaseName)) {
                return knownAssetShortNames[lowerCaseName];
            }

            return null;
        }
    }
}