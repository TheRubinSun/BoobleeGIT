using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridNodes))]
public class MyGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        {
            DrawDefaultInspector();

            GridNodes myGrid = (GridNodes)target;
            GUILayout.Space(10);

            if(GUILayout.Button("Обновить сетку (Scan)"))
            {
                myGrid.RefreshGrid();
                SceneView.RepaintAll();
            }
        }
    }
}
