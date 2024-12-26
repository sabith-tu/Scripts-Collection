using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class Watcher : EditorWindow
{
    private Vector2 scrollPosition;

    [MenuItem("Tools/Watcher")]
    public static void ShowEditorWindow()
    {
        GetWindow<Watcher>();
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Watcher is for runtime only", MessageType.Warning);
            return;
        }
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);
        if (!WatcherWraper.IsAnyWatcherAvailable()) Section_NoWatcher();
        Section_BooleanWatcher(WatcherWraper.watchList_boolean);
        Section_StringWatcher(WatcherWraper.watchList_string);
        Section_ObjectWatcher(WatcherWraper.watchList_object);
        EditorGUILayout.EndScrollView();
        if (WatcherWraper.IsAnyWatcherAvailable())
        {
            EditorGUILayout.Space(10);
            if (GUILayout.Button("Clear All", GUILayout.Height(35))) WatcherWraper.ClearAll();
        }

        Repaint();
    }

    private void Section_NoWatcher()
    {
        EditorGUILayout.HelpBox("Nothing is watched. \nUse WatchString / WatchBoolean / WatchObject from WatcherWraper",
            MessageType.Info);
    }

    private void Section_StringWatcher(Dictionary<string, string> watchList_string)
    {
        if (watchList_string == null || watchList_string.Keys.Count == 0) return;

        // EditorGUILayout.LabelField("--------------------[ Strings ]--------------------", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.Space(5);
        watchList_string.Keys.ToList().ForEach(key =>
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("    " + key, GUILayout.Width((Screen.width * 0.5f).Min(150).Max(300)));
            EditorGUILayout.LabelField(watchList_string[key]);
            EditorGUILayout.EndHorizontal();
            if (watchList_string.Keys.Last() != key)
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.Width(Screen.width));
        });
        EditorGUILayout.Space(5);
        EditorGUILayout.EndVertical();
    }

    private void Section_ObjectWatcher(Dictionary<string, Object> watchList_object)
    {
        if (watchList_object == null || watchList_object.Keys.Count == 0) return;

        // EditorGUILayout.LabelField("--------------------[ Objects ]--------------------", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.Space(5);
        watchList_object.Keys.ToList().ForEach(key =>
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("    " + key, GUILayout.Width((Screen.width * 0.5f).Min(150).Max(300)));
            EditorGUILayout.ObjectField(watchList_object[key], typeof(Object), GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();
            if (watchList_object.Keys.Last() != key)
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.Width(Screen.width));
        });
        EditorGUILayout.Space(5);
        EditorGUILayout.EndVertical();
    }

    private void Section_BooleanWatcher(Dictionary<string, bool> watchList_boolean)
    {
        if (watchList_boolean == null || watchList_boolean.Keys.Count == 0) return;

        EditorGUILayout.Space(10);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.Space(5);
        watchList_boolean.Keys.ToList().ForEach(key =>
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("    " + key, GUILayout.Width((Screen.width * 0.5f).Min(150).Max(300)));

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle(watchList_boolean[key], GUILayout.Width(20));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
            if (watchList_boolean.Keys.Last() != key)
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.Width(Screen.width));
        });
        EditorGUILayout.Space(5);
        EditorGUILayout.EndVertical();
    }
}

