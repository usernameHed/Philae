using hedCommon.extension.runtime;
using hedCommon.time;
using philae.gravity.graviton;
using philae.gravity.physicsBody;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.graviton
{
    public class GravitonSlowDownOverTime : MonoBehaviour
    {
        [SerializeField]
        private Graviton _graviton;
        [SerializeField]
        private float _maxMass = 1000f;
        [SerializeField]
        private float _slowDown = 0.95f;

        [SerializeField]
        private AnimationCurve _slowDownOverTime = new AnimationCurve();

        private FrequencyChrono _frequencyChrono = new FrequencyChrono();

        private float _initialMass = 1f;

        private void Start()
        {
            _graviton.RigidGraviton.Mass = _initialMass;
            _frequencyChrono.StartChrono();
        }

        private void FixedUpdate()
        {
            float massToAdd = _slowDownOverTime.Evaluate(_frequencyChrono.GetTimer());
            massToAdd = ExtMathf.Remap(massToAdd, 0, 1, _initialMass, _maxMass);
            float mass = _initialMass + massToAdd;
            mass = Mathf.Clamp(mass, _initialMass, _maxMass);

            _graviton.RigidGraviton.SlowDown(_slowDown);

            _graviton.RigidGraviton.Mass = mass;
            if (mass >= _maxMass)
            {
                DeleteGraviton();
            }
        }

        public void DeleteGraviton()
        {
            Debug.Log("delete graviton !");
            _graviton.DeleteGraviton();
        }
    }
}