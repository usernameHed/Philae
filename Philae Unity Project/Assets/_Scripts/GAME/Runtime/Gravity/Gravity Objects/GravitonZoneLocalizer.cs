using hedCommon.extension.runtime;
using hedCommon.time;
using philae.data.gravity;
using philae.gravity.zones;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.graviton
{
    [Serializable]
    public class GravitonZoneLocalizer
    {
        public List<GravityAttractorZone> ZonesWhereWeAreInside = new List<GravityAttractorZone>();

        [SerializeField, ReadOnly]
        private bool _isInsideAZone = false;

        private Graviton _gravitonElement;
        public Graviton GravitonElement => _gravitonElement;
        private ZonesLister _zonesLister;
        public ZonesLister ZonesLister() => _zonesLister;
        private FrequencyCoolDown _frequencyToFindZones = new FrequencyCoolDown();


        private GravitonSettings _gravitonSettings;


        public void Init(Graviton graviton, ZonesLister zonesLister, GravitonSettings settings)
        {
            _gravitonElement = graviton;
            _zonesLister = zonesLister;
            _gravitonSettings = settings;
            _frequencyToFindZones = new FrequencyCoolDown();
            _isInsideAZone = false;
        }

        /// <summary>
        /// called every frame, optimize the calculation of zone detections
        /// </summary>
        public void AttemptToCalculateInWhichZoneWeAre()
        {
            if (_gravitonElement == null)
            {
                return;
            }

            if (_gravitonElement.SettingsLocal.FrequencyToFindZones == 0 || _frequencyToFindZones.IsNotRunning())
            {
                _isInsideAZone = _zonesLister.CalculateInWhichZoneGravitonIs(_gravitonElement, this);
                _frequencyToFindZones.StartCoolDown(_gravitonElement.SettingsLocal.FrequencyToFindZones);
            }
        }

        /// <summary>
        /// called when we want to check a change in zone/graviton positionement
        /// </summary>
        /// <param name="isInsideZone"></param>
        /// <param name="zone"></param>
        /// <param name="graviton"></param>
        public static void SetupZoneChange(bool isInsideZone, GravityAttractorZone zone, GravitonZoneLocalizer gravitonZoneLocalizer)
        {
            bool zoneContainGraviton = gravitonZoneLocalizer.ZonesWhereWeAreInside.Contains(zone);

            bool isGravitonAlreadyInsideZone = isInsideZone && zoneContainGraviton;
            bool isGravitonAlreadyOusideZone = !isInsideZone && !zoneContainGraviton;
            bool isGravitonEnterANewZone = isInsideZone && !zoneContainGraviton;
            bool isGravitonLeaveAZone = !isInsideZone && zoneContainGraviton;

            if (isGravitonAlreadyInsideZone || isGravitonAlreadyOusideZone)
            {
                return;
            }
            if (isGravitonEnterANewZone)
            {
                gravitonZoneLocalizer.ZonesWhereWeAreInside.AddIfNotContain(zone);
                zone.AddGraviton(gravitonZoneLocalizer.GravitonElement);
            }
            else if (isGravitonLeaveAZone)
            {
                gravitonZoneLocalizer.ZonesWhereWeAreInside.Remove(zone);
                zone.LeaveZone(gravitonZoneLocalizer.GravitonElement);
            }
        }
    }
}