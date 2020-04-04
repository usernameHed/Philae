using hedCommon.extension.runtime;
using hedCommon.time;
using FluffyUnderware.Curvy;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using hedCommon.mixed;
using extUnityComponents;

namespace hedCommon.splines
{
    [ExecuteInEditMode]
    public class CustomSplineController : MonoBehaviour, IHaveDecoratorOnIt
    {
        [Serializable]
        public class ListLinkedSplineController
        {
            [Serializable]
            public class SplineControllerInfo
            {
                public CustomSplineController OtherController;
                public float AddedDistance;
            }

            public bool HasLinkedSplineController { get { return (OtherSplineControllers.Count != 0); } }
            public List<SplineControllerInfo> OtherSplineControllers;
        }

        [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
        private float _speed = 120f;
        public float Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
            }
        }
        [FoldoutGroup("GamePlay"), Tooltip("set to 1 for normal, -1 for inverse"), SerializeField]
        public float InverseDirection = 1f;

        [FoldoutGroup("GamePlay"), Tooltip("")]
        public bool AutomaticMove = false;

        [FoldoutGroup("GamePlay"), Tooltip("")]
        public bool HasChanged = false;


#if UNITY_EDITOR
        [FoldoutGroup("GamePlay"), Tooltip("")]
        public bool UpdatePositionInEditor = true;
        [FoldoutGroup("GamePlay"), Tooltip("")]
        public bool LockToCurrentWorldPosition = true;

        [FoldoutGroup("GamePlay"), Tooltip("")]
        public bool MoveByItSelfInEditor = false;

        [Serializable]
        public struct SnapAngle
        {
            public bool Snap;
            public float Angle;
        }

        [FoldoutGroup("GamePlay"), Tooltip("")]
        public SnapAngle SnapToAngle;

        [FoldoutGroup("GamePlay"), Tooltip("")]
        public bool ShowDatasInEditorWindow = true;


