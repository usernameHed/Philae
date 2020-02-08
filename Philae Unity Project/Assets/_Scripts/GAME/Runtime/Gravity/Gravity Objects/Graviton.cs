using philae.data.gravity;
using philae.gravity.physicsBody;
using philae.gravity.zones;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace philae.gravity.graviton
{
    [ExecuteInEditMode]
    public class Graviton : MonoBehaviour
    {
        [FoldoutGroup("Settings"), SerializeField, InlineEditor, FormerlySerializedAs("_settingsGlobal")]
        public GravitonSettings SettingsGlobal = default;
        [FoldoutGroup("Settings")]
        public GravitonSettingsLocal SettingsLocal = new GravitonSettingsLocal();

        [FoldoutGroup("GamePlay"), SerializeField]
        private GravitonZoneLocalizer _gravitonZoneLocalizer = new GravitonZoneLocalizer();
        [FoldoutGroup("GamePlay"), SerializeField]
        private GravitonGravityCalculation _gravitonGravityCalculation = new GravitonGravityCalculation();
        [FoldoutGroup("GamePlay"), SerializeField]
        private GravitonPhysicsApplyer _gravitonPhysicsApplyer = new GravitonPhysicsApplyer();

        [FoldoutGroup("Object"), SerializeField]
        private RigidGraviton _rigidGraviton;
        public RigidGraviton RigidGraviton { get { return (_rigidGraviton); } }
        public Vector3 Position
        { 
            get
            {
                if (_rigidGraviton == null)
                {
                    return (Vector3.zero);
                }
                return (_rigidGraviton.Position);
            }
            set { _rigidGraviton.Position = value; }
        }
        public Vector3 LocalScale
        { 
            get
            {
                if (_rigidGraviton == null)
                {
                    return (Vector3.one);
                }
                return (_rigidGraviton.transform.localScale);
            }
        }
        public Vector3 GetAcceleration { get { return (_rigidGraviton.GetVelocity()); } }
        public Vector3 GetAccelerationNormalized { get { return (_rigidGraviton.GetVelocityNormalized()); } }

        [FoldoutGroup("Object"), SerializeField]
        private ZonesLister _zoneLister;
        [SerializeField]
        private InitialPush _initialPush;
        public InitialPush InitialPush { get { return (_initialPush); } }

        [SerializeField]
        private GravitonJetPack _gravitonJetPack;
        public GravitonJetPack GravitonJetPack { get { return (_gravitonJetPack); } }

        private void OnEnable()
        {
            AddToLister();
        }


        public void AddToLister()
        {
            if (ZonesLister.Instance == null || GravitonLister.Instance == null)
            {
                Debug.Log("game unloading...");
                return;
            }

            if (GravitonLister.Instance)
            {
                Debug.Log("add to graviton Lsiter before init " + gameObject.name);
                GravitonLister.Instance.AddGraviton(this);
            }
            _gravitonZoneLocalizer.ZonesWhereWeAreInside.Clear();

            SettingsLocal.MassChanged -= OnMassChanged;
            SettingsLocal.MassChanged += OnMassChanged;
        }

        public void OnMassChanged()
        {
            _rigidGraviton.Mass = SettingsLocal.Mass;
        }

        private void OnDisable()
        {
            if (GravitonLister.Instance)
            {
                GravitonLister.Instance.RemoveGraviton(this);
            }
            SettingsLocal.MassChanged -= OnMassChanged;
            ClearGravitonZone();
        }

        public void ClearGravitonZone()
        {
            if (_zoneLister)
            {
                _zoneLister.DesactivateGraviton(_gravitonZoneLocalizer);
            }
        }

        public void Init(ZonesLister zoneLister)
        {
            if (ZonesLister.Instance == null || GravitonLister.Instance == null)
            {
                Debug.Log("game unloading...");
                return;
            }

            _zoneLister = zoneLister;

            Debug.Log("init graviton !");
            _gravitonJetPack.Init(this, SettingsLocal, _gravitonGravityCalculation);
            _gravitonZoneLocalizer.Init(this, _zoneLister, SettingsGlobal);
            _gravitonGravityCalculation.Init(this, _gravitonZoneLocalizer, SettingsGlobal, SettingsLocal);
            _gravitonPhysicsApplyer.Init(this, _gravitonGravityCalculation, SettingsGlobal);
        }

        public void TeleportGraviton()
        {

        }

        public bool IsInitialized()
        {
            if (_zoneLister == null)
            {
                return (false);
            }
            if (_gravitonZoneLocalizer.ZonesLister() == null)
            {
                return (false);
            }
            return (true);
        }

        /// <summary>
        /// called at each frame (update in editor, and fixedUpdate in play)
        /// </summary>
        public void CustomPhysicLoop()
        {
            _gravitonZoneLocalizer.AttemptToCalculateInWhichZoneWeAre();
            _gravitonGravityCalculation.CalculateGravity();
            _gravitonPhysicsApplyer.ApplyGravity();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            _gravitonGravityCalculation.OnCustomDrawGizmos();
        }
#endif

        public void DeleteGraviton()
        {
            Destroy(gameObject);
        }
    }
}