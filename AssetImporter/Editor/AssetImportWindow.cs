using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetImportWindow : EditorWindow
{
    private Dictionary<string, bool> importToggles = new Dictionary<string, bool>();
    private Vector2 assetStoreScrollPosition = Vector2.zero;
    private bool showQuickSelect = false;

    int selectedTab = 0;
    private string searchQuery = "";

    private bool assetStorageFoldout = false;
    private string assetStoragePath = @"C:\Users\mstue\AppData\Roaming\Unity\Asset Store-5.x\";
    private Dictionary<string, bool> assetSelection = new Dictionary<string, bool>();
    private Vector2 assetStorageScrollPosition = Vector2.zero;

    [MenuItem("Project Setup/Asset Importer")]
    public static void ShowWindow()
    {
        GetWindow<AssetImportWindow>("Asset Importer");
    }

    private void OnEnable()
    {
        InitializeImportToggles();
    }

    private void InitializeImportToggles()
    {
        importToggles.Clear();
        AddAssetsToToggles(UnityAssetLocations.AssetStore);
    }

    private void AddAssetsToToggles(Dictionary<string, string> assets)
    {
        foreach (var asset in assets)
        {
            importToggles[asset.Key] = false;
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(10, 10, 10, 10) });
        try
        {
            DrawHeader();
            DrawActionButtons();
            DrawTabs();
            DrawMainContent();
        }
        finally
        {
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Space(5);
        EditorGUILayout.LabelField("Asset Importer", EditorStyles.boldLabel);
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();
    }

    private void DrawSearchBar()
    {
        EditorGUILayout.BeginHorizontal();
        GUIContent searchIcon = EditorGUIUtility.IconContent("Search Icon");
        EditorGUILayout.LabelField(searchIcon, GUILayout.Width(20));
        searchQuery = EditorGUILayout.TextField(searchQuery, EditorStyles.toolbarSearchField);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawActionButtons()
    {
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = new Color(0.7f, 0.9f, 0.7f); // Light green
        if (
            GUILayout.Button(
                new GUIContent("Import Selected", EditorGUIUtility.IconContent("Import").image),
                GUILayout.Height(30)
            )
        )
        {
            ImportSelectedAssets();
        }
        GUI.backgroundColor = new Color(0.9f, 0.7f, 0.7f); // Light red
        if (
            GUILayout.Button(
                new GUIContent("Clear All", EditorGUIUtility.IconContent("Clear").image),
                GUILayout.Height(30),
                GUILayout.Width(100)
            )
        )
        {
            ClearAllToggles();
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    private void DrawQuickSelectToggle()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        showQuickSelect = EditorGUILayout.ToggleLeft(
            new GUIContent(
                "Show Quick Select",
                EditorGUIUtility.IconContent("FilterByLabel").image
            ),
            showQuickSelect,
            EditorStyles.boldLabel
        );
        EditorGUILayout.EndHorizontal();
    }

    private void DrawTabs()
    {
        string[] tabs = { "Categories", "All Assets", "Selected Assets" };
        selectedTab = GUILayout.Toolbar(selectedTab, tabs, GUILayout.Height(25));
    }

    private void DrawMainContent()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        try
        {
            assetStoreScrollPosition = EditorGUILayout.BeginScrollView(assetStoreScrollPosition);
            try
            {
                if (selectedTab == 0)
                {
                    Categories();
                }
                else if (selectedTab == 1)
                {
                    DrawSearchBar();
                    GUILayout.Space(10);
                    DrawQuickSelectToggle();
                    if (showQuickSelect)
                    {
                        DrawQuickSelectButtons();
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                        GUILayout.Space(5);
                    }
                    GUILayout.Space(10);
                    AllAssets();
                }
                else
                {
                    DrawSelectedAssets();
                }
            }
            finally
            {
                EditorGUILayout.EndScrollView();
            }
        }
        finally
        {
            EditorGUILayout.EndVertical();
        }
    }

    private void ImportSelectedAssets()
    {
        foreach (var toggle in importToggles)
        {
            if (toggle.Value)
            {
                string assetPath = GetAssetPath(toggle.Key);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    ProjectInitialization.ImportAsset(assetPath);
                }
                else
                {
                    Debug.LogError($"Asset key {toggle.Key} not found in any dictionary");
                }
            }
        }
    }

    private string GetAssetPath(string key)
    {
        if (UnityAssetLocations.AssetStore.TryGetValue(key, out var assetPath))
        {
            return assetPath;
        }

        return null;
    }

    private void ClearAllToggles()
    {
        var keys = importToggles.Keys.ToList();
        foreach (var key in keys)
        {
            importToggles[key] = false;
        }
    }

    private void DrawSelectedAssets()
    {
        var selectedItems = importToggles.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

        if (selectedItems.Count == 0)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.HelpBox("No assets selected", MessageType.Info);
            EditorGUILayout.EndVertical();
            return;
        }

        foreach (var item in selectedItems)
        {
            if (importToggles.ContainsKey(item))
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                importToggles[item] = EditorGUILayout.ToggleLeft(item, importToggles[item]);
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void DrawQuickSelectButtons()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // Title
        EditorGUILayout.LabelField("Quick Select Categories", EditorStyles.boldLabel);
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        // Column 1: Categories
        EditorGUILayout.BeginVertical(
            EditorStyles.helpBox,
            GUILayout.Width(position.width / 3 - 10)
        );
        EditorGUILayout.LabelField("Core Categories", EditorStyles.boldLabel);
        GUILayout.Space(5);

        using (new EditorGUILayout.VerticalScope())
        {
            DrawQuickSelectGroup(
                new[]
                {
                    ("🌍 World Creation", UnityAssetLocations.WorldCreation),
                    ("👤 Character Control", UnityAssetLocations.CharacterControl),
                    ("🎭 Animation Tools", UnityAssetLocations.AnimationTools),
                    ("🤖 AI & Behavior", UnityAssetLocations.AIAndBehavior),
                    ("🔧 Editor Enhancements", UnityAssetLocations.EditorEnhancements),
                    ("🌲 Environment Assets", UnityAssetLocations.EnvironmentAssets),
                    ("✨ VFX & Graphics", UnityAssetLocations.VFXAndGraphics),
                    ("⚡ Optimization Tools", UnityAssetLocations.OptimizationTools),
                }
            );
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        // Column 2: Major Publishers
        EditorGUILayout.BeginVertical(
            EditorStyles.helpBox,
            GUILayout.Width(position.width / 3 - 10)
        );
        EditorGUILayout.LabelField("Major Publishers", EditorStyles.boldLabel);
        GUILayout.Space(5);

        using (new EditorGUILayout.VerticalScope())
        {
            DrawQuickSelectGroup(
                new[]
                {
                    ("🌿 Nature Manufacture", UnityAssetLocations.NatureManufactureAssets),
                    ("🌍 Procedural Worlds", UnityAssetLocations.ProceduralWorldsAssets),
                    ("🦁 Malber Animations", UnityAssetLocations.MalberAnimationsAssets),
                    ("⚙️ FImpossible", UnityAssetLocations.FImpossibleCreationsAssets),
                    ("🎨 Layer Lab", UnityAssetLocations.LayerLabAssets),
                    ("🎮 Polyperfect", UnityAssetLocations.PolyperfectAssets),
                }
            );
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        // Column 3: Additional Publishers
        EditorGUILayout.BeginVertical(
            EditorStyles.helpBox,
            GUILayout.Width(position.width / 3 - 10)
        );
        EditorGUILayout.LabelField("Additional Publishers", EditorStyles.boldLabel);
        GUILayout.Space(5);

        using (new EditorGUILayout.VerticalScope())
        {
            DrawQuickSelectGroup(
                new[]
                {
                    ("🎲 Protofactor", UnityAssetLocations.ProtofactorAssets),
                    ("🦊 Quirky Series", UnityAssetLocations.QuirkySeriesAssets),
                    ("🏔️ Distant Lands", UnityAssetLocations.DistantLandsAssets),
                    ("⛰️ More Mountains", UnityAssetLocations.MoreMountainsAssets),
                    ("🎯 MT Assets", UnityAssetLocations.MTAssetsAssets),
                }
            );
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawQuickSelectGroup(IEnumerable<(string name, List<string> assets)> buttons)
    {
        foreach (var (name, assets) in buttons)
        {
            if (GUILayout.Button(name, GUILayout.Height(24)))
            {
                foreach (var toggle in importToggles)
                {
                    foreach (var prefix in assets)
                    {
                        if (toggle.Key.StartsWith(prefix))
                        {
                            importToggles[toggle.Key] = true;
                            break;
                        }
                    }
                }
            }
        }
    }

    private void AllAssets()
    {
        bool assetStoreFoldout = EditorPrefs.GetBool("AssetStoreFoldout", false);
        assetStoreFoldout = EditorGUILayout.Foldout(assetStoreFoldout, "All Assets", true);
        EditorPrefs.SetBool("AssetStoreFoldout", assetStoreFoldout);
        if (assetStoreFoldout)
        {
            foreach (var asset in UnityAssetLocations.AssetStore)
            {
                if (
                    string.IsNullOrEmpty(searchQuery)
                    || asset.Key.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                )
                {
                    importToggles[asset.Key] = EditorGUILayout.ToggleLeft(
                        asset.Key,
                        importToggles[asset.Key]
                    );
                }
            }
        }
    }

    private void Categories()
    {
        bool categoriesFoldout = EditorPrefs.GetBool("CategoriesFoldout", false);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();
        categoriesFoldout = EditorGUILayout.Foldout(categoriesFoldout, "Categories", true);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Expand All", GUILayout.Width(100)))
        {
            // Set all category foldouts to true
            SetAllCategoryFoldouts(true);
        }
        if (GUILayout.Button("Collapse All", GUILayout.Width(100)))
        {
            // Set all category foldouts to false
            SetAllCategoryFoldouts(false);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        EditorPrefs.SetBool("CategoriesFoldout", categoriesFoldout);

        if (categoriesFoldout)
        {
            DrawCategorySection("World Creation", UnityAssetLocations.WorldCreation);
            DrawCategorySection("Character Control", UnityAssetLocations.CharacterControl);
            DrawCategorySection("Animation Tools", UnityAssetLocations.AnimationTools);
            DrawCategorySection("AI & Behavior", UnityAssetLocations.AIAndBehavior);
            DrawCategorySection("Editor Enhancements", UnityAssetLocations.EditorEnhancements);
            DrawCategorySection("Environment Assets", UnityAssetLocations.EnvironmentAssets);
            DrawCategorySection("VFX & Graphics", UnityAssetLocations.VFXAndGraphics);
            DrawCategorySection("Optimization Tools", UnityAssetLocations.OptimizationTools);

            GUILayout.Space(5);
            EditorGUILayout.LabelField("Publishers", EditorStyles.boldLabel);
            GUILayout.Space(5);
            DrawCategorySection("Sabi", UnityAssetLocations.Sabi);
            DrawCategorySection("Nature Manufacture", UnityAssetLocations.NatureManufactureAssets);
            DrawCategorySection("Procedural Worlds", UnityAssetLocations.ProceduralWorldsAssets);
            DrawCategorySection("Malber Animations", UnityAssetLocations.MalberAnimationsAssets);
            DrawCategorySection("FImpossible", UnityAssetLocations.FImpossibleCreationsAssets);
            DrawCategorySection("Layer Lab", UnityAssetLocations.LayerLabAssets);
            DrawCategorySection("Polyperfect", UnityAssetLocations.PolyperfectAssets);
            DrawCategorySection("Protofactor", UnityAssetLocations.ProtofactorAssets);
            DrawCategorySection("Quirky Series", UnityAssetLocations.QuirkySeriesAssets);
            DrawCategorySection("Distant Lands", UnityAssetLocations.DistantLandsAssets);
            DrawCategorySection("More Mountains", UnityAssetLocations.MoreMountainsAssets);
            DrawCategorySection("MT Assets", UnityAssetLocations.MTAssetsAssets);
        }

        EditorGUILayout.EndVertical();
    }

    private void SetAllCategoryFoldouts(bool value)
    {
        // Add all your category names here
        string[] categories =
        {
            "World Creation",
            "Character Control",
            "Animation Tools",
            "AI & Behavior",
            "Editor Enhancements",
            "Environment Assets",
            "VFX & Graphics",
            "Optimization Tools",
            "Nature Manufacture",
            "Procedural Worlds",
            "Malber Animations",
            "FImpossible",
            "Layer Lab",
            "Polyperfect",
            "Protofactor",
            "Quirky Series",
            "Distant Lands",
            "More Mountains",
            "MT Assets",
        };

        foreach (var category in categories)
        {
            EditorPrefs.SetBool($"{category}Foldout", value);
        }
    }

    private void DrawCategorySection(string categoryName, List<string> assetPrefixes)
    {
        bool categoryFoldout = EditorPrefs.GetBool($"{categoryName}Foldout", false);
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        try
        {
            categoryFoldout = EditorGUILayout.Foldout(categoryFoldout, categoryName, true);

            // Add select all/none buttons for each category
            if (GUILayout.Button("All", GUILayout.Width(40)))
            {
                SetCategoryToggles(assetPrefixes, true);
            }
            if (GUILayout.Button("None", GUILayout.Width(40)))
            {
                SetCategoryToggles(assetPrefixes, false);
            }
        }
        finally
        {
            EditorGUILayout.EndHorizontal();
        }

        EditorPrefs.SetBool($"{categoryName}Foldout", categoryFoldout);

        if (categoryFoldout)
        {
            EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(10, 0, 0, 0) });
            try
            {
                // Create a list of keys to modify
                var keysToModify = new List<string>();

                foreach (var toggle in importToggles)
                {
                    foreach (var prefix in assetPrefixes)
                    {
                        if (toggle.Key.StartsWith(prefix))
                        {
                            keysToModify.Add(toggle.Key);
                            break;
                        }
                    }
                }

                // Modify the dictionary outside of the iteration
                foreach (var key in keysToModify)
                {
                    importToggles[key] = EditorGUILayout.ToggleLeft(key, importToggles[key]);
                }
            }
            finally
            {
                EditorGUILayout.EndVertical();
            }
        }
        EditorGUI.indentLevel--;
    }

    private void SetCategoryToggles(List<string> assetPrefixes, bool value)
    {
        // Create a list of keys to modify
        var keysToModify = new List<string>();

        foreach (var toggle in importToggles)
        {
            foreach (var prefix in assetPrefixes)
            {
                if (toggle.Key.StartsWith(prefix))
                {
                    keysToModify.Add(toggle.Key);
                    break;
                }
            }
        }

        // Modify the dictionary outside of the iteration
        foreach (var key in keysToModify)
        {
            importToggles[key] = value;
        }
    }

    private void SetAllToggles(bool value)
    {
        var keys = importToggles.Keys.ToList();
        foreach (var key in keys)
        {
            importToggles[key] = value;
        }
    }
}
