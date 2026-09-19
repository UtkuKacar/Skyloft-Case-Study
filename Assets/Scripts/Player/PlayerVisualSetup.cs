using UnityEngine;
using UnityEngine.Rendering;

namespace Skyloft.Player
{
    /// <summary>
    /// Manages player visual configuration at initialization.
    /// Handles weapon hand socket binding and ensures the rifle moves synchronously with the character skeleton.
    /// Disables cast shadows and receive shadows across all player mesh renderers for optimal mobile rendering.
    /// Does not implement Update() to ensure zero per-frame CPU overhead.
    /// </summary>
    [DisallowMultipleComponent]
    public class PlayerVisualSetup : MonoBehaviour
    {
        [Header("Weapon Attachment")]
        [Tooltip("Visual socket transform where the weapon is attached.")]
        [SerializeField] private Transform weaponSocket;

        [Tooltip("Prefab of the rifle weapon to instantiate if none is attached at start.")]
        [SerializeField] private GameObject riflePrefab;

        [Header("Hand Bone Binding")]
        [Tooltip("Target bone name for weapon grip attachment.")]
        [SerializeField] private string targetHandBoneName = "RightHand";

        [Tooltip("Local grip position offset relative to hand bone.")]
        [SerializeField] private Vector3 gripPositionOffset = new Vector3(0.05f, 0.08f, 0.02f);

        [Tooltip("Local grip Euler rotation offset relative to hand bone to align barrel forward.")]
        [SerializeField] private Vector3 gripRotationOffset = new Vector3(0f, 90f, 90f);

        private void Awake()
        {
            SetupWeaponAttachment();
            ApplyMobileShadowSettings();
        }

        /// <summary>
        /// Ensures the weapon socket is correctly parented to the character's right hand bone
        /// so that skeletal animations directly drive the weapon transform.
        /// </summary>
        public void SetupWeaponAttachment()
        {
            Transform handBone = FindHandBone(transform);
            if (handBone == null)
            {
                Debug.LogWarning("[PlayerVisualSetup] Could not find right hand bone in character hierarchy.");
                return;
            }

            if (weaponSocket == null)
            {
                // Look for an existing WeaponSocket in children
                Transform existingSocket = transform.Find("WeaponSocket");
                if (existingSocket == null)
                {
                    existingSocket = handBone.Find("WeaponSocket");
                }

                if (existingSocket != null)
                {
                    weaponSocket = existingSocket;
                }
                else
                {
                    GameObject socketGo = new GameObject("WeaponSocket");
                    weaponSocket = socketGo.transform;
                }
            }

            // Ensure weapon socket is parented under the hand bone
            if (weaponSocket.parent != handBone)
            {
                weaponSocket.SetParent(handBone, false);
            }

            weaponSocket.localPosition = gripPositionOffset;
            weaponSocket.localRotation = Quaternion.Euler(gripRotationOffset);
            weaponSocket.localScale = Vector3.one;

            // If weapon socket has no child and riflePrefab is provided, instantiate it
            if (weaponSocket.childCount == 0 && riflePrefab != null)
            {
                GameObject rifleObj = Instantiate(riflePrefab, weaponSocket);
                rifleObj.name = "Rifle";
                rifleObj.transform.localPosition = Vector3.zero;
                rifleObj.transform.localRotation = Quaternion.identity;
                rifleObj.transform.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// Recursively searches for the hand bone in the character skeleton.
        /// </summary>
        private Transform FindHandBone(Transform current)
        {
            if (current.name.Contains(targetHandBoneName))
            {
                return current;
            }

            for (int i = 0; i < current.childCount; i++)
            {
                Transform found = FindHandBone(current.GetChild(i));
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Applies shadow casting and receiving rules to all child renderers for the mobile baseline.
        /// </summary>
        public void ApplyMobileShadowSettings()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].shadowCastingMode = ShadowCastingMode.Off;
                renderers[i].receiveShadows = false;
            }
        }

        public Transform WeaponSocket => weaponSocket;
        public GameObject RiflePrefab => riflePrefab;
    }
}
