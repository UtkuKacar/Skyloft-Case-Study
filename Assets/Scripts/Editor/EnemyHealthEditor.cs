using Skyloft.Enemy;
using UnityEditor;
using UnityEngine;

namespace Skyloft.Editor
{
    /// <summary>Editor-only health inspection and manual damage verification.</summary>
    [CustomEditor(typeof(EnemyHealth))]
    public sealed class EnemyHealthEditor : UnityEditor.Editor
    {
        private float testDamage = 25f;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var health = (EnemyHealth)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Play Mode Verification", EditorStyles.boldLabel);
            if (!Application.isPlaying || EditorUtility.IsPersistent(health))
            {
                EditorGUILayout.HelpBox(
                    "Enter Play Mode and select the scene Enemy to inspect health and apply test damage. " +
                    "Current health initializes from Max Health in Awake.", MessageType.Info);
                return;
            }

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.FloatField("Current Health", health.CurrentHealth);
                EditorGUILayout.Toggle("Is Dead", health.IsDead);
            }

            bool canDamage = health.isActiveAndEnabled && !health.IsDead;
            testDamage = EditorGUILayout.FloatField("Test Damage", testDamage);
            bool validDamage = testDamage > 0f && !float.IsNaN(testDamage) && !float.IsInfinity(testDamage);
            using (new EditorGUI.DisabledScope(!canDamage || !validDamage))
            {
                if (GUILayout.Button("Apply Test Damage"))
                {
                    ApplyTestDamage(health, testDamage);
                }
            }

            using (new EditorGUI.DisabledScope(!canDamage))
            {
                if (GUILayout.Button("Apply Lethal Damage"))
                {
                    ApplyTestDamage(health, health.CurrentHealth);
                }
            }

            EditorGUILayout.HelpBox(
                "Death deactivates the Enemy. Stop and restart Play Mode for a fresh test. " +
                "These controls are editor-only and do not apply damage during normal gameplay.", MessageType.Info);
        }

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

        public static void ApplyTestDamage(EnemyHealth health, float damage)
        {
            if (!Application.isPlaying || health == null || EditorUtility.IsPersistent(health))
            {
                return;
            }

            health.TakeDamage(damage);
        }
    }
}
