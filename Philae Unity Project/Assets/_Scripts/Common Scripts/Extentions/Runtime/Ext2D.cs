using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace hedCommon.extension.runtime
{
    public static class Ext2D
    {
        public static void AutoSizeCollider2d(SpriteRenderer spriteRenderer, Collider2D collider)
        {
            System.Type typeCollider = collider.GetType();

            if (typeCollider == typeof(BoxCollider2D))
            {
                BoxCollider2D box = (BoxCollider2D)collider;

                if (spriteRenderer.drawMode == SpriteDrawMode.Simple)
                {
                    Vector2 sizeBounds = spriteRenderer.sprite.bounds.size;
                    box.size = sizeBounds;
                    box.offset = Vector2.zero;
                }
                else
                {
                    box.offset = Vector2.zero;
                    box.size = spriteRenderer.size;
                }
            }
            else if (typeCollider == typeof(CapsuleCollider2D))
            {
                CapsuleCollider2D capsule = (CapsuleCollider2D)collider;
                Vector2 sizeBounds = spriteRenderer.sprite.bounds.size;

                capsule.size = sizeBounds;

                Vector2 localScale = spriteRenderer.transform.localScale;
                if (Mathf.Abs(localScale.x) > Mathf.Abs(localScale.y))
                {
                    capsule.direction = CapsuleDirection2D.Horizontal;
                }
                else
                {
                    capsule.direction = CapsuleDirection2D.Vertical;
                }
            }
        }

    }
}