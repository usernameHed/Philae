using hedCommon.extension.runtime;
using philae.gravity.graviton;
using philae.gravity.zones;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor.logic
{
    [ExecuteInEditMode]
    public class AttractorLister : MonoBehaviour, IEnumerable<Attractor>
    {
        [SerializeField, ReadOnly]
        private List<Attractor> _attractors = new List<Attractor>(50);
        [SerializeField, ReadOnly]
        private GravityAttractorZone _refZone;

        public void Init(GravityAttractorZone gravityAttractorZone)
        {
            _refZone = gravityAttractorZone;
            for (int i = 0; i < _attractors.Count; i++)
            {
                if (_attractors[i] != null)
                {
                    _attractors[i].Init();
                }
            }
        }

        public void AddAttractor(Attractor attractor)
        {
            bool added = _attractors.AddIfNotContain(attractor);
            if (added)
            {
                Debug.Log("add: " + attractor + "(" + added + ")", attractor);
            }
        }

        public void RemoveAttractor(Attractor attractor)
        {
            bool removed = _attractors.Remove(attractor);
            if (removed)
            {
                Debug.Log("remove: " + attractor, attractor);
            }
        }

        public IEnumerator<Attractor> GetEnumerator()
        {
            for (int i = 0; i < _attractors.Count; i++)
            {
                yield return _attractors[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (GetEnumerator());
        }

#if UNITY_EDITOR
        private void Update()
        {
            ClearNullGraviton();
        }

        private void ClearNullGraviton()
        {
            //Debug.Log("test null gravitonn");
            for (int i = _attractors.Count - 1; i >= 0; i--)
            {
                if (_attractors[i] == null)
                {
                    Debug.Log("remove null ref: " + i);
                    _attractors.RemoveAt(i);
                    continue;
                }
                if (!_attractors[i].gameObject.activeInHierarchy)
                {
                    Debug.Log("remove " + _attractors[i].gameObject);
                    _attractors.RemoveAt(i);
                    continue;
                }
            }
        }
#endif
    }
}