using philae.data.gravity;
using hedCommon.extension.runtime;
using philae.gravity.attractor.logic;
using philae.gravity.zones;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using philae.gravity.graviton;
using philae.gravity.physicsBody;

namespace philae.gravity.attractor
{
    [ExecuteInEditMode]
    public abstract class Attractor : MonoBehaviour
    {
        [FoldoutGroup("Settings"), InlineEditor]
        public AttractorSettings SettingsGlobal = default;
        [FoldoutGroup("Settings")]
        public AttractorSettingsLocal SettingsLocal = new AttractorSettingsLocal();

        [SerializeField]
        protected RigidAttractor _rigidAttractor = default;

        //[SerializeField, ReadOnly]
        //protected Transform _parent;
        [SerializeField]
        protected List<AttractorListerLogic> _attractorListerLogic;
        //public GravityAttractorZone Zone { get {  return (_attractorListerLogic?.Zone); } }

        [SerializeField, ReadOnly]
        protected float _minRangeWithScale = 0f;
        [SerializeField, ReadOnly]
        protected float _maxRangeWithScale = 0f;

        [SerializeField, ReadOnly]
        protected List<Graviton> _gravitonsInside = new List<Graviton>(50);

        private Vector3 _oldScale = new Vector3(-1, -1, -1);

        public Vector3 Position { get { return (transform.position); } }

        /// <summary>
        /// CALLED ONLY when creating the attractor from editor, NEVER AFTER
        /// </summary>
        /// <param name="attractorListerLogic"></param>
        public virtual void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            _attractorListerLogic = attractorListerLogic;
            SetupLinkToAttractorListers();

            transform.hasChanged = false;
            _oldScale = new Vector3(-1, -1, -1);
            _rigidAttractor.IsKinematic = true;
            Init();
        }

        private void SetupLinkToAttractorListers()
        {
            for (int i = 0; i < _attractorListerLogic.Count; i++)
            {
                _attractorListerLogic[i].Lister.AddAttractor(this);
            }
        }

        private void RemoveLinkToAttractorListers()
        {
            for (int i = 0; i < _attractorListerLogic.Count; i++)
            {
                _attractorListerLogic[i].Lister.RemoveAttractor(this);
            }
        }

        /// <summary>
        /// here fill automaticly zones
        /// </summary>
        /// <returns></returns>
        public List<AttractorListerLogic> AutomaticlySetupZone()
        {
            RemoveLinkToAttractorListers();
            _attractorListerLogic = ZonesLister.Instance.GetZonesInWichAttractorIs(this);
            SetupLinkToAttractorListers();
            return (_attractorListerLogic);
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            SetupOnLoad();
        }
#endif

        public void Init()
        {
            SetupOnLoad();
        }

        private void SetupOnLoad()
        {
            SetupLinkToAttractorListers();

            _gravitonsInside.Clear();

            MinMaxRangeChanged();

            SettingsLocal.MinMaxRangeChanged -= MinMaxRangeChanged;
            SettingsLocal.MinMaxRangeChanged += MinMaxRangeChanged;
            SettingsLocal.GravityChanged -= OnGravityChanged;
            SettingsLocal.GravityChanged += OnGravityChanged;
            SettingsLocal.OnKinematicChanged -= OnKinematicChanged;
            SettingsLocal.OnKinematicChanged += OnKinematicChanged;

#if UNITY_EDITOR
            //_rigidAttractor.hideFlags = HideFlags.NotEditable;
#endif
        }

        private void OnDisable()
        {
            SettingsLocal.MinMaxRangeChanged -= MinMaxRangeChanged;
            SettingsLocal.GravityChanged -= OnGravityChanged;
            SettingsLocal.OnKinematicChanged -= OnKinematicChanged;

            RemoveLinkToAttractorListers();
        }

        private void MinMaxRangeChanged()
        {
            float maxScaleDimension = transform.localScale.Maximum();
            _minRangeWithScale = SettingsLocal.MinRange * maxScaleDimension;
            _maxRangeWithScale = SettingsLocal.MaxRange * maxScaleDimension;

            Move();
        }

        /// <summary>
        /// called after init, or when transform has changed
        /// </summary>
        public abstract void Move();

        private void OnGravityChanged()
        {

        }

        private void OnKinematicChanged()
        {
            _rigidAttractor.IsKinematic = SettingsLocal.IsKinematic;
        }

        protected void AddOrRemoveGravitonFromList(Graviton graviton, bool canApplyGravity)
        {
            bool alreadyContainThisGraviton = _gravitonsInside.Contains(graviton);
            if (canApplyGravity && !alreadyContainThisGraviton)
            {
                _gravitonsInside.Add(graviton);
            }
            else if (!canApplyGravity && alreadyContainThisGraviton)
            {
                _gravitonsInside.Remove(graviton);
            }
        }

        public void RemoveGravionFromList(Graviton graviton)
        {
            _gravitonsInside.Remove(graviton);
        }

        public void RemoveAllGravitonFromList()
        {
            _gravitonsInside.Clear();
        }

        

        public abstract Vector3 GetClosestPoint(Graviton graviton, out bool canApplyGravity);
        public virtual void CustomUpdateIfCanMove()
        {
            bool canMove = !Application.isPlaying
                || (Application.isPlaying && SettingsGlobal.IsMovable);

            if (canMove && transform.hasChanged)
            {
                if (transform.localScale != _oldScale)
                {
                    MinMaxRangeChanged();
                    _oldScale = transform.localScale;
                }
                else
                {
                    Move();
                }

                transform.hasChanged = false;
            }
        }

        public Vector3 GetRightPosWithRange(Vector3 posEntity, Vector3 posCenter, float range, float maxRange, out bool outOfRange)
        {
            outOfRange = false;

            if (range == 0 && maxRange == 0)
            {
                //Debug.Log("always 'in zone', no range defined");
                return (posCenter);
            }

            Vector3 posFound = posCenter;
            float lenghtCenterToPlayer = 0;

            if (range > 0)
            {
                Vector3 realPos = posCenter + (posEntity - posCenter).normalized * range;
                lenghtCenterToPlayer = (posEntity - posCenter).sqrMagnitude;
                float lenghtCenterToRangeMax = (realPos - posCenter).sqrMagnitude;
                if (lenghtCenterToRangeMax > lenghtCenterToPlayer)
                {
                    realPos = posEntity;
                    //Debug.Log("is inside !");
                    //outOfRange = true;
                }
                else
                {
                    //Debug.Log("is in zone !");
                }

                posFound = realPos;
            }

            //if player is out of range, return null
            if (maxRange > range)
            {
                Vector3 realPos = posCenter + (posEntity - posCenter).normalized * maxRange;
                //calculate only if we havn't already calculate
                if (range == 0)
                    lenghtCenterToPlayer = (posEntity - posCenter).sqrMagnitude;
                float lenghtCenterToRangeMax = (realPos - posCenter).sqrMagnitude;
                if (lenghtCenterToPlayer > lenghtCenterToRangeMax)
                {
                    posFound = Vector3.zero;
                    outOfRange = true;
                    //Debug.Log("is out of zone !");
                }
            }

            return (posFound);
        }

       

        

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!SettingsGlobal.ShowRange)
            {
                return;
            }
            DrawRange(GetColor());
        }

        private Color GetColor()
        {
            if (_gravitonsInside.Count == 0)
            {
                return (Color.red);
            }
            return (Color.green);
        }

        private void OnDrawGizmosSelected()
        {
            if (!SettingsGlobal.ShowRangeOnSelect)
            {
                return;
            }
            DrawRange(GetColor());
        }

        protected abstract void DrawRange(Color color);

#endif
    }
}