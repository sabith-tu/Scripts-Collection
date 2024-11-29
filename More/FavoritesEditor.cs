// Assets/_Scripts/Helper/Sabi specific/Editor/FavoritesEditor.cs
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FavoritesEditor : EditorWindow
{
    private List<UnityEngine.Object> favoriteAssets = new List<UnityEngine.Object>();
    private Vector2 scrollPosition;
    private List<UnityEngine.Object> savedAssets = new List<UnityEngine.Object>();
    private bool showAssetReferences = true;

    [MenuItem("SABI/Favorites Editor")]
    private static void ShowWindow()
    {
        var window = GetWindow<FavoritesEditor>();
        window.titleContent = new GUIContent("Favorites");
        window.Show();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        GUILayout.Label("Favorite Assets", EditorStyles.boldLabel);

        // Drag and drop area
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        showAssetReferences = EditorGUILayout.Foldout(showAssetReferences, " Favorites", true);
        if (showAssetReferences)
        {
            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.Label(
                "Drag and Drop Assets Here to Save as Favorite",
                EditorStyles.centeredGreyMiniLabel
            );
            var dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "");
            GUI.backgroundColor = Color.white;

            // Handle drag and drop
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (var draggedObject in DragAndDrop.objectReferences)
                        {
                            if (!savedAssets.Contains(draggedObject))
                            {
                                savedAssets.Add(draggedObject);
                            }
                        }
                        SaveFavorites(); // Save the favorites after adding
                        GUI.changed = true;
                        Event.current.Use();
                    }
                    break;
            }

            // Display saved assets
            if (savedAssets.Count != 0)
            {
                GUILayout.Space(10);
                GUILayout.Label("Saved Assets:", EditorStyles.boldLabel);
            }

            for (int i = savedAssets.Count - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal();
                savedAssets[i] = EditorGUILayout.ObjectField(
                    savedAssets[i],
                    typeof(UnityEngine.Object),
                    false
                );
                if (GUILayout.Button("×", GUILayout.Width(20)))
                {
                    savedAssets.RemoveAt(i);
                    SaveFavorites(); // Save after removing
                }
                EditorGUILayout.EndHorizontal();
            }

            // Clear all button
            if (savedAssets.Count > 0)
            {
                GUILayout.Space(5);
                if (GUILayout.Button("Clear All"))
                {
                    savedAssets.Clear();
                    SaveFavorites(); // Save after clearing
                }
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
    }

    private void OnEnable()
    {
        LoadFavorites();
    }

    private void OnDisable()
    {
        SaveFavorites();
    }

    private void SaveFavorites()
    {
        // Save favorite asset GUIDs
        string[] guids = savedAssets // Change from favoriteAssets to savedAssets
            .Where(asset => asset != null)
            .Select(asset => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset)))
            .ToArray();

        EditorPrefs.SetString("Favorites_Assets", string.Join(",", guids));
    }

    private void LoadFavorites()
    {
        savedAssets.Clear(); // Change from favoriteAssets to savedAssets
        string savedGUIDs = EditorPrefs.GetString("Favorites_Assets", "");

        if (!string.IsNullOrEmpty(savedGUIDs))
        {
            string[] guids = savedGUIDs.Split(',');
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                    if (asset != null)
                    {
                        savedAssets.Add(asset); // Change from favoriteAssets to savedAssets
                    }
                }
            }
        }
    }
}
