using hedCommon.editorGlobal;
using philae.data.gravity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor.logic
{
    [RequireComponent(typeof(AttractorLister))]
    public class AttractorsUpdater : IUpdater
    {
        [SerializeField]
        private AttractorListerLogic _attractorListerLogic = default;
        [SerializeField]
        private AttractorLister _attractorLister;

        private AttractorListerSettings _settings;

        public void Init(AttractorListerSettings settings)
        {
            _settings = settings;
        }

        public override void CustomLoop()
        {
             AttemptToUpdateAttractors();
        }

        private void AttemptToUpdateAttractors()
        {
            foreach (Attractor attractor in _attractorLister)
            {
                attractor.CustomUpdateIfCanMove();
            }
        }
    }
}