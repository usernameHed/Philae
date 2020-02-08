using hedCommon.extension.runtime;
using hedCommon.singletons;
using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;

namespace hedCommon.time
{
    /// <summary>
    /// Use this class instead of the Time class if you want to use time in editor
    /// use TimeEditor.time instead of Time.time will work in editor & in play mode
    ///
    /// use TimeEditor.time instead of Time.time
    /// use TimeEditor.timeScale instead of Time.timeScale
    /// use TimeEditor.unscaledTime instead of Time.unscaledTime
    /// use TimeEditor.deltaTime instead of Time.deltaTime
    /// use TimeEditor.unscaledDeltaTime instead of Time.unscaledDeltaTime
    /// 
    /// In editor, you can set the timeScale to negative to backward time !
    /// but warning: doen't work in play mode
    /// </summary>
    [ExecuteInEditMode]
    public class TimeEditor : SingletonMono<TimeEditor>
    {
        //private float _editModeLastUpdate;   //the last time the controller was updated while in Edit Mode
        private float _timeScale = 1f;       //current timeScale in both editor & play mode
        private float _currentTime;          //current time timeScale-dependent (like Time.time but in editMode)
        private float _currentTimeIndependentTimeScale;  //current time timeScale-independent (like Time.unscaledTime but in editMode)
        private float _currentDeltaTime;     //current deltaTime timeScale-dependent (like Time.deltaTime but in editMode)
        private float _currentUnscaledDeltaTime; //current deltaTime timeScale-independent (like Time.unscaledDeltaTime but in editMode)
        private float _smoothDeltaTime;      //current deltatime, smoothed X time
        private float _smoothUnscaledDeltaTime;      //current deltatime, smoothed X time

        private Queue<float> _olfDeltaTimes = new Queue<float>();         //list of all previousDeltaTime;
        private Queue<float> _olfUnscaledDeltaTimes = new Queue<float>();         //list of all previousDeltaTime;

        private readonly int _maxAmountDeltaTimeSaved = 10; //amount of previous deltaTime to save for the smoothDeltaTime algorythm

        //don't let the deltaTime get higher than 1/30: if you have low fps (bellow 30),
        //the game start to go in slow motion.
        //if you don't want this behavior, put the value at 0.4 for exemple as precaution, we don't
        //want the player, after a huge spike of 5 seconds, to travel walls !
        private readonly float _maxSizeDeltaTime = 0.033f;
        //private readonly float _maxSizeDeltaTime = 0.033f * 1000;

        private bool _isInHyperSampling = false;
        public bool IsInHyperSampling { get { return (_isInHyperSampling); } }

        #region public properties

        /// <summary>
        /// get the current timeScale
        /// </summary>
        public static float timeScale
        {
            get
            {
                return (Instance._timeScale);
            }
            set
            {
                if (value != Instance._timeScale)
                {
                    Instance._timeScale = value;
                    Time.timeScale = Mathf.Max(0, Instance._timeScale);
                }
            }
        }
        /// <summary>
        /// the time (timeScale dependent) at the begening of this frame (Read only). This is the time in seconds since the start of the game / the editor compilation
        /// </summary>
        public static float time
        {
            get
            {
                return (Instance._currentTime);
            }
        }

        public static float fixedTime
        {
            get
            {
                return (Time.fixedTime);
            }
        }

        public static float unscaledFixedTime
        {
            get
            {
                return (Time.fixedUnscaledTime);
            }
        }

        /// <summary>
        /// The timeScale-independant time for this frame (Read Only). This is the time in seconds since the start of the game / the editor compilation
        /// </summary>
        public static float unscaledTime
        {
            get
            {
                return (Instance._currentTimeIndependentTimeScale);
            }
        }

        /// <summary>
        /// The completion time in seconds since the last from (Read Only)
        /// </summary>
        public static float deltaTime
        {
            get
            {
                return (Instance._currentDeltaTime);
            }
        }

        /// <summary>
        /// The timeScale-independent interval in seconds from the last frames to the curren tone
        /// </summary>
        public static float unscaledDeltaTime
        {
            get
            {
                return (Instance._currentUnscaledDeltaTime);
            }
        }

        /// <summary>
        /// The timeScale-dependent smoothed DeltaTime, it's the average of the 'n = 10' previous frames
        /// </summary>
        public static float smoothDeltaTime
        {
            get
            {
                if (Instance._timeScale == 0)
                {
                    return (0);
                }

                return (Instance._smoothDeltaTime);
            }
        }

        public static float DeltaTimeEditorAndRunTime
        {
            get
            {
                if (!Application.isPlaying)
                {
                    return (TimeEditor.smoothDeltaTime);
                }
                else
                {
                    return (Time.fixedDeltaTime);
                }
            }
        }

        public static float TimeEditorAndRunTime
        {
            get
            {
                if (!Application.isPlaying)
                {
                    return (TimeEditor.time);
                }
                else
                {
                    return (TimeEditor.fixedTime);
                }
            }
        }

        public static float TimeUnscaledEditorAndRunTime
        {
            get
            {
                if (!Application.isPlaying)
                {
                    return (TimeEditor.unscaledTime);
                }
                else
                {
                    return (TimeEditor.unscaledFixedTime);
                }
            }
        }

        /// <summary>
        /// The timeScale-independent smoothed DeltaTime, it's the average of the 'n = 10' previous frames
        /// </summary>
        public static float smoothUnscaledDeltaTime
        {
            get
            {
                return (Instance._smoothUnscaledDeltaTime);
            }
        }
        #endregion

        #region private functions

        /// <summary>
        /// subscribe to EditorApplication.update to be able to call EditorApplication.QueuePlayerLoopUpdate();
        /// We need to do it because unity doens't update when no even are triggered in the scene.
        /// 
        /// Then, Start the timer, this timer will increase every frame (in play or in editor mode), like the normal Time
        /// </summary>
        private void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.update += EditorUpdate;
#endif
            StartCoolDown();
        }

        protected void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// called every editorUpdate, tell unity to execute the Update() method
        /// even if no event are triggered in the scene
        /// in the scene.
        /// </summary>
        private void EditorUpdate()
        {
            if (!Application.isPlaying)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
#endif

        /// <summary>
        /// at start, we initialize the current time
        /// </summary>
        public void StartCoolDown(bool hyperSampling = false)
        {
            _isInHyperSampling = hyperSampling;
            _currentTime = 0;
            _currentTimeIndependentTimeScale = 0;
        }

        public void StopHyperSampling()
        {
            _isInHyperSampling = false;
        }

        public void AdvanceInHyperSampling(float deltaTime = 0.016f)
        {
            AdvanceFromOneFrame(deltaTime);
        }

        /// <summary>
        /// called every frame in play and in editor, thanks to EditorApplication.QueuePlayerLoopUpdate();
        /// add to the current time, then save the current time for later.
        /// </summary>
        private void Update()
        {
            if (_isInHyperSampling)
            {
                return;
            }
            AdvanceFromOneFrame(Time.unscaledDeltaTime);
        }

        private void AdvanceFromOneFrame(float deltaTime)
        {
            CalculateDeltaTimes(deltaTime);
            AdvanceTime();
        }

        private void CalculateDeltaTimes(float deltaTime)
        {
            _currentUnscaledDeltaTime = deltaTime;
            _currentDeltaTime = _currentUnscaledDeltaTime * timeScale;

            ClampDeltaTime();
            
            SetSmoothDeltaTimes();
        }

        /// <summary>
        /// Don't allow fps to drop bellow _maxSizeDeltaTime (30FPS for 0.033f)
        /// if we are in hyperSampling, don't clamp.
        /// </summary>
        private void ClampDeltaTime()
        {
            if (!_isInHyperSampling)
            {
                _currentDeltaTime = Mathf.Min(_currentDeltaTime, _maxSizeDeltaTime);
            }
        }

        /// <summary>
        /// Set and manage the smoothdeltaTime, by adding the average of the 'n' previous deltaTimes together
        /// </summary>
        private void SetSmoothDeltaTimes()
        {
            float sumOfPreviousDeltaTimes = _olfDeltaTimes.GetSum();
            float sumOfPreviousUnslacedDeltaTime = _olfUnscaledDeltaTimes.GetSum();

            _smoothDeltaTime = (_currentDeltaTime + (sumOfPreviousDeltaTimes)) / (_olfDeltaTimes.Count + 1);
            _smoothUnscaledDeltaTime = (_currentUnscaledDeltaTime + (sumOfPreviousUnslacedDeltaTime)) / (_olfUnscaledDeltaTimes.Count + 1);
            //_smoothDeltaTime = (_currentDeltaTime + (sum of 'n' previous delta times)) / ('n' + 1)

            _olfDeltaTimes.Enqueue(_currentDeltaTime);
            _olfUnscaledDeltaTimes.Enqueue(_currentUnscaledDeltaTime);

            while (_olfDeltaTimes.Count > _maxAmountDeltaTimeSaved)
            {
                _olfDeltaTimes.Dequeue();
            }

            while (_olfUnscaledDeltaTimes.Count > _maxAmountDeltaTimeSaved)
            {
                _olfUnscaledDeltaTimes.Dequeue();
            }
        }

        /// <summary>
        /// called every frame, add delta time to the current timer, with or not timeScale
        /// </summary>
        private void AdvanceTime()
        {
            _currentTime += _currentDeltaTime;
            _currentTimeIndependentTimeScale += _currentUnscaledDeltaTime;
        }

        #endregion
    }
}