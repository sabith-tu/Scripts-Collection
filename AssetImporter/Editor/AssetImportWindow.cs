using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetImportWindow : EditorWindow
{
    #region Private Fields
    [SerializeField]
    private Dictionary<string, bool> m_ImportToggles = new Dictionary<string, bool>();

    [SerializeField]
    private Vector2 m_AssetStoreScrollPosition = Vector2.zero;

    [SerializeField]
    private bool m_ShowQuickSelect = false;

    [SerializeField]
    private int m_SelectedTab = 0;

    [SerializeField]
    private string m_SearchQuery = "";

    [SerializeField]
    private bool m_AssetStorageFoldout = false;

    [SerializeField]
    private string m_AssetStoragePath = @"C:\Users\mstue\AppData\Roaming\Unity\Asset Store-5.x\";

    [SerializeField]
    private Dictionary<string, bool> m_AssetSelection = new Dictionary<string, bool>();

    [SerializeField]
    private Vector2 m_AssetStorageScrollPosition = Vector2.zero;

    [SerializeField]
    private UnityEngine.Object m_TargetValueObject;

    [SerializeField]
    private string m_TimeScale = "1";
    #endregion

    #region Constants
    private const string c_MenuPath = "Project Setup/Unity Tools";
    private const string c_WindowTitle = "Unity Project Tools";
    #endregion

    #region Enums
    private enum MainTab
    {
        ProjectSetup,
        AssetImporter,
        DebugTools,
    }
    #endregion

    #region Private Properties
    private MainTab m_CurrentMainTab = MainTab.AssetImporter;
    #endregion

    [MenuItem(c_MenuPath)]
    public static void ShowWindow()
    {
        GetWindow<AssetImportWindow>(c_WindowTitle);
    }

    private void OnEnable()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        InitializeImportToggles();
    }

    private void InitializeImportToggles()
    {
        m_ImportToggles.Clear();
        AddAssetsToToggles(UnityAssetLocations.AssetStore);
    }

    private void AddAssetsToToggles(Dictionary<string, string> assets)
    {
        foreach (var asset in assets)
        {
            m_ImportToggles[asset.Key] = false;
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(10, 10, 10, 10) });
        try
        {
            DrawMainTabs();

            switch (m_CurrentMainTab)
            {
                case MainTab.ProjectSetup:
                    DrawProjectSetupContent();
                    break;
                case MainTab.AssetImporter:
                    DrawActionButtons();
                    DrawTabs();
                    DrawMainContent();
                    break;
                case MainTab.DebugTools:
                    DrawDebugToolsContent();
                    break;
            }
        }
        finally
        {
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawMainTabs()
    {
        m_CurrentMainTab = (MainTab)
            GUILayout.Toolbar(
                (int)m_CurrentMainTab,
                new string[] { "Project Setup", "Asset Importer", "Debug Tools" },
                GUILayout.Height(25)
            );

        GUILayout.Space(10);
    }

    private void DrawProjectSetupContent()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Project Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        #region Folder Management
        EditorGUILayout.LabelField("Folder Management", EditorStyles.boldLabel);

        // Setup Folder Structure Button
        GUI.backgroundColor = new Color(0.7f, 0.9f, 0.7f);
        if (
            GUILayout.Button(
                new GUIContent(
                    "Setup Folder Structure",
                    EditorGUIUtility.IconContent("Folder Icon").image
                ),
                GUILayout.Height(30)
            )
        )
        {
            SetupFolderStructure();
        }

        // Organize Files Button
        GUI.backgroundColor = new Color(0.7f, 0.7f, 0.9f);
        if (
            GUILayout.Button(
                new GUIContent(
                    "Organize Files from Root",
                    EditorGUIUtility.IconContent("FilterByType").image
                ),
                GUILayout.Height(30)
            )
        )
        {
            OrganizeFiles();
        }
        #endregion

        GUILayout.Space(10);

        #region Project Settings
        EditorGUILayout.LabelField("Project Settings", EditorStyles.boldLabel);

        // Version Control Setup
        GUI.backgroundColor = new Color(0.8f, 0.9f, 0.7f);
        if (
            GUILayout.Button(
                new GUIContent(
                    "Setup Version Control",
                    EditorGUIUtility.IconContent("d_P4_Local").image
                ),
                GUILayout.Height(30)
            )
        )
        {
            SetupVersionControl();
        }

        #endregion

        GUILayout.Space(10);

        #region Quality Tools
        EditorGUILayout.LabelField("Quality Tools", EditorStyles.boldLabel);

        // Find Missing References
        GUI.backgroundColor = new Color(0.9f, 0.7f, 0.7f);
        if (
            GUILayout.Button(
                new GUIContent(
                    "Find Missing References",
                    EditorGUIUtility.IconContent("console.warnicon").image
                ),
                GUILayout.Height(30)
            )
        )
        {
            FindMissingReferences();
        }

        // Clean Empty Folders
        GUI.backgroundColor = new Color(0.7f, 0.9f, 0.9f);
        if (
            GUILayout.Button(
                new GUIContent(
                    "Clean Empty Folders",
                    EditorGUIUtility.IconContent("TreeEditor.Trash").image
                ),
                GUILayout.Height(30)
            )
        )
        {
            CleanEmptyFolders();
        }
        #endregion

        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndVertical();
    }

    private void SetupFolderStructure()
    {
        // Root level folders
        string[] rootFolders = new string[]
        {
            "Assets/_Project", // Main project folder to keep things organized
            "Assets/_Project/Core", // Core systems and managers
            "Assets/_Project/Art", // All art assets
            "Assets/_Project/Scripts", // All scripts
            "Assets/_Project/Prefabs", // Prefab assets
            "Assets/_Project/Scenes", // Scene files
            "Assets/_Project/Resources", // Resources folder for runtime loading
            "Assets/_Project/Settings", // Project settings and configurations
            "Assets/AssetStore", // Asset store imports
            "Assets/Temp", // Temporary files
        };

        // Art sub-folders
        string[] artFolders = new string[]
        {
            "Materials",
            "Textures",
            "Models",
            "Animation",
            "AnimationControllers",
            "Sprites",
            "UI",
            "Fonts",
            "VFX",
            "Shaders",
        };

        // Scripts sub-folders
        string[] scriptFolders = new string[]
        {
            "Editor",
            "Runtime",
            "ScriptableObjects",
            "Systems",
            "UI",
            "Utils",
        };

        // Prefabs sub-folders
        string[] prefabFolders = new string[]
        {
            "Environment",
            "Characters",
            "UI",
            "VFX",
            "Systems",
        };

        try
        {
            // Create root folders
            foreach (string folder in rootFolders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    CreateFolder(folder);
                }
            }

            // Create Art sub-folders
            foreach (string folder in artFolders)
            {
                string path = $"Assets/_Project/Art/{folder}";
                if (!AssetDatabase.IsValidFolder(path))
                {
                    CreateFolder(path);
                }
            }

            // Create Scripts sub-folders
            foreach (string folder in scriptFolders)
            {
                string path = $"Assets/_Project/Scripts/{folder}";
                if (!AssetDatabase.IsValidFolder(path))
                {
                    CreateFolder(path);
                }
            }

            // Create Prefabs sub-folders
            foreach (string folder in prefabFolders)
            {
                string path = $"Assets/_Project/Prefabs/{folder}";
                if (!AssetDatabase.IsValidFolder(path))
                {
                    CreateFolder(path);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("Folder structure created successfully!");
            EditorUtility.DisplayDialog("Success", "Folder structure has been created!", "OK");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error creating folder structure: {e.Message}");
            EditorUtility.DisplayDialog(
                "Error",
                "Failed to create folder structure. Check console for details.",
                "OK"
            );
        }
    }

    private void CreateFolder(string folderPath)
    {
        string[] folderStructure = folderPath.Split('/');
        string currentPath = folderStructure[0];

        for (int i = 1; i < folderStructure.Length; i++)
        {
            string parentPath = currentPath;
            currentPath = Path.Combine(currentPath, folderStructure[i]);

            if (!AssetDatabase.IsValidFolder(currentPath))
            {
                string guid = AssetDatabase.CreateFolder(parentPath, folderStructure[i]);
                if (string.IsNullOrEmpty(guid))
                {
                    throw new Exception($"Failed to create folder: {currentPath}");
                }
            }
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
        m_SearchQuery = EditorGUILayout.TextField(m_SearchQuery, EditorStyles.toolbarSearchField);
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
        m_ShowQuickSelect = EditorGUILayout.ToggleLeft(
            new GUIContent(
                "Show Quick Select",
                EditorGUIUtility.IconContent("FilterByLabel").image
            ),
            m_ShowQuickSelect,
            EditorStyles.boldLabel
        );
        EditorGUILayout.EndHorizontal();
    }

    private void DrawTabs()
    {
        string[] tabs = { "Categories", "All Assets", "Selected Assets" };
        m_SelectedTab = GUILayout.Toolbar(m_SelectedTab, tabs, GUILayout.Height(25));
    }

    private void DrawMainContent()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        try
        {
            m_AssetStoreScrollPosition = EditorGUILayout.BeginScrollView(
                m_AssetStoreScrollPosition
            );
            try
            {
                if (m_SelectedTab == 0)
                {
                    Categories();
                }
                else if (m_SelectedTab == 1)
                {
                    DrawSearchBar();
                    GUILayout.Space(10);
                    DrawQuickSelectToggle();
                    if (m_ShowQuickSelect)
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
        foreach (var toggle in m_ImportToggles)
        {
            if (toggle.Value)
            {
                string assetPath = GetAssetPath(toggle.Key);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    string assetLocation =
                        "C:\\Users\\mstue\\AppData\\Roaming\\Unity\\Asset Store-5.x\\";

                    AssetDatabase.ImportPackage($"{assetLocation}{assetPath}.unitypackage", false);
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
        var keys = m_ImportToggles.Keys.ToList();
        foreach (var key in keys)
        {
            m_ImportToggles[key] = false;
        }
    }

    private void DrawSelectedAssets()
    {
        var selectedItems = m_ImportToggles.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

        if (selectedItems.Count == 0)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.HelpBox("No assets selected", MessageType.Info);
            EditorGUILayout.EndVertical();
            return;
        }

        foreach (var item in selectedItems)
        {
            if (m_ImportToggles.ContainsKey(item))
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                m_ImportToggles[item] = EditorGUILayout.ToggleLeft(item, m_ImportToggles[item]);
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
                    ("���� MT Assets", UnityAssetLocations.MTAssetsAssets),
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
                // Create a temporary list of keys to modify
                var keysToModify = new List<string>();

                // First, collect all keys that need to be modified
                foreach (var toggle in m_ImportToggles)
                {
                    foreach (var prefix in assets)
                    {
                        if (toggle.Key.StartsWith(prefix))
                        {
                            keysToModify.Add(toggle.Key);
                            break;
                        }
                    }
                }

                // Then modify the dictionary using the collected keys
                foreach (var key in keysToModify)
                {
                    m_ImportToggles[key] = true;
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
                    string.IsNullOrEmpty(m_SearchQuery)
                    || asset.Key.Contains(m_SearchQuery, StringComparison.OrdinalIgnoreCase)
                )
                {
                    m_ImportToggles[asset.Key] = EditorGUILayout.ToggleLeft(
                        asset.Key,
                        m_ImportToggles[asset.Key]
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
            foreach (var item in UnityAssetLocations.Catogarys.Keys)
                DrawCategorySection(item, UnityAssetLocations.Catogarys[item]);
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Publishers", EditorStyles.boldLabel);
            GUILayout.Space(5);
            foreach (var item in UnityAssetLocations.Publishers.Keys)
                DrawCategorySection(item, UnityAssetLocations.Publishers[item]);
        }

        EditorGUILayout.EndVertical();
    }

    private void SetAllCategoryFoldouts(bool value)
    {
        // Add all your category names here
        List<string> categories = new List<string>();

        categories.AddRange(UnityAssetLocations.Catogarys.Keys);
        categories.AddRange(UnityAssetLocations.Publishers.Keys);

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

                foreach (var toggle in m_ImportToggles)
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
                    m_ImportToggles[key] = EditorGUILayout.ToggleLeft(key, m_ImportToggles[key]);
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

        foreach (var toggle in m_ImportToggles)
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
            m_ImportToggles[key] = value;
        }
    }

    private void SetAllToggles(bool value)
    {
        var keys = m_ImportToggles.Keys.ToList();
        foreach (var key in keys)
        {
            m_ImportToggles[key] = value;
        }
    }

    private void OrganizeFiles()
    {
        if (
            !EditorUtility.DisplayDialog(
                "Organize Files",
                "This will move files from the root Assets folder into the appropriate folders. This operation cannot be undone. Continue?",
                "Yes",
                "Cancel"
            )
        )
        {
            return;
        }

        // Ensure the folder structure exists
        SetupFolderStructure();

        // Define file type mappings
        var fileTypeMappings = new Dictionary<string, string>
        {
            // Scripts
            { ".cs", "Assets/_Project/Scripts/Runtime" },
            // Art - Models
            { ".fbx", "Assets/_Project/Art/Models" },
            { ".obj", "Assets/_Project/Art/Models" },
            { ".3ds", "Assets/_Project/Art/Models" },
            // Art - Textures
            { ".png", "Assets/_Project/Art/Textures" },
            { ".jpg", "Assets/_Project/Art/Textures" },
            { ".jpeg", "Assets/_Project/Art/Textures" },
            { ".tga", "Assets/_Project/Art/Textures" },
            { ".psd", "Assets/_Project/Art/Textures" },
            // Materials
            { ".mat", "Assets/_Project/Art/Materials" },
            // Animations
            { ".anim", "Assets/_Project/Art/Animation" },
            { ".controller", "Assets/_Project/Art/AnimationControllers" },
            // Prefabs
            { ".prefab", "Assets/_Project/Prefabs" },
            // Shaders
            { ".shader", "Assets/_Project/Art/Shaders" },
            { ".shadergraph", "Assets/_Project/Art/Shaders" },
            // Scenes
            { ".unity", "Assets/_Project/Scenes" },
            // ScriptableObjects
            { ".asset", "Assets/_Project/Scripts/ScriptableObjects" },
            // UI
            { ".ttf", "Assets/_Project/Art/Fonts" },
            { ".otf", "Assets/_Project/Art/Fonts" },
        };

        try
        {
            // Get all files in the root Assets folder
            string[] allFiles = Directory.GetFiles("Assets", "*.*", SearchOption.TopDirectoryOnly);
            int filesProcessed = 0;
            int filesSkipped = 0;

            foreach (string filePath in allFiles)
            {
                string extension = Path.GetExtension(filePath).ToLower();

                // Skip meta files
                if (extension == ".meta")
                    continue;

                // Skip folders
                if (string.IsNullOrEmpty(extension))
                    continue;

                if (fileTypeMappings.TryGetValue(extension, out string targetFolder))
                {
                    string fileName = Path.GetFileName(filePath);
                    string newPath = Path.Combine(targetFolder, fileName);

                    // Create the target folder if it doesn't exist
                    if (!AssetDatabase.IsValidFolder(targetFolder))
                    {
                        CreateFolder(targetFolder);
                    }

                    // Check if file already exists in target location
                    if (File.Exists(newPath))
                    {
                        Debug.LogWarning($"File already exists at destination: {newPath}");
                        filesSkipped++;
                        continue;
                    }

                    // Move the file
                    try
                    {
                        AssetDatabase.MoveAsset(filePath, newPath);
                        filesProcessed++;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to move {filePath}: {e.Message}");
                        filesSkipped++;
                    }
                }
                else
                {
                    Debug.LogWarning($"No mapping found for file type: {extension}");
                    filesSkipped++;
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog(
                "Organization Complete",
                $"Files processed: {filesProcessed}\nFiles skipped: {filesSkipped}",
                "OK"
            );
        }
        catch (Exception e)
        {
            Debug.LogError($"Error organizing files: {e.Message}");
            EditorUtility.DisplayDialog(
                "Error",
                "Failed to organize files. Check console for details.",
                "OK"
            );
        }
    }

    // Add special handling for editor scripts
    private bool IsEditorScript(string filePath)
    {
        if (!filePath.EndsWith(".cs"))
            return false;

        // Read the file content
        string content = File.ReadAllText(filePath);

        // Check if the file contains editor-specific using statements or attributes
        return content.Contains("UnityEditor")
            || content.Contains("[CustomEditor")
            || content.Contains("[CustomPropertyDrawer")
            || content.Contains("EditorWindow");
    }

    private void SetupVersionControl()
    {
        if (
            !EditorUtility.DisplayDialog(
                "Setup Version Control",
                "This will create standard .gitignore and .gitattributes files. Continue?",
                "Yes",
                "Cancel"
            )
        )
            return;

        try
        {
            // Create .gitignore
            string gitignore =
                @"# Unity generated
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Uu]ser[Ss]ettings/

# Never ignore Asset meta data
!/[Aa]ssets/**/*.meta

# Visual Studio cache directory
.vs/

# Gradle cache directory
.gradle/

# Autogenerated VS/MD/Consulo solution and project files
ExportedObj/
.consulo/
*.csproj
*.unityproj
*.sln
*.suo
*.tmp
*.user
*.userprefs
*.pidb
*.booproj
*.svd
*.pdb
*.mdb
*.opendb
*.VC.db

# Unity3D generated meta files
*.pidb.meta
*.pdb.meta
*.mdb.meta

# Unity3D generated file on crash reports
sysinfo.txt

# Builds
*.apk
*.aab
*.unitypackage
*.app

# Crashlytics generated file
crashlytics-build.properties

# Packed Addressables
/[Aa]ssets/[Aa]ddressable[Aa]ssets[Dd]ata/*/*.bin*

# Temporary auto-generated Android Assets
/[Aa]ssets/[Ss]treamingAssets/aa.meta
/[Aa]ssets/[Ss]treamingAssets/aa/*";

            File.WriteAllText(Path.Combine(Application.dataPath, "../.gitignore"), gitignore);

            // Create .gitattributes
            string gitattributes =
                @"# Unity YAML
*.mat merge=unityyamlmerge eol=lf
*.anim merge=unityyamlmerge eol=lf
*.unity merge=unityyamlmerge eol=lf
*.prefab merge=unityyamlmerge eol=lf
*.physicsMaterial2D merge=unityyamlmerge eol=lf
*.physicMaterial merge=unityyamlmerge eol=lf
*.asset merge=unityyamlmerge eol=lf
*.meta merge=unityyamlmerge eol=lf
*.controller merge=unityyamlmerge eol=lf

# git-lfs
*.cubemap filter=lfs diff=lfs merge=lfs -text
*.unitypackage filter=lfs diff=lfs merge=lfs -text
*.fbx filter=lfs diff=lfs merge=lfs -text
*.obj filter=lfs diff=lfs merge=lfs -text
*.png filter=lfs diff=lfs merge=lfs -text
*.jpg filter=lfs diff=lfs merge=lfs -text
*.jpeg filter=lfs diff=lfs merge=lfs -text
*.hdr filter=lfs diff=lfs merge=lfs -text
*.exr filter=lfs diff=lfs merge=lfs -text
*.mp3 filter=lfs diff=lfs merge=lfs -text
*.wav filter=lfs diff=lfs merge=lfs -text
*.ogg filter=lfs diff=lfs merge=lfs -text
*.mp4 filter=lfs diff=lfs merge=lfs -text
*.mov filter=lfs diff=lfs merge=lfs -text
*.psd filter=lfs diff=lfs merge=lfs -text
*.mb filter=lfs diff=lfs merge=lfs -text
*.tga filter=lfs diff=lfs merge=lfs -text
*.tif filter=lfs diff=lfs merge=lfs -text
*.tiff filter=lfs diff=lfs merge=lfs -text
*.zip filter=lfs diff=lfs merge=lfs -text
*.dll filter=lfs diff=lfs merge=lfs -text
*.pdf filter=lfs diff=lfs merge=lfs -text
*.ttf filter=lfs diff=lfs merge=lfs -text
*.otf filter=lfs diff=lfs merge=lfs -text";

            File.WriteAllText(
                Path.Combine(Application.dataPath, "../.gitattributes"),
                gitattributes
            );

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog(
                "Success",
                "Version control files created successfully!",
                "OK"
            );
        }
        catch (Exception e)
        {
            Debug.LogError($"Error setting up version control: {e.Message}");
            EditorUtility.DisplayDialog(
                "Error",
                "Failed to setup version control. Check console for details.",
                "OK"
            );
        }
    }

    private void FindMissingReferences()
    {
        if (
            !EditorUtility.DisplayDialog(
                "Find Missing References",
                "This will scan all prefabs and scenes for missing references. This might take a while. Continue?",
                "Yes",
                "Cancel"
            )
        )
            return;

        List<string> assetsWithMissingRefs = new List<string>();

        try
        {
            // Scan prefabs
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                Component[] components = prefab.GetComponentsInChildren<Component>(true);
                foreach (Component component in components)
                {
                    if (component == null)
                    {
                        assetsWithMissingRefs.Add($"Prefab: {path}");
                        break;
                    }
                }
            }

            // Scan scenes
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
            foreach (string guid in sceneGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                // Note: This would require additional scene loading logic to check references
                // Implementation depends on your specific needs
            }

            if (assetsWithMissingRefs.Count > 0)
            {
                string message =
                    "Found missing references in:\n\n" + string.Join("\n", assetsWithMissingRefs);
                EditorUtility.DisplayDialog("Missing References", message, "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Success", "No missing references found!", "OK");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error finding missing references: {e.Message}");
            EditorUtility.DisplayDialog(
                "Error",
                "Failed to complete reference check. Check console for details.",
                "OK"
            );
        }
    }

    private void CleanEmptyFolders()
    {
        if (
            !EditorUtility.DisplayDialog(
                "Clean Empty Folders",
                "This will remove all empty folders in the Assets directory. Continue?",
                "Yes",
                "Cancel"
            )
        )
            return;

        try
        {
            bool foldersRemoved;
            do
            {
                foldersRemoved = false;
                string[] directories = Directory.GetDirectories(
                    "Assets",
                    "*",
                    SearchOption.AllDirectories
                );

                foreach (string directory in directories)
                {
                    if (IsDirectoryEmpty(directory))
                    {
                        Directory.Delete(directory, false);
                        File.Delete(directory + ".meta");
                        foldersRemoved = true;
                    }
                }

                AssetDatabase.Refresh();
            } while (foldersRemoved); // Repeat until no more empty folders are found

            EditorUtility.DisplayDialog("Success", "Empty folders cleaned successfully!", "OK");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error cleaning empty folders: {e.Message}");
            EditorUtility.DisplayDialog(
                "Error",
                "Failed to clean empty folders. Check console for details.",
                "OK"
            );
        }
    }

    private bool IsDirectoryEmpty(string path)
    {
        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        return files.Length == 1 && files[0].EndsWith(".meta"); // Only .meta file means empty
    }

    private void DrawDebugToolsContent()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        #region Time Controls
        EditorGUILayout.LabelField("Time Controls", EditorStyles.boldLabel);
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        m_TimeScale = EditorGUILayout.TextField("Time Scale", m_TimeScale);
        GUI.backgroundColor = new Color(0.7f, 0.9f, 0.7f);
        if (GUILayout.Button("Apply", GUILayout.Height(20)))
        {
            if (float.TryParse(m_TimeScale, out float timeScaleValue))
                Time.timeScale = timeScaleValue;
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("0.1x", GUILayout.Height(25)))
            Time.timeScale = 0.1f;
        if (GUILayout.Button("0.5x", GUILayout.Height(25)))
            Time.timeScale = 0.5f;
        if (GUILayout.Button("1x", GUILayout.Height(25)))
            Time.timeScale = 1f;
        if (GUILayout.Button("2x", GUILayout.Height(25)))
            Time.timeScale = 2f;
        if (GUILayout.Button("5x", GUILayout.Height(25)))
            Time.timeScale = 5f;
        EditorGUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(10);

        #region Scene Tools
        EditorGUILayout.LabelField("Scene Tools", EditorStyles.boldLabel);
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = new Color(0.9f, 0.7f, 0.7f);
        if (
            GUILayout.Button(
                new GUIContent("Clear Console", "Clear Unity Debug Console"),
                GUILayout.Height(30)
            )
        )
        {
            var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
            var clearMethod = logEntries.GetMethod(
                "Clear",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
            );
            clearMethod.Invoke(null, null);
        }

        GUI.backgroundColor = new Color(0.7f, 0.7f, 0.9f);
        if (
            GUILayout.Button(
                new GUIContent("Reload Scene", "Reload the current scene"),
                GUILayout.Height(30)
            )
        )
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal();
        m_TargetValueObject = EditorGUILayout.ObjectField(
            "Find References",
            m_TargetValueObject,
            typeof(UnityEngine.Object),
            true
        );
        GUI.backgroundColor = new Color(0.7f, 0.9f, 0.9f);
        if (GUILayout.Button("Find", GUILayout.Width(60), GUILayout.Height(20)))
        {
            FindReferencesInScene(m_TargetValueObject);
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        #endregion

        GUILayout.Space(10);

        #region Performance Tools
        EditorGUILayout.LabelField("Performance Tools", EditorStyles.boldLabel);
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = new Color(0.8f, 0.9f, 0.7f);
        if (
            GUILayout.Button(
                new GUIContent("Stats Window", "Show Unity Stats Window"),
                GUILayout.Height(30)
            )
        )
        {
            EditorWindow.GetWindow(System.Type.GetType("UnityEditor.ProfilerWindow,UnityEditor"));
        }

        GUI.backgroundColor = new Color(0.9f, 0.8f, 0.7f);
        if (
            GUILayout.Button(
                new GUIContent("Frame Debugger", "Open Frame Debugger"),
                GUILayout.Height(30)
            )
        )
        {
            EditorWindow.GetWindow(
                System.Type.GetType("UnityEditor.FrameDebuggerWindow,UnityEditor")
            );
        }

        // New Memory Profiler Button
        GUI.backgroundColor = new Color(0.7f, 0.9f, 0.9f);
        if (
            GUILayout.Button(
                new GUIContent("Memory Profiler", "Open Memory Profiler"),
                GUILayout.Height(30)
            )
        )
        {
            EditorWindow.GetWindow(
                System.Type.GetType("UnityEditor.MemoryProfilerWindow,UnityEditor")
            );
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Debugging Tools
        EditorGUILayout.LabelField("Debugging Tools", EditorStyles.boldLabel);
        GUILayout.Space(5);

        // Debug Log Viewer Button
        if (GUILayout.Button("Open Debug Log Viewer", GUILayout.Height(30)))
        {
            // Open custom log viewer
        }

        // Object Pooling Debugger Button
        if (GUILayout.Button("Open Object Pooling Debugger", GUILayout.Height(30)))
        {
            // Open object pooling debugger
        }

        // Performance Profiler Button
        if (GUILayout.Button("Open Performance Profiler", GUILayout.Height(30)))
        {
            EditorWindow.GetWindow(System.Type.GetType("UnityEditor.ProfilerWindow,UnityEditor"));
        }

        // Memory Usage Report Button
        if (GUILayout.Button("Generate Memory Usage Report", GUILayout.Height(30)))
        {
            // Generate memory usage report
        }

        // Scene Hierarchy Viewer Button
        if (GUILayout.Button("Open Scene Hierarchy Viewer", GUILayout.Height(30)))
        {
            // Open scene hierarchy viewer
        }
        #endregion

        EditorGUILayout.EndVertical();
    }

    private void FindReferencesInScene(UnityEngine.Object target)
    {
        if (target == null)
            return;

        List<GameObject> foundObjects = new List<GameObject>();
        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            Component[] components = go.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component == null)
                    continue;

                SerializedObject so = new SerializedObject(component);
                SerializedProperty sp = so.GetIterator();

                while (sp.NextVisible(true))
                {
                    if (
                        sp.propertyType == SerializedPropertyType.ObjectReference
                        && sp.objectReferenceValue == target
                    )
                    {
                        Debug.Log(
                            $"Found reference in {go.name} -> {component.GetType().Name}",
                            go
                        );
                        foundObjects.Add(go);
                    }
                }
            }
        }

        if (foundObjects.Count > 0)
        {
            Selection.objects = foundObjects.ToArray();
        }
        else
        {
            Debug.Log("No references found in scene.");
        }
    }
}
