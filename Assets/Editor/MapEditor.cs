using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : EditorWindow
{
    [MenuItem("Custom/Map")]
    static public void ShowWindow()
    {
        // 윈도우 생성
        MapEditor window = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));
    }
}
