using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace extUnityComponents
{
    public class RigidBodyInternalProperties
    {
        private Rigidbody _currentTarget = null;
        private DecoratorComponentsEditor _currentEditor;
        private RigidBodyAdditionalMonobehaviourSettings _specialSettings;
        private RigidBodyCenterOfMass _centerOfMass = new RigidBodyCenterOfMass();

        public void Init(Rigidbody parent, DecoratorComponentsEditor current)
        {
            _currentTarget = parent;
            _currentEditor = current;
            _specialSettings = _currentTarget.transform.GetComponent<RigidBodyAdditionalMonobehaviourSettings>();
            InitializeLate(current);
        }

        private void InitializeLate(DecoratorComponentsEditor current)
        {
            if (_specialSettings)
            {
                _specialSettings.hideFlags = HideFlags.NotEditable;
            }
            _centerOfMass.Init(_currentTarget, current, _specialSettings);
        }

        public void CustomDisable()
        {
            _centerOfMass.CustomDisable();
        }

        public void InitOnFirstOnSceneGUI()
        {
            _centerOfMass.InitOnFirstOnSceneGUI();
        }

        public void CustomOnSceneGUI()
        {
            _centerOfMass.CustomOnSceneGUI();
        }

        public bool IsSpecialSettingsActive()
        {
            return (_specialSettings != null);
        }

        public void DisplayInternalProperties(bool justCreated, bool justDestroyed)
        {
            if (justDestroyed)
            {
                CustomDisable();
                return;
            }

            if (justCreated)
            {
                InitializeLate(_currentEditor);
            }

            using (HorizontalScope horizontalScope = new HorizontalScope(EditorStyles.helpBox))
            {
                using (VerticalScope verticalScope = new VerticalScope())
                {
                    SetSolverIteration();
                    SetSleepThreshold();
                    SetMaxDepenetation();
                }
            }

            if (_currentEditor.targets.Length < 2)
            {
                _centerOfMass.CustomOnInspectorGUI();
            }
        }

        private void SetSleepThreshold()
        {
            using (HorizontalScope horizontalScope = new HorizontalScope())
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
                    _currentTarget.sleepThreshold = RigidBodyAdditionalMonobehaviourSettings.DEFAULT_SLEEP_THRESHOLD;
                    _specialSettings.SetSleepThreshold(_currentTarget.sleepThreshold);
                }
            }
        }

        private void SetSolverIteration()
        {
            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                int solver = ExtGUIFloatFields.IntField(_currentTarget.solverIterations, null, out bool solverChanged, "SolverIteration", "More you have, more precise you are, more it cost");
                if (solverChanged)
                {
                    _currentTarget.solverIterations = solver;
                    _specialSettings.SetSolverIteration(_currentTarget.solverIterations);
                }
                GUILayout.Label("");
                if (GUILayout.Button("Default", GUILayout.Width(70)))
                {
                    _currentTarget.solverIterations = RigidBodyAdditionalMonobehaviourSettings.DEFAULT_SOLVER_ITERATION;
                    _specialSettings.SetSolverIteration(_currentTarget.solverIterations);
                }
            }
            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                int solver = ExtGUIFloatFields.IntField(_currentTarget.solverVelocityIterations, null, out bool solverChanged, "SolverVelocityIteration", "More you have, more precise you are, more it cost");
                if (solverChanged)
                {
                    _currentTarget.solverVelocityIterations = solver;
                    _specialSettings.SetSolverVelocityIteration(_currentTarget.solverVelocityIterations);
                }
                GUILayout.Label("");
                if (GUILayout.Button("Default", GUILayout.Width(70)))
                {
                    _currentTarget.solverVelocityIterations = RigidBodyAdditionalMonobehaviourSettings.DEFAULT_SOLVER_VELOCITY_ITERATION;
                    _specialSettings.SetSolverVelocityIteration(_currentTarget.solverVelocityIterations);
                }
            }
        }

        private void SetMaxDepenetation()
        {
            using (HorizontalScope horizontalScope = new HorizontalScope())
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
                    _currentTarget.maxDepenetrationVelocity = RigidBodyAdditionalMonobehaviourSettings.DEFAULT_MAX_DEPENETRATION_VELOCITY;
                    _specialSettings.SetMaxDepenetrationVelocity(_currentTarget.maxDepenetrationVelocity);
                }
            }
        }
    }
}