using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace ExtUnityComponents
{
    public class RigidBodyInternalProperties
    {
        private Rigidbody _currentTarget = null;
        private DecoratorComponentsEditor _currentEditor;
        private RigidBodySpecialSettings _specialSettings;

        public void Init(Rigidbody parent, DecoratorComponentsEditor current)
        {
            _currentTarget = parent;
            _currentEditor = current;
            _specialSettings = _currentTarget.transform.GetOrAddComponent<RigidBodySpecialSettings>();
            _specialSettings.hideFlags = HideFlags.HideInInspector;
        }

        public void DisplayInternalProperties()
        {
            using (HorizontalScope horizontalScope = new HorizontalScope(EditorStyles.helpBox))
            {
                using (ExtGUIScopes.Verti())
                {
                    SetSolverIteration();
                    SetSleepThreshold();
                    SetMaxDepenetation();
                }
            }
        }

        private void SetSleepThreshold()
        {
            using (ExtGUIScopes.Horiz())
            {
                float sleepThreshold = ExtGUIFloatFields.FloatField(_currentTarget.sleepThreshold, null, out bool sleepThresholdChanged, "SleepThreshold", "Modify SleepThreshold of the rigidBody");
                if (sleepThresholdChanged)
                {
                    _currentTarget.sleepThreshold = sleepThreshold;
                    _specialSettings.SetSleepThreshold(_currentTarget.sleepThreshold);
                }
                GUILayout.Label("");
                if (GUILayout.Button("Default", GUILayout.Width(70)))
                {
                    _currentTarget.sleepThreshold = RigidBodySpecialSettings.DEFAULT_SLEEP_THRESHOLD;
                    _specialSettings.SetSleepThreshold(_currentTarget.sleepThreshold);
                }
            }
        }

        private void SetSolverIteration()
        {
            using (ExtGUIScopes.Horiz())
            {
                int solver = ExtGUIFloatFields.IntField(_currentTarget.solverIterations, null, out bool solverChanged, "SolverIteration", "Modify the solver iteration of the rigidbody");
                if (solverChanged)
                {
                    _currentTarget.solverIterations = solver;
                    _specialSettings.SetSolverIteration(_currentTarget.solverIterations);
                }
                GUILayout.Label("");
                if (GUILayout.Button("Default", GUILayout.Width(70)))
                {
                    _currentTarget.solverIterations = RigidBodySpecialSettings.DEFAULT_SOLVER_ITERATION;
                    _specialSettings.SetSolverIteration(_currentTarget.solverIterations);
                }
            }
        }

        private void SetMaxDepenetation()
        {
            using (ExtGUIScopes.Horiz())
            {
                float sleepThreshold = ExtGUIFloatFields.FloatField(_currentTarget.maxDepenetrationVelocity, null, out bool maxDepenetationChanged, "MaxDepenetrationVelocity", "Modify maxDepenetrationVelocity of the rigidBody");
                if (maxDepenetationChanged)
                {
                    _currentTarget.maxDepenetrationVelocity = sleepThreshold;
                    _specialSettings.SetMaxDepenetrationVelocity(_currentTarget.maxDepenetrationVelocity);
                }
                GUILayout.Label("");
                if (GUILayout.Button("Default", GUILayout.Width(70)))
                {
                    _currentTarget.maxDepenetrationVelocity = RigidBodySpecialSettings.DEFAULT_MAX_DEPENETRATION_VELOCITY;
                    _specialSettings.SetMaxDepenetrationVelocity(_currentTarget.maxDepenetrationVelocity);
                }
            }
        }
    }
}