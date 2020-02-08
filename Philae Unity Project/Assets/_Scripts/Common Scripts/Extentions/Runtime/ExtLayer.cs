using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.runtime
{
    public static class ExtLayer
    {
        /// <summary>
        /// but what if you want the collision mask to be based on the weapon’s layer?
        /// It’d be nice to set some weapons to “Team1” and others to “Team2”,
        /// perhaps, and also to ensure your code doesn’t break if you change
        /// the collision matrix in the project’s Physics Settings
        /// 
        /// USE:
        /// if(Physics.Raycast(startPosition, direction, out hitInfo, distance,
        ///                          weapon.gameObject.GetCollisionMask()) )
        ///{
        ///    // Handle a hit
        ///}
        ///
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static int GetCollisionMask(this GameObject gameObject, int layer = -1)
        {
            if (layer == -1)
                layer = gameObject.layer;

            int mask = 0;
            for (int i = 0; i < 32; i++)
                mask |= (Physics.GetIgnoreLayerCollision(layer, i) ? 0 : 1) << i;

            return mask;
        }

        /// <summary>
        /// is the object's layer in the specified layermask
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static bool IsInLayerMask(this GameObject gameObject, LayerMask mask)
        {
            return ((mask.value & (1 << gameObject.layer)) > 0);
        }

        public static bool IsInLayerMask(this GameObject gameObject, string nameLayer)
        {
            return (gameObject.layer == LayerMask.NameToLayer(nameLayer));
        }

        public static int GetLayerMask(string layerMask)
        {
            return (1 << LayerMask.NameToLayer(layerMask));
        }
    }
}