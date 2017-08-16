
using UnityEngine;

namespace LoadAsset
{
    public class LoadAsset
    {
        public static void OnLoad()
        {
            ModAssetBundleManager.RegisterAssetBundle("ak47");

            uConsole.RegisterCommand("prybar", new uConsole.DebugCommand(SpawnPrybar));
            uConsole.RegisterCommand("ak47", new uConsole.DebugCommand(Spawnak47));
        }

        private static void SpawnPrybar()
        {
            UnityEngine.Object prefab = Resources.Load("GEAR_Prybar");
            Debug.Log("Prefab: " + prefab);

            Transform playerTransform = GameManager.GetPlayerTransform();
            Vector3 targetPosition = playerTransform.position + playerTransform.forward * 2;

            instantiatePrefab((GameObject)prefab, targetPosition);
        }

        private static void Spawnak47()
        {
            UnityEngine.Object prefab = Resources.Load("GEAR_ak47");
            Debug.Log("Prefab: " + prefab);

            Transform playerTransform = GameManager.GetPlayerTransform();
            Vector3 targetPosition = playerTransform.position + playerTransform.forward * 2;

            instantiatePrefab((GameObject)prefab, targetPosition);
        }

        private static void instantiatePrefab(GameObject prefab, Vector3 targetPosition)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(prefab, targetPosition, Quaternion.identity);
            gameObject.name = prefab.name;
            FirstPersonWeapon goFPW = gameObject.GetComponent<FirstPersonWeapon>();
            //FirstPersonWeapon rifle = (FirstPersonWeapon)Resources.Load("GEAR_Rifle");
            //goFPW.m_Animator = rifle.m_Animator;
            //goFPW.m_BulletEmissionPoint = rifle.m_BulletEmissionPoint;
            //GunItem gun = gameObject.GetComponent<GunItem>();
            //GameObject ammo = UnityEngine.Object.Instantiate<GameObject>(Resources.Load("GEAR_RifleAmmoSingle") as GameObject);
            //gun.m_AmmoPrefab = ammo;
            int m_FPSMeshID = GameManager.GetVpFPSCamera().GetWeaponIDFromName("Rifle");
            vp_FPSWeapon a = GameManager.GetVpFPSCamera().GetWeaponFromID(m_FPSMeshID);
            a.m_GearItem = gameObject.GetComponent<GearItem>();
            a.m_GunItem = gameObject.GetComponent<GunItem>();
            a.m_FirstPersonWeaponRightHand = goFPW;
            a.m_FirstPersonWeaponShoulderPrefab = gameObject;

            Debug.Log("Created game object " + gameObject + ", layer " + gameObject.layer);
            foreach (Component eachComponent in gameObject.GetComponents<Component>())
            {
                Debug.Log("  with component " + eachComponent);
            }

            StickToGroundAndOrientOnSlope(gameObject.transform, gameObject.transform.position);
        }

        private static bool StickToGroundAndOrientOnSlope(Transform modifiedTransform, Vector3 desiredPosition)
        {
            RaycastHit hitInfo;
            if (!Physics.Raycast(desiredPosition, Vector3.down, out hitInfo, float.PositiveInfinity, Utils.m_PhysicalCollisionLayerMask))
            {
                return false;
            }

            modifiedTransform.position = hitInfo.point;
            modifiedTransform.rotation = Quaternion.identity;
            modifiedTransform.rotation = Utils.GetOrientationOnSlope(GameManager.GetPlayerManagerComponent().transform, hitInfo.normal);
            return true;
        }
    }
}
