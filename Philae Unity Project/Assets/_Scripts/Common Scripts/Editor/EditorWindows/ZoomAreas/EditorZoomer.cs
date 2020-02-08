using hedCommon.extension.runtime;
using UnityEngine;
 
public static class EditorZoomArea
{
    private const float kEditorWindowTabHeight = 21.0f;
    private static Matrix4x4 _prevGuiMatrix;

    public enum PivotType
    {
        TOP_LEFT,
        MIDDLE
    }

    public static Rect Begin(float zoomScale, Rect screenCoordsArea, PivotType pivotType)
    {
        // End the group Unity begins automatically for an EditorWindow to clip out the window tab. This allows us to draw outside of the size of the EditorWindow.
        GUI.EndGroup();

        Rect clippedArea = screenCoordsArea.ScaleSizeBy(1.0f / zoomScale, EditorZoomArea.GetPivotOfRect(screenCoordsArea, pivotType));

        //clippedArea.y += kEditorWindowTabHeight;

        GUI.BeginGroup(clippedArea);

        _prevGuiMatrix = GUI.matrix;
        Matrix4x4 translation = Matrix4x4.TRS(EditorZoomArea.GetPivotOfRect(clippedArea, pivotType), Quaternion.identity, Vector3.one);
        Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1.0f));
        GUI.matrix = translation * scale * translation.inverse * GUI.matrix;

        return clippedArea;
    }

    private static Vector2 GetPivotOfRect(this Rect rect, PivotType pivot)
    {
        switch (pivot)
        {
            case PivotType.TOP_LEFT:
                return (rect.TopLeft());
            case PivotType.MIDDLE:
                return (rect.Middle());
        }
        return (rect.TopLeft());
    }

    public static void End()
    {
        GUI.matrix = _prevGuiMatrix;
        GUI.EndGroup();
        GUI.BeginGroup(new Rect(0.0f, kEditorWindowTabHeight, Screen.width, Screen.height));
    }
}