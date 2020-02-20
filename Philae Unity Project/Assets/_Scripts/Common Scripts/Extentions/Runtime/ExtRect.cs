using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.runtime
{
    public static class ExtRect
    {
        public static Rect Lerp(Rect prev, Rect next, float time)
        {
            Rect rect = new Rect();
            rect.x = Mathf.Lerp(prev.x, next.x, time);
            rect.y = Mathf.Lerp(prev.y, next.y, time);
            rect.width = Mathf.Lerp(prev.width, next.width, time);
            rect.height = Mathf.Lerp(prev.height, next.height, time);
            return (rect);
        }

        public static Rect GetRectFromVector4(Vector4 vector)
        {
            return (new Rect(vector.x, vector.y, vector.z, vector.w));
        }

        public static Vector2 TopLeft(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMin);
        }

        public static Vector2 Middle(this Rect rect)
        {
            return new Vector2(rect.xMin - rect.xMax, rect.yMin - rect.yMax);
        }


        public static Rect ScaleSizeBy(this Rect rect, float scale)
        {
            return rect.ScaleSizeBy(scale, rect.center);
        }
        public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
        {
            Rect result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale;
            result.xMax *= scale;
            result.yMin *= scale;
            result.yMax *= scale;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }
        public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
        {
            return rect.ScaleSizeBy(scale, rect.center);
        }
        public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
        {
            Rect result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale.x;
            result.xMax *= scale.x;
            result.yMin *= scale.y;
            result.yMax *= scale.y;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }

        public static Rect Set(this Rect rect, Vector2 pos, Vector2 size)
        {
            rect.Set(pos.x, pos.y, size.x, size.y);
            return new Rect(rect);
        }

        public static Rect SetBetween(this Rect rect, Vector2 pos, Vector2 pos2)
        {
            rect.Set(pos.x, pos.y, pos2.x - pos.x, pos2.y - pos.y);
            return new Rect(rect);
        }

        /// <summary>
        /// Sets x/y
        /// </summary>
        public static Rect SetPosition(this Rect rect, Vector2 pos)
        {
            rect.x = pos.x;
            rect.y = pos.y;
            return new Rect(rect);
        }

        /// <summary>
        /// Sets x/y
        /// </summary>
        public static Rect SetPosition(this Rect rect, float x, float y)
        {
            rect.x = x;
            rect.y = y;
            return new Rect(rect);
        }

        /// <summary>
        /// gets width/height as Vector2
        /// </summary>
        public static Vector2 GetSize(this Rect rect)
        {
            return new Vector2(rect.width, rect.height);
        }

        public static Rect ShiftBy(this Rect rect, int x, int y)
        {
            rect.x += (float)x;
            rect.y += (float)y;
            return new Rect(rect);
        }

        public static Rect Include(this Rect rect, Rect other)
        {
            Rect r = new Rect();
            r.xMin = Mathf.Min(rect.xMin, other.xMin);
            r.xMax = Mathf.Max(rect.xMax, other.xMax);
            r.yMin = Mathf.Min(rect.yMin, other.yMin);
            r.yMax = Mathf.Max(rect.yMax, other.yMax);
            return r;
        }

        public static void SetRectX(this RectTransform rectTransform, float x)
        {
            rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
        }

        public static float GetRectX(this RectTransform rectTransform)
        {
            return (rectTransform.anchoredPosition.x);
        }

        public static void SetRectY(this RectTransform rectTransform, float y)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
        }
        public static float GetRectY(this RectTransform rectTransform)
        {
            return (rectTransform.anchoredPosition.y);
        }
        public static void SetWidth(this RectTransform rectTransform, float width)
        {
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
        }
        public static float GetWidth(this RectTransform rectTransform)
        {
            return (rectTransform.sizeDelta.x);
        }

        public static void SetOffsetMinX(this RectTransform rectTransform, float minX)
        {
            rectTransform.offsetMin = new Vector2(minX, rectTransform.offsetMin.y);
        }

        public static void SetOffsetMinY(this RectTransform rectTransform, float minY)
        {
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, minY);
        }

        public static void SetOffsetMaxX(this RectTransform rectTransform, float maxX)
        {
            rectTransform.offsetMax = new Vector2(maxX, rectTransform.offsetMax.y);
        }

        public static void SetOffsetMaxY(this RectTransform rectTransform, float maxY)
        {
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, maxY);
        }

        public static void SetHeight(this RectTransform rectTransform, float height)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }
        public static float GetHeight(this RectTransform rectTransform)
        {
            return (rectTransform.sizeDelta.y);
        }

    }
}