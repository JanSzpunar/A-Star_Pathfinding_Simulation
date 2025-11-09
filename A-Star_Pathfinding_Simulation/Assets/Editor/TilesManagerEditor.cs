using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TilesManager))]
public class TilesManagerEditor : Editor
{
    private SerializedProperty WidthProp;
    private SerializedProperty HeightProp;
    private SerializedProperty GridContentsProp;
    private SerializedProperty AllTylesVariantsProp;


    void OnEnable()
    {
        WidthProp = serializedObject.FindProperty("Width");
        HeightProp = serializedObject.FindProperty("Height");
        GridContentsProp = serializedObject.FindProperty("GridContents");
        AllTylesVariantsProp = serializedObject.FindProperty("TylesVariantsInRespawnOrder");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(WidthProp);
        EditorGUILayout.PropertyField(HeightProp);
        EditorGUILayout.PropertyField(AllTylesVariantsProp);

        WidthProp.intValue = Mathf.Max(1, WidthProp.intValue);
        HeightProp.intValue = Mathf.Max(1, HeightProp.intValue);

        int width = WidthProp.intValue;
        int height = HeightProp.intValue;
        int totalSize = width * height;

        while (GridContentsProp.arraySize < totalSize)
            GridContentsProp.InsertArrayElementAtIndex(GridContentsProp.arraySize);
        while (GridContentsProp.arraySize > totalSize)
            GridContentsProp.DeleteArrayElementAtIndex(GridContentsProp.arraySize - 1);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tiles Matrix", EditorStyles.boldLabel);

        for (int y = 0; y < height; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                SerializedProperty entry = GridContentsProp.GetArrayElementAtIndex(index);
                SerializedProperty posProp = entry.FindPropertyRelative("Position");
                SerializedProperty dataProp = entry.FindPropertyRelative("Data");

                posProp.vector2IntValue = new Vector2Int(x, y);
                dataProp.objectReferenceValue = EditorGUILayout.ObjectField(dataProp.objectReferenceValue, typeof(TileData), false, GUILayout.Width(100));
            }
            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
