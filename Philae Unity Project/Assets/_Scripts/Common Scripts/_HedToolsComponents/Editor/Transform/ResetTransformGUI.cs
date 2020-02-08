using hedCommon.extension.editor;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace ExtUnityComponents.transform
{
    public class ResetTransformGUI
    {
        private Transform[] _concretTarget;

        public void Init(Transform[] concretTarget)
        {
            _concretTarget = concretTarget;
        }

        public void CustomDisable()
        {

        }

        /// <summary>
        /// when we click on reset, reset every target to there
        /// default state
        /// </summary>
        private void ApplyResetTransform(ref Transform[] concretTarget)
        {
            for (int i = 0; i < concretTarget.Length; i++)
            {
                ExtUndo.Record(concretTarget[i], "Reset tool transform");
                concretTarget[i].localPosition = Vector3.zero;
                concretTarget[i].localRotation = Quaternion.identity;
                concretTarget[i].localScale = Vector3.one;
            }
        }

        /// <summary>
        /// if every position are at 0, we can't reset position
        /// </summary>
        /// <returns></returns>
        private bool CanResetPosition()
        {
            for (int i = 0; i < _concretTarget.Length; i++)
            {
                if (_concretTarget[i] == null)
                {
                    continue;
                }
                if (_concretTarget[i].position != Vector3.zero)
                {
                    return (true);
                }
            }
            return (false);
        }

        /// <summary>
        /// if every rotation are at 0, we can't reset rotation
        /// </summary>
        /// <returns></returns>
        private bool CanResetRotation()
        {
            for (int i = 0; i < _concretTarget.Length; i++)
            {
                if (_concretTarget[i] == null)
                {
                    continue;
                }
                if (_concretTarget[i].rotation != Quaternion.identity)
                {
                    return (true);
                }
            }
            return (false);
        }

        /// <summary>
        /// if every position are at 0, we can't reset position
        /// </summary>
        /// <returns></returns>
        private bool CanResetScale()
        {
            for (int i = 0; i < _concretTarget.Length; i++)
            {
                if (_concretTarget[i] == null)
                {
                    continue;
                }
                if (_concretTarget[i].localScale != Vector3.one)
                {
                    return (true);
                }
            }
            return (false);
        }

        /// <summary>
        /// reset position
        /// </summary>
        /// <param name="concretTarget">all targets to reset</param>
        /// <param name="applyUndo">do we record the undo ?</param>
        private void ResetPosition(bool applyUndo)
        {
            EditorGUI.BeginDisabledGroup(!CanResetPosition());
            {
                if (GUILayout.Button("Position", EditorStyles.miniButton))
                {
                    for (int i = 0; i < _concretTarget.Length; i++)
                    {
                        if (applyUndo)
                        {
                            ExtUndo.Record(_concretTarget[i], "Reset tool position");
                        }
                        _concretTarget[i].localPosition = Vector3.zero;
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// reset position
        /// </summary>
        /// <param name="concretTarget">all targets to reset</param>
        /// <param name="applyUndo">do we record the undo ?</param>
        private void ResetAll(bool applyUndo)
        {
            bool canReset = CanResetPosition() || CanResetRotation() || CanResetScale();
            EditorGUI.BeginDisabledGroup(!canReset);
            {
                if (GUILayout.Button("All", EditorStyles.miniButton))
                {
                    for (int i = 0; i < _concretTarget.Length; i++)
                    {
                        if (applyUndo)
                        {
                            ExtUndo.Record(_concretTarget[i], "Reset tool transform");
                        }
                        _concretTarget[i].localPosition = Vector3.zero;
                        _concretTarget[i].localRotation = Quaternion.identity;
                        _concretTarget[i].localScale = Vector3.one;
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// reset position
        /// </summary>
        /// <param name="concretTarget">all targets to reset</param>
        /// <param name="applyUndo">do we record the undo ?</param>
        private void ResetRotation(bool applyUndo)
        {
            EditorGUI.BeginDisabledGroup(!CanResetRotation());
            {
                if (GUILayout.Button("Rotation", EditorStyles.miniButton))
                {
                    for (int i = 0; i < _concretTarget.Length; i++)
                    {
                        if (applyUndo)
                        {
                            ExtUndo.Record(_concretTarget[i], "Reset tool rotation");
                        }
                        _concretTarget[i].localRotation = Quaternion.identity;
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// reset position
        /// </summary>
        /// <param name="concretTarget">all targets to reset</param>
        /// <param name="applyUndo">do we record the undo ?</param>
        private void ResetScale(bool applyUndo)
        {
            EditorGUI.BeginDisabledGroup(!CanResetScale());
            {
                if (GUILayout.Button("Scale", EditorStyles.miniButton))
                {
                    for (int i = 0; i < _concretTarget.Length; i++)
                    {
                        if (applyUndo)
                        {
                            ExtUndo.Record(_concretTarget[i], "Reset tool localScale");
                        }
                        _concretTarget[i].localScale = Vector3.one;
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// display reset transform buttons
        /// </summary>
        public void CustomOnInspectorGUI()
        {
            using (HorizontalScope horizontalScope = new HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Reset:");

                ResetPosition(true);
                ResetRotation(true);
                ResetScale(true);
                ResetAll(true);
            }
        }
    }
}