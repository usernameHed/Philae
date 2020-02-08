using hedCommon.extension.runtime;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace hedCommon.tools
{
    [ExecuteInEditMode]
    public class AutoSizeCollider2d : MonoBehaviour
    {
        [FoldoutGroup("GamePlay")]
        public bool AutoSizeInPlay = false;

        [FoldoutGroup("Object")]
        public SpriteRenderer SpriteRenderer;
        [FoldoutGroup("Object")]
        public Collider2D Collider2D;





        private void Start()
        {
            if (!AutoSizeInPlay && Application.isPlaying)
            {
                this.enabled = false;
            }
        }

        private void ApplyAutoSize()
        {
            if (SpriteRenderer == null)
            {
                SpriteRenderer = GetComponent<SpriteRenderer>();
            }
            if (Collider2D == null)
            {
                Collider2D = GetComponent<Collider2D>();
            }

            Ext2D.AutoSizeCollider2d(SpriteRenderer, Collider2D);
        }

        // Update is called once per frame
        private void Update()
        {
            ApplyAutoSize();
        }
    }
}