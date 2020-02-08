using hedCommon.editorGlobal;
using hedCommon.extension.runtime;
using philae.data.gravity;
using philae.gravity.attractor;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using philae.sound;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace philae.gravity.zones
{
    [ExecuteInEditMode, 
        RequireComponent(typeof(AttractorListerLogic),
        typeof(AttractorLister),
        typeof(AttractorsUpdater))]
    public class GravityAttractorZone : SerializedMonoBehaviour
    {
        [FoldoutGroup("Settings"), InlineEditor]
        public ZoneSettings SettingsGlobal = default;
        [FoldoutGroup("Settings"), SerializeField]
        public ZoneSettingsLocal SettingsLocal = new ZoneSettingsLocal();

        [SerializeField]
        private Transform _scalerZoneReference;
        public Transform GetScalerZoneReference { get { return (_scalerZoneReference); } }
        public Transform SetScalerZoneReference { set { _scalerZoneReference = value; } }

        [SerializeField]
        private AttractorListerLogic _attractorLogic;
        public AttractorListerLogic AttractorLogic { get { return (_attractorLogic); } }

        public Zone CurrentZone = null;
        [SerializeField, ReadOnly]
        private ZonesLister _refZoneLister;



        public delegate void OnGravitonEnter(Graviton graviton);
        [ReadOnly]
        public OnGravitonEnter OnGravitonEnterZone;
        [ReadOnly]
        public OnGravitonEnter OnGravitonLeaveZone;

        public void ChangeCombineInteractionAsset(ZoneSettings settings)
        {
            SettingsGlobal = settings;
            _refZoneLister.FillZones();
        }

        private List<Graviton> _gravitonsInside = new List<Graviton>();
        public bool IsZoneEmpty() => _gravitonsInside.Count == 0;

        [SerializeField, ReadOnly]
        protected Transform _parent;

        public ZoneSettingsLocal.Shape ShapeZone
        {
            get
            {
                return (SettingsLocal.ShapeZone);
            }
            set
            {
                if (value == SettingsLocal.ShapeZone)
                {
                    return;
                }
                SettingsLocal.ShapeZone = value;
                CreateShape();
            }
        }

        [Button]
        public void Init(ZonesLister zonesLister)
        {
            if (_scalerZoneReference == null)
            {
                return;
            }

            _refZoneLister = zonesLister;
            if (CurrentZone == null)
            {
                Debug.Log("create shape ?");
                CreateShape();
            }
            _gravitonsInside.Clear();
            _scalerZoneReference.hasChanged = true;

            if (_attractorLogic == null)
            {
                _attractorLogic = transform.GetComponentInChildren<AttractorListerLogic>();
            }
            if (_attractorLogic != null)
            {
                _attractorLogic.Init(this);
            }

            SettingsLocal.IsActiveZoneChange -= IsActiveZoneChanged;
            SettingsLocal.IsActiveZoneChange += IsActiveZoneChanged;
            OnGravitonEnterZone -= OnEnterInZone;
            OnGravitonEnterZone += OnEnterInZone;
            OnGravitonLeaveZone -= OnLeaveZone;
            OnGravitonLeaveZone += OnLeaveZone;
        }




        private void CreateShape()
        {
            switch (SettingsLocal.ShapeZone)
            {
                case ZoneSettingsLocal.Shape.SPHERE:
                    CurrentZone = new ZoneSphere();
                    CurrentZone.Init(this);
                    break;
                case ZoneSettingsLocal.Shape.CUBE:
                    CurrentZone = new ZoneCube();
                    CurrentZone.Init(this);
                    break;
                case ZoneSettingsLocal.Shape.CYLINDER:
                    CurrentZone = new ZoneCylinder();
                    CurrentZone.Init(this);
                    break;
                case ZoneSettingsLocal.Shape.CAPSULE:
                    CurrentZone = new ZoneCapsule();
                    CurrentZone.Init(this);
                    break;

            }
            
        }

        public void AddGraviton(Graviton graviton)
        {
            bool added = _gravitonsInside.AddIfNotContain(graviton);
            if (added)
            {
                if (OnGravitonEnterZone != null)
                {
                    OnGravitonEnterZone.Invoke(graviton);
                }
            }
        }

        public void LeaveZone(Graviton graviton)
        {
            foreach (Attractor attractor in _attractorLogic.Lister)
            {
                attractor.RemoveGravionFromList(graviton);
            }
            _gravitonsInside.Remove(graviton);
            if (OnGravitonLeaveZone != null)
            {
                OnGravitonLeaveZone.Invoke(graviton);
            }
        }

        public void OnEnterInZone(Graviton graviton)
        {
            Debug.Log(graviton.name + " enter in zone " + gameObject.name);
        }

        public void OnLeaveZone(Graviton graviton)
        {
            Debug.Log(graviton.name + " leave in zone " + gameObject.name);
        }

        private void IsActiveZoneChanged()
        {
            DesactiveZone();
        }

        public void DesactiveZone()
        {
            foreach (Attractor attractor in _attractorLogic.Lister)
            {
                attractor.RemoveAllGravitonFromList();
            }
        }

        private void Update()
        {
            if (_scalerZoneReference.hasChanged)
            {
                CurrentZone.Move(_scalerZoneReference.position, _scalerZoneReference.rotation, _scalerZoneReference.localScale);
                _scalerZoneReference.hasChanged = false;
            }
#if UNITY_EDITOR
            ClearNullGraviton();
#endif
        }

#if UNITY_EDITOR
        private void ClearNullGraviton()
        {
            //Debug.Log("test null gravitonn");
            for (int i = _gravitonsInside.Count - 1; i >= 0; i--)
            {
                if (_gravitonsInside[i] == null)
                {
                    Debug.Log("remove null ref: " + i);
                    _gravitonsInside.RemoveAt(i);
                    continue;
                }
                if (!_gravitonsInside[i].gameObject.activeInHierarchy)
                {
                    Debug.Log("remove " + _gravitonsInside[i].gameObject);
                    _gravitonsInside.RemoveAt(i);
                    continue;
                }
            }
        }
#endif

        public bool IsInsideShape(Vector3 point)
        {
            return (CurrentZone.IsInsideShape(point));
        }


        private void OnDrawGizmos()
        {
            if (!EditorOptions.ShowZones)
            {
                return;
            }
            CurrentZone.Draw();
        }

        private void OnDestroy()
        {
            SettingsLocal.IsActiveZoneChange -= IsActiveZoneChanged;
            OnGravitonEnterZone -= OnEnterInZone;
            OnGravitonLeaveZone -= OnLeaveZone;
            if (ZonesLister.Instance == null)
            {
                return;
            }
            ZonesLister.Instance.RemoveZone(this);
        }
    }
}