using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;
using Skyloft.Player;

namespace Skyloft.Editor
{
    /// <summary>
    /// Editor utility that runs automatically and provides a menu item to configure the Player prefab,
    /// attach the rifle to the right hand bone (mixamorig:RightHand), configure AnimatorController motions,
    /// and enforce mobile shadow settings.
    /// </summary>
    [InitializeOnLoad]
    public static class PlayerWeaponSetupEditor
    {
        private const string PlayerFbxPath = "Assets/Art/Original/player/player.fbx";
        private const string RiflePrefabPath = "Assets/Prefabs/Weapons/Rifle.prefab";
        private const string PlayerPrefabPath = "Assets/Prefabs/Player/Player.prefab";
        private const string AnimatorControllerPath = "Assets/Animations/PlayerAnimator.controller";

        private static bool s_HasRun = false;

        static PlayerWeaponSetupEditor()
        {
            EditorApplication.delayCall += () =>
            {
                if (!s_HasRun)
                {
                    s_HasRun = true;
                    SetupPlayerWeaponAndVisuals();
                }
            };
        }

        [MenuItem("Skyloft/Setup Player Weapon & Visuals")]
        public static void SetupPlayerWeaponAndVisuals()
        {
            Debug.Log("[Skyloft] Starting Player Weapon and Visual Setup...");

            SetupAnimatorClips();
            SetupPlayerPrefabWeapon();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Skyloft] Player Weapon and Visual Setup completed successfully!");
        }

        private static void SetupAnimatorClips()
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(AnimatorControllerPath);
            if (controller == null)
            {
                Debug.LogWarning($"[Skyloft] AnimatorController not found at: {AnimatorControllerPath}");
                return;
            }

            Object[] playerAssets = AssetDatabase.LoadAllAssetsAtPath(PlayerFbxPath);
            AnimationClip validClip = null;

            foreach (var asset in playerAssets)
            {
                if (asset is AnimationClip clip && !clip.name.StartsWith("__preview__"))
                {
                    Debug.Log($"[Skyloft] Found AnimationClip in FBX: '{clip.name}', duration: {clip.length:F2}s");
                    if (validClip == null)
                    {
                        validClip = clip;
                    }
                }
            }

            if (validClip != null && controller.layers.Length > 0)
            {
                var sm = controller.layers[0].stateMachine;
                foreach (var childState in sm.states)
                {
                    // Preserve explicitly configured locomotion clips across domain reloads.
                    if (childState.state.motion == null)
                    {
                        childState.state.motion = validClip;
                        Debug.Log($"[Skyloft] Bound motion '{validClip.name}' to Animator state '{childState.state.name}'.");
                    }
                }
                EditorUtility.SetDirty(controller);
            }
        }

        private static void SetupPlayerPrefabWeapon()
        {
            GameObject riflePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(RiflePrefabPath);
            if (riflePrefab == null)
            {
                Debug.LogError($"[Skyloft] Rifle prefab not found at: {RiflePrefabPath}");
                return;
            }

            GameObject playerRoot = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
            if (playerRoot == null)
            {
                Debug.LogError($"[Skyloft] Could not load Player prefab contents at: {PlayerPrefabPath}");
                return;
            }

            try
            {
                // 1. Remove any legacy/stray WeaponSocket under the root Player transform
                Transform straySocket = playerRoot.transform.Find("WeaponSocket");
                if (straySocket != null)
                {
                    Object.DestroyImmediate(straySocket.gameObject);
                    Debug.Log("[Skyloft] Removed stray WeaponSocket from Player root.");
                }

                // 2. Find the character skeleton right hand bone under Model
                Transform rightHandBone = FindBoneRecursive(playerRoot.transform, "RightHand");
                if (rightHandBone == null)
                {
                    Debug.LogError("[Skyloft] Could not find right hand bone (RightHand) in Player hierarchy.");
                    return;
                }

                Debug.Log($"[Skyloft] Found right hand bone: {rightHandBone.name}");

                // 3. Locate or create WeaponSocket under rightHandBone
                Transform weaponSocket = rightHandBone.Find("WeaponSocket");
                if (weaponSocket == null)
                {
                    GameObject socketGo = new GameObject("WeaponSocket");
                    weaponSocket = socketGo.transform;
                    weaponSocket.SetParent(rightHandBone, false);
                    Debug.Log("[Skyloft] Created WeaponSocket under " + rightHandBone.name);
                }

                // Set local grip transform relative to the hand bone
                var gripSetup = playerRoot.GetComponent<PlayerVisualSetup>();
                if (gripSetup != null)
                {
                    var gripSettings = new SerializedObject(gripSetup);
                    weaponSocket.localPosition = gripSettings.FindProperty("gripPositionOffset").vector3Value;
                    weaponSocket.localRotation = Quaternion.Euler(gripSettings.FindProperty("gripRotationOffset").vector3Value);
                }
                weaponSocket.localScale = Vector3.one;

                // 4. Ensure Rifle instance exists under WeaponSocket
                Transform rifleTransform = weaponSocket.Find("Rifle");
                if (rifleTransform == null)
                {
                    GameObject rifleInstance = (GameObject)PrefabUtility.InstantiatePrefab(riflePrefab, weaponSocket);
                    rifleInstance.name = "Rifle";
                    rifleInstance.transform.localPosition = Vector3.zero;
                    rifleInstance.transform.localRotation = Quaternion.identity;
                    rifleInstance.transform.localScale = Vector3.one;
                    Debug.Log("[Skyloft] Instantiated Rifle prefab under WeaponSocket.");
                }

                // 5. Apply mobile shadow settings across all renderers
                Renderer[] allRenderers = playerRoot.GetComponentsInChildren<Renderer>(true);
                foreach (var renderer in allRenderers)
                {
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                    renderer.receiveShadows = false;
                }

                // 6. Configure PlayerVisualSetup serialized fields
                var visualSetup = playerRoot.GetComponent<PlayerVisualSetup>();
                if (visualSetup != null)
                {
                    var serializedObject = new SerializedObject(visualSetup);
                    var socketProp = serializedObject.FindProperty("weaponSocket");
                    if (socketProp != null)
                    {
                        socketProp.objectReferenceValue = weaponSocket;
                    }
                    var rifleProp = serializedObject.FindProperty("riflePrefab");
                    if (rifleProp != null)
                    {
                        rifleProp.objectReferenceValue = riflePrefab;
                    }
                    serializedObject.ApplyModifiedProperties();
                }

                // 7. Save the updated Prefab asset
                PrefabUtility.SaveAsPrefabAsset(playerRoot, PlayerPrefabPath);
                Debug.Log("[Skyloft] Successfully saved updated Player.prefab!");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(playerRoot);
            }
        }

        private static Transform FindBoneRecursive(Transform current, string boneKeyword)
        {
            if (current.name.IndexOf(boneKeyword, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return current;
            }

            for (int i = 0; i < current.childCount; i++)
            {
                Transform found = FindBoneRecursive(current.GetChild(i), boneKeyword);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }
}