        [FoldoutGroup("GamePlay"), Tooltip("")]
        public SplineOptions SplineOptions;
#endif
        [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
        public bool PositionLocal = false;

        [FoldoutGroup("Offset"), Tooltip(""), Range(-180, 180), SerializeField]
        private float _offsetAngle = 0;
        public float OffsetAngle
        {
            get
            {
                return _offsetAngle;
            }
        }
        public void SetOffsetAngle(float angle, bool forceUpdate)
        {
            _offsetAngle = angle;
            if (forceUpdate)
            {
                CustomUpdate(false, TimeEditor.fixedDeltaTime);
            }
        }

        [FoldoutGroup("Offset"), Tooltip(""), SerializeField]
        private float _offsetRadius = 0;
        public float OffsetRadius
        {
            get
            {
                return _offsetRadius;
            }
        }
        public void SetOffsetRadius(float radius, bool forceUpdate)
        {
            _offsetRadius = radius;
            if (forceUpdate)
            {
                CustomUpdate(false, TimeEditor.fixedDeltaTime);
            }
        }

        [FoldoutGroup("Offset"), Tooltip("")]
        public float BaseOffsetUp = 0;

        [FoldoutGroup("Object"), Tooltip(""), SerializeField]
        private CurvySpline _curvySpline = default;
        public CurvySpline CurvySpline
        {
            get
            {
                return _curvySpline;
            }
            set
            {
                _curvySpline = value;
            }
        }

        #region IHaveDecoratorOnIt
        [FoldoutGroup("IHaveDecoratorOnIt"), SerializeField]
        private bool _showExtensions = true;
        [FoldoutGroup("IHaveDecoratorOnIt"), SerializeField]
        private bool _showTinyEditorWindows = true;
        public Component GetReferenceDecoratorsComponent() => this;

        public bool ShowExtension { get => _showExtensions; set => _showExtensions = value; }
        public bool ShowTinyEditorWindow { get => _showTinyEditorWindows; set => _showTinyEditorWindows = value; }
        #endregion


        [FoldoutGroup("Object"), Tooltip(""), SerializeField, FormerlySerializedAs("_toMove")]
        public Transform ToMove = default;

        [FoldoutGroup("Debug"), Tooltip(""), SerializeField]
        private float _currentDist = 0.5f;

        [FoldoutGroup("Debug"), Tooltip(""), Range(0f, 1f), OnValueChanged("PercentChangedFromInpector"), SerializeField]
        private float _percentInSpline = 0f;

        [FoldoutGroup("Debug"), Tooltip("")]
        public ListLinkedSplineController LinkedSplineController = new ListLinkedSplineController();

        [FoldoutGroup("Debug"), Tooltip(""), SerializeField]
        public bool IsMovableOnPlay = false;

        [FoldoutGroup("Editor"), Tooltip("")]
        public bool IsMovingInEditor = false;

        private Vector3 splineOffset = Vector3.zero;
        private Vector3 _oldPosition = ExtVector3.GetNullVector();

        public delegate void ActionWhenChangeChunk(int newIndex);

        

        public void Init(CurvySpline curvySpline)
        {
            _curvySpline = curvySpline;
        }

        

#if UNITY_EDITOR
        private void OnEnable()
        {
            if (ToMove == null)
            {
                ToMove = gameObject.transform;
            }
        }

        private void Awake()
        {
            if (SplineOptions == null)
            {
                SplineOptions = ExtFind.GetScript<SplineOptions>();
            }
            if (SplineOptions && _curvySpline == null)
            {
                _curvySpline = SplineOptions.GetDefaultSplineForCustomSplineController();
            }
        }

#endif

        private void Start()
        {
            if (_curvySpline == null)
            {
                return;
            }

            splineOffset = _curvySpline.transform.localPosition;
        }

        /// <summary>
        /// tell the controller to move or not
        /// </summary>
        public void PlayOrPause()
        {
            AutomaticMove = !AutomaticMove;
        }
        public void Play()
        {
            AutomaticMove = true;
        }
        public void Pause()
        {
            AutomaticMove = false;
        }

        #region Usefull static function

        /// <summary>
        /// from a set of keys, calculate the index prev & next where the player is in between
        /// if calculatePercent is true and there is more than 2 keys, also calculate the global percent & local percent
        /// </summary>
        /// <param name="keysOnTheSameSpline">set of keys</param>
        /// <param name="currentPercent">current percent of the player</param>
        /// <param name="calculatePercent">do we calculate the percentage ?</param>
        /// <param name="prevIndex">the previous key from the player</param>
        /// <param name="nextIndex">the next key from the player</param>
        /// <param name="totalPercentChunk">total percent from all the key</param>
        /// <param name="percentPlayerFromIndex">percent from the prev to last key</param>
        public static void GetPrevAndNextFromPlayer(CustomSplineController[] keysOnTheSameSpline,
            float currentPercent,
            bool calculatePercent,
            ref int prevIndex,
            ref int nextIndex,
            ref float totalPercentChunk,
            ref float percentPlayerFromIndex,
            bool hardReset,
            ActionWhenChangeChunk actionToCall)
        {
            if (keysOnTheSameSpline.Length == 0)
            {
                return;
            }

            int savePreviousIndex = prevIndex;

            if (currentPercent < keysOnTheSameSpline[0].GetPercent()
                || currentPercent >= keysOnTheSameSpline[keysOnTheSameSpline.Length - 1].GetPercent())
            {
                Debug.Log(currentPercent + " < " + keysOnTheSameSpline[0].GetPercent() + " || " + currentPercent + ">= " + keysOnTheSameSpline[keysOnTheSameSpline.Length - 1].GetPercent());

                prevIndex = keysOnTheSameSpline.Length - 1;
                nextIndex = 0;
                if (calculatePercent && keysOnTheSameSpline.Length > 1)
                {
                    totalPercentChunk = keysOnTheSameSpline[0].GetPercent() + (1 - keysOnTheSameSpline[keysOnTheSameSpline.Length - 1].GetPercent());
                    if (currentPercent < keysOnTheSameSpline[0].GetPercent())
                    {
                        percentPlayerFromIndex = currentPercent + (1 - keysOnTheSameSpline[keysOnTheSameSpline.Length - 1].GetPercent());
                    }
                    else
                    {
                        percentPlayerFromIndex = currentPercent - keysOnTheSameSpline[keysOnTheSameSpline.Length - 1].GetPercent();
                    }
                }
            }
            else
            {
                Debug.Log("in second quarter");
                for (int i = 0; i < keysOnTheSameSpline.Length - 1; i++)
                {
                    if (currentPercent >= keysOnTheSameSpline[i].GetPercent()
                        && currentPercent < keysOnTheSameSpline[i + 1].GetPercent())
                    {
                        prevIndex = i;
                        nextIndex = i + 1;
                        if (calculatePercent && keysOnTheSameSpline.Length > 1)
                        {
                            totalPercentChunk = keysOnTheSameSpline[i + 1].GetPercent() - keysOnTheSameSpline[i].GetPercent();
                            percentPlayerFromIndex = currentPercent - keysOnTheSameSpline[i].GetPercent();
                        }
                        break;
                    }
                }
            }

            if (savePreviousIndex != prevIndex || hardReset)
            {
                actionToCall(prevIndex);
            }
        }

        /// <summary>
        /// from a set of key in an array, return the one closest to an reference key
        /// </summary>
        /// <param name="keyReference">the reference key from which we want to get the closest one</param>
        /// <param name="allKeyToTest">all the key to test</param>
        /// <returns></returns>
        public static CustomSplineController GetClosestKeyFromAList(CustomSplineController keyReference, CustomSplineController[] allKeyToTest, out int index, float marginFromReferenceKeyInDistance = 0)
        {
            index = -1;

            float[] closest = new float[allKeyToTest.Length];
            for (int i = 0; i < allKeyToTest.Length; i++)
            {
                closest[i] = CustomSplineController.GetDistBetween2Controllers(keyReference, allKeyToTest[i], true);
                closest[i] += marginFromReferenceKeyInDistance;

                if (closest[i] >= keyReference.CurvySpline.Length - 0.001)
                {
                    closest[i] = 0;
                }
            }
            ExtMathf.GetClosestValueFromAnother(0, closest, out int indexFound);
            if (indexFound == -1)
            {
                return (null);
            }

            index = indexFound;
            return (allKeyToTest[index]);
        }

        /// <summary>
        /// get a world position by a distance
        /// </summary>
        /// <param name="spline"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public static Vector3 GetWorldPosByDistance(CurvySpline spline, float dist)
        {
            Vector3 posByDist = spline.InterpolateByDistanceFast(dist);
            posByDist = spline.transform.TransformPoint(posByDist);
            return (posByDist);
        }

        public static Vector3 GetWorldPosByPercent(CurvySpline spline, float percent)
        {
            Vector3 posByPercent = spline.InterpolateFast(percent);
            posByPercent = spline.transform.TransformPoint(posByPercent);

            return (posByPercent);
        }
        public static Quaternion GetWorldRotationByPercent(CurvySpline spline, float percent)
        {
            Quaternion rotation = spline.GetOrientationFast(percent);
            return (rotation);
        }

        /// <summary>
        /// get the distance between the 2 objects
        /// </summary>
        public static float GetDistMagnitudeBetweenTwoControllers(CustomSplineController one, CustomSplineController two)
        {
            return (Vector3.Magnitude(one.ToMove.position - two.ToMove.position));
        }

        /// <summary>
        /// from a given point, find the closest point in the spline
        /// </summary>
        public static float GetPercentFromWorldPos(Vector3 worldPos, out Vector3 closestPosition, CurvySpline curvySpline)
        {
            Vector3 posRelative = curvySpline.transform.InverseTransformPoint(worldPos);

            float nearest = curvySpline.GetNearestPointTF(posRelative, out closestPosition);
            return (nearest);
        }
        /// <summary>
        /// from a given point, find the closest point in the spline
        /// </summary>
        public static float GetDistFromWorldPos(Vector3 worldPos, ref Vector3 closestPosition, CurvySpline curvySpline)
        {
            Vector3 posRelative = curvySpline.transform.InverseTransformPoint(worldPos);

            float nearest = curvySpline.GetNearestPointTF(posRelative, out closestPosition);
            float dist = curvySpline.TFToDistance(nearest);
            return (dist);
        }

        /// <summary>
        /// from a given dist/percent of a controller, tell if the controllerDist to test is "after" this
        /// </summary>
        public static bool IsAfterThisController(float thisControllerDistance, float controllerDistToTest, float lenghtSpline, float margin = 0)
        {
            thisControllerDistance = CustomSplineController.RepairLenght(thisControllerDistance + margin, lenghtSpline);

            if (lenghtSpline == 0)
            {
                Debug.LogWarning("warning 0 lenght spline !");
                return (false);
            }

            if (thisControllerDistance < controllerDistToTest)
            {
                if (controllerDistToTest - thisControllerDistance < (lenghtSpline / 2))
                {
                    return (true);  //after
                }
                else
                {
                    return (false); //before
                }
            }
            else
            {
                float addHalfLenght = thisControllerDistance + (lenghtSpline / 2);
                if (addHalfLenght < lenghtSpline)
                {
                    return (false); //we are before
                }
                else
                {
                    float valueBefore = lenghtSpline - thisControllerDistance;
                    float valueAfter = (lenghtSpline / 2) - valueBefore;
                    if (controllerDistToTest < valueAfter)
                    {
                        return (true);  //after
                    }
                    else
                    {
                        return (false);//before
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static float RepairPercent(float percent)
        {
            if (percent < 0)
            {
                percent += 1;
            }
            if (percent > 1)
            {
                percent %= 1f;
            }
            return (percent);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static float RepairLenght(float lenght, CurvySpline curvySpline)
        {
            float lenghtSpline = curvySpline.Length;
            if (lenghtSpline.IsNaN() || lenghtSpline == 0)
            {
                curvySpline.Refresh();
                lenghtSpline = curvySpline.Length;
            }
            if (lenght < 0)
            {
                lenght += lenghtSpline;
            }
            if (lenghtSpline == 0)
            {
                Debug.LogWarning("Spline not init, return !");
                return (lenght);
            }


            if (lenght > lenghtSpline)
            {
                lenght %= lenghtSpline;
            }
            return (lenght);
        }
        public static float RepairLenght(float lenght, float lenghtSpline)
        {
            if (lenghtSpline.IsNaN() || lenghtSpline == 0)
            {
                Debug.LogError("here wrong function, try to pass in parametter the spline itself");
                return (0);
            }
            if (lenght < 0)
            {
                lenght += lenghtSpline;
            }
            if (lenghtSpline == 0)
            {
                Debug.LogWarning("Spline not init, return !");
                return (lenght);
            }


            if (lenght > lenghtSpline)
            {
                lenght %= lenghtSpline;
            }
            return (lenght);
        }

        /// <summary>
        /// get the distance between 2 controllers (clockwise !)
        /// the 2 controllers had to have the same Spline in common !
        /// 
        /// twoSides:
        /// false: calculate clockWise: from the first, to the second, alond the Spline.
        /// (if the second Controller is just behind the first, and if the lenght is 4000.
        ///  the distance will be like 3995. And not 5 !)
        ///  
        /// true: return the relative distance, with the example I given, the distance will be 5.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="twoSides">if it's true: whatever the position is behind or in front of, return the distance. If not, return the dist clockwise from the first to the second</param>
        /// <returns></returns>
        public static float GetDistBetween2Controllers(CustomSplineController first, CustomSplineController second, bool clockwise = true)
        {
            if (first.CurvySpline == null)
            {
                return (0);
            }

            if (!clockwise)
            {
                return (GetRealDistBetween2Values(first.GetDistance(), second.GetDistance(), first.CurvySpline.Length));
            }
            return (GetDistClockWiseBetween2Values(first.GetDistance(), second.GetDistance(), first.CurvySpline.Length));
        }

        /// <summary>
        /// get the distance between 2 controllers (clockwise !)
        /// the 2 controllers had to have the same Spline in common !
        /// 
        /// twoSides:
        /// false: calculate clockWise: from the first, to the second, alond the Spline.
        /// (if the second Controller is just behind the first, and if the lenght is 4000.
        ///  the distance will be like 3995. And not 5 !)
        ///  
        /// true: return the relative distance, with the example I given, the distance will be 5.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="twoSides">if it's true: whatever the position is behind or in front of, return the distance. If not, return the dist clockwise from the first to the second</param>
        /// <returns></returns>
        public static float GetPercentBetween2Controllers(CustomSplineController first, CustomSplineController second, bool clockwise = true)
        {
            if (!clockwise)
            {
                return (GetRealDistBetween2Values(first.GetPercent(), second.GetPercent(), 1f));
            }
            return (GetDistClockWiseBetween2Values(first.GetPercent(), second.GetPercent(), 1f));
        }

        /// <summary>
        /// Get the distance between 2 dist
        /// if you want percentage and not distance, set 1 to maxDist
        /// </summary>
        /// <param name="firstDist"></param>
        /// <param name="secondDist"></param>
        /// <returns></returns>
        public static float GetDistClockWiseBetween2Values(float firstDist, float secondDist, float maxDist)
        {
            //here the first is bellow 0
            if (firstDist > secondDist)
            {
                float distToZero = maxDist - firstDist;
                float finalDist = distToZero + secondDist;
                return (finalDist);
            }
            //here normal: 1, and then second
            else
            {
                float finalDist = secondDist - firstDist;
                return (finalDist);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstDist"></param>
        /// <param name="secondDist"></param>
        /// <param name="maxDist"></param>
        /// <returns></returns>
        public static float GetRealDistBetween2Values(float firstDist, float secondDist, float maxDist)
        {
            if (maxDist == 0)
            {
                Debug.LogError("warning 0 lenght spline !");
                return (0);
            }

            if (firstDist < secondDist)
            {
                if (secondDist - firstDist < (maxDist / 2))
                {
                    return (GetDistClockWiseBetween2Values(firstDist, secondDist, maxDist));  //after
                }
                else
                {
                    return (GetDistClockWiseBetween2Values(secondDist, firstDist, maxDist)); //before
                }
            }
            else
            {
                float addHalfLenght = firstDist + (maxDist / 2);
                if (addHalfLenght < maxDist)
                {
                    return (GetDistClockWiseBetween2Values(secondDist, firstDist, maxDist)); //we are before
                }
                else
                {
                    float valueBefore = maxDist - firstDist;
                    float valueAfter = (maxDist / 2) - valueBefore;
                    if (secondDist < valueAfter)
                    {
                        return (GetDistClockWiseBetween2Values(firstDist, secondDist, maxDist));  //after
                    }
                    else
                    {
                        return (GetDistClockWiseBetween2Values(secondDist, firstDist, maxDist));//before
                    }
                }
            }
        }

        #endregion

        public void SetClosestPositionTo(Vector3 position)
        {
            float percent = CustomSplineController.GetPercentFromWorldPos(position, out Vector3 closestPosition, _curvySpline);
            SetPercent(percent, true, CurvyClamping.Loop);
        }

        /// <summary>
        /// set the percent, and force to move the player if needed
        /// </summary>
        /// <param name="percent">from 0 to 1</param>
        /// <param name="update">force update</param>
        public void SetPercent(float percent, bool update, CurvyClamping curvyClamping = CurvyClamping.Clamp)
        {
            if (_curvySpline == null)
            {
                return;
            }

            _currentDist = _curvySpline.TFToDistance(percent, curvyClamping);
            _percentInSpline = percent;
            if (update)
            {
                CustomUpdate(false, TimeEditor.fixedDeltaTime);
                _oldPosition = ToMove.transform.position;
            }
        }
        public void SetDistance(float dist, bool update, CurvyClamping curvyClamping = CurvyClamping.Clamp)
        {
            if (_curvySpline == null || ToMove == null)
            {
                return;
            }

            _currentDist = dist;
            if (curvyClamping == CurvyClamping.Loop)
            {
                if (_currentDist < 0)
                {
                    _currentDist += _curvySpline.Length;
                }
                _currentDist %= _curvySpline.Length;
            }

            _percentInSpline = _curvySpline.DistanceToTF(_currentDist, curvyClamping);

            if (update)
            {
                CustomUpdate(false, TimeEditor.fixedDeltaTime);
                _oldPosition = ToMove.transform.position;
            }
        }

        /// <summary>
        /// get percent Normalized
        /// </summary>
        /// <returns>percent from 0 to 1</returns>
        public float GetPercent()
        {
            return (_percentInSpline);
        }
        public float GetPercentWithAhead(float percentAhead)
        {
            float newPercent = CustomSplineController.RepairPercent(_percentInSpline + percentAhead);
            return (newPercent);
        }
        /// <summary>
        /// get percent by distance
        /// </summary>
        /// <returns>return dist from 0 to Spline.Lenght</returns>
        public float GetDistance()
        {
            return (_currentDist);
        }
        public float GetPercentByDistanceWithAhead(float distAhead)
        {
            float newLenght = CustomSplineController.RepairLenght(_currentDist + distAhead, _curvySpline);
            return (newLenght);
        }

        /// <summary>
        /// update the Controllers percent & position
        /// </summary>
        /// <param name="move">do we move incrementaly ?</param>
        public void CustomUpdate(bool move, float deltaTime)
        {
            if (move)
            {
                Move(deltaTime);
            }
            ChangePosition();
        }

        public void ResetOffset()
        {
            SetOffsetAngle(0, false);
            SetOffsetRadius(0, true);
        }

        public void SetOffset(float angle, float radius, bool update)
        {
            SetOffsetAngle(angle, false);
            SetOffsetRadius(radius, update);
        }

        /// <summary>
        /// move along the spline incrementaly
        /// </summary>
        private void Move(float deltaTime)
        {
#if UNITY_EDITOR
            if (_curvySpline == null)
            {
                return;
            }
#endif

            _currentDist = (_currentDist + (_speed * InverseDirection * deltaTime));
            if (_currentDist >= _curvySpline.Length)
            {
                _currentDist -= _curvySpline.Length;
            }
            if (_currentDist < 0)
            {
                _currentDist += _curvySpline.Length;
            }
            _percentInSpline = _curvySpline.DistanceToTF(_currentDist);
        }

        /// <summary>
        /// set the position of the SplineController from his percent value,
        /// and apply the good offsets
        /// </summary>
        [Button]
        public void ChangePosition()
        {
#if UNITY_EDITOR
            if (_curvySpline == null)
            {
                return;
            }
            splineOffset = _curvySpline.transform.localPosition;

            //in editor, actualize the percent to match the distance ! (if we change the spline of the player, we need to update this !)
            _percentInSpline = _curvySpline.DistanceToTF(_currentDist);
#endif

            if (ToMove == null)
            {
                return;
            }

            Vector3 position = _curvySpline.InterpolateByDistanceFast(_currentDist) + splineOffset;
            Quaternion rotation = _curvySpline.GetOrientationFast(_curvySpline.DistanceToTF(_currentDist));
            if (!PositionLocal)
            {
                position += _curvySpline.transform.position;
                rotation *= _curvySpline.transform.localRotation;

            }
            ToMove.position = position;
            ToMove.rotation = rotation;

            ToMove.position = ApplyOffset(ToMove.position, ToMove.forward, ToMove.up, _offsetAngle, _offsetRadius);

            ApplyChangeToAllLinkedControllers();
        }

        private void ApplyChangeToAllLinkedControllers()
        {
            if (LinkedSplineController.OtherSplineControllers == null)
            {
                return;
            }
            for (int i = 0; i < LinkedSplineController.OtherSplineControllers.Count; i++)
            {
                if (LinkedSplineController.OtherSplineControllers[i].OtherController == null)
                {
                    continue;
                }
                if (LinkedSplineController.OtherSplineControllers[i].OtherController.CurvySpline == _curvySpline)
                {
                    LinkedSplineController.OtherSplineControllers[i].OtherController.SetDistance(GetDistance() + LinkedSplineController.OtherSplineControllers[i].AddedDistance, true, CurvyClamping.Loop);
                }
                else if (LinkedSplineController.OtherSplineControllers[i].OtherController.CurvySpline != null)
                {
                    Vector3 closestPos = ToMove.position;
                    float dist = CustomSplineController.GetDistFromWorldPos(_oldPosition, ref closestPos, LinkedSplineController.OtherSplineControllers[i].OtherController.CurvySpline);

                    LinkedSplineController.OtherSplineControllers[i].OtherController.SetDistance(dist + LinkedSplineController.OtherSplineControllers[i].AddedDistance, true, CurvyClamping.Loop);
                }
            }
        }


        private void PercentChangedFromInpector()
        {
            _currentDist = _curvySpline.TFToDistance(_percentInSpline);
            CustomUpdate(true, TimeEditor.fixedDeltaTime);
        }

        /// <summary>
        /// change the default offset up, and reload changes
        /// </summary>
        /// <param name="baseOfsetUp"></param>
        public void ChangeBaseOffsetUp(float baseOfsetUp)
        {
            BaseOffsetUp = baseOfsetUp;
            CustomUpdate(false, TimeEditor.fixedDeltaTime);
        }

        /// <summary>
        /// apply the offset
        /// </summary>
        private Vector3 ApplyOffset(Vector3 pos, Vector3 forward, Vector3 up, float angle, float radius)
        {
            //apply offset
            Quaternion R = Quaternion.AngleAxis(angle, forward);
            Vector3 finalPos = pos + (R * up) * radius;

            //apply default offset up
            Quaternion Default = Quaternion.AngleAxis(0, forward);
            Vector3 finalPosDefault = finalPos + (Default * up) * BaseOffsetUp;

            return (finalPosDefault);
        }

#if UNITY_EDITOR
        /// <summary>
        /// try to stay where we are, even if the spline move
        /// </summary>
        private void TryToKeepPosition()
        {
            if (_oldPosition == ExtVector3.GetNullVector())
            {
                _oldPosition = ToMove.transform.position;
            }
            ChangePosition();

            if (!ExtVector3.IsClose(_oldPosition, ToMove.transform.position, 0.01f))
            {
                //stay where we are !
                Vector3 closestPos = Vector3.zero;
                float dist = CustomSplineController.GetDistFromWorldPos(_oldPosition, ref closestPos, _curvySpline);

                SetDistance(dist, true);
            }
        }
#endif

        private void Update()
        {
#if UNITY_EDITOR
            if (SplineOptions == null)
            {
                SplineOptions = ExtFind.GetScript<SplineOptions>();
            }
            if (SplineOptions != null)
            {
                BaseOffsetUp = SplineOptions.BaseOffsetUp;
            }
            if (MoveByItSelfInEditor)
            {
                Move(TimeEditor.fixedDeltaTime);
            }
#endif


            if (HasChanged)
            {
                ChangePosition();
                HasChanged = false;
            }


            if (!IsMovingInEditor)
            {
                if (
#if UNITY_EDITOR
                !LockToCurrentWorldPosition ||
#endif
                AutomaticMove)
                {
                    ChangePosition();
                    _oldPosition = ToMove.transform.position;
                }
#if UNITY_EDITOR
                else if (_curvySpline != null)
                {
                    TryToKeepPosition();
                }
#endif
            }
            else
            {
                _oldPosition = ToMove.transform.position;
            }
        }

    }
}