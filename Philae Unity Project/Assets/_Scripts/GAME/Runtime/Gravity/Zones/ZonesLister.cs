using hedCommon.extension.runtime;
using hedCommon.singletons;
using philae.data.gravity;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace philae.gravity.zones
{
    [ExecuteInEditMode]
    public class ZonesLister : SingletonMono<ZonesLister>//, IOnLoadedScene, IOnCompile
    {
        [SerializeField, ReadOnly]
        private List<GravityAttractorZone> _zonesSubstractive = new List<GravityAttractorZone>(50);
        [SerializeField, ReadOnly]
        private List<GravityAttractorZone> _zonesAddidive = new List<GravityAttractorZone>(50);
        [SerializeField, ReadOnly]
        private List<GravityAttractorZone> _zones = new List<GravityAttractorZone>(100);

        [Button]
        public void Init()
        {
            Debug.Log("update zones lister");
            GravitonLister.Instance.ClearGravitonZone();
            FillZones();
            InitZones();
        }

        public void FillZones()
        {
            _zonesSubstractive.Clear();
            _zonesAddidive.Clear();
            _zones.Clear();
            GravityAttractorZone[] zone = ExtFind.GetScripts<GravityAttractorZone>();
            for (int i = 0; i < zone.Length; i++)
            {
                if (zone[i].SettingsGlobal.Jonction == ZoneSettings.JonctionInteraction.SUBSTRACTIVE)
                {
                    _zonesSubstractive.AddIfNotContain(zone[i]);
                }
                else
                {
                    _zonesAddidive.AddIfNotContain(zone[i]);
                }
                
            }
            _zones.Append(_zonesSubstractive);
            _zones.Append(_zonesAddidive);
        }

        public bool Contain(GravityAttractorZone zone)
        {
            return (_zones.Contains(zone));
        }

        public void InitZones()
        {
            for (int i = 0; i < _zones.Count; i++)
            {
                _zones[i].Init(this);
            }
        }

        public void RemoveZone(GravityAttractorZone zoneToRemove)
        {
            if (zoneToRemove.SettingsGlobal.Jonction == ZoneSettings.JonctionInteraction.SUBSTRACTIVE)
            {
                _zonesSubstractive.Remove(zoneToRemove);
            }
            else
            {
                _zonesAddidive.Remove(zoneToRemove);
            }
            _zones.Remove(zoneToRemove);
        }

        /// <summary>
        /// called each optimized timeFrame, determine if we are inside that zone or not
        /// </summary>
        public bool CalculateInWhichZoneGravitonIs(Graviton gravitonElement, GravitonZoneLocalizer gravitonZoneLocalizer)
        {
            if (gravitonElement == null)
            {
                return (false);
            }

            bool isInsideAZone = false;
            for (int i = 0; i < _zonesSubstractive.Count; i++)
            {
                if (_zonesSubstractive[i] == null)
                {
                    continue;
                }

                bool isInsideZone = _zonesSubstractive[i].IsInsideShape(gravitonElement.Position);
                if (isInsideZone)
                {
                    isInsideAZone = true;
                }
                GravitonZoneLocalizer.SetupZoneChange(isInsideZone, _zonesSubstractive[i], gravitonZoneLocalizer);
            }
            //if inside substractive zone, return true, and don't test additive zones
            if (isInsideAZone)
            {
                //if true, try to get out the graviton from additive zones
                for (int i = 0; i < _zonesAddidive.Count; i++)
                {
                    if (_zonesAddidive[i] == null)
                    {
                        continue;
                    }
                    GravitonZoneLocalizer.SetupZoneChange(false, _zonesAddidive[i], gravitonZoneLocalizer);
                }

                return (true);
            }

            for (int i = 0; i < _zonesAddidive.Count; i++)
            {
                if (_zonesAddidive[i] == null)
                {
                    continue;
                }

                bool isInsideZone = _zonesAddidive[i].IsInsideShape(gravitonElement.Position);
                if (isInsideZone)
                {
                    isInsideAZone = true;
                }
                GravitonZoneLocalizer.SetupZoneChange(isInsideZone, _zonesAddidive[i], gravitonZoneLocalizer);
            }

            return (isInsideAZone);
        }

        /// <summary>
        /// called when a graviton has been desactivated, or destroyed
        /// </summary>
        /// <param name="gravitonZoneLocalizer"></param>
        /// <returns></returns>
        public void DesactivateGraviton(GravitonZoneLocalizer gravitonZoneLocalizer)
        {
            for (int i = 0; i < _zones.Count; i++)
            {
                GravitonZoneLocalizer.SetupZoneChange(false, _zones[i], gravitonZoneLocalizer);
            }
        }
    }
}