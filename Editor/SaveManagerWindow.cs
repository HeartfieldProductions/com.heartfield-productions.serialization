using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Heartfield.Serialization;

namespace HeartfieldEditor.Serialization
{
    enum SpecialFolders
    {
        GameData = -1,
        ApplicationData = 26,
        CommonApplicationData = 35,
        CommonTemplates = 45,
        Desktop = 0,
        Favorites = 6,             //Not suported on Linux
        Fonts = 20,
        InternetCache = 32,        //Not suported on Linux
        LocalApplicationData = 28,
        MyDocuments = 5,
        MyMusic = 13,
        MyPictures = 39,
        MyVideos = 14,
        ProgramFiles = 38,         //Not Suported on Linux
        Templates = 21,
        UserProfile = 40
    }

    sealed class SaveManagerWindow : HeartfieldEditorWindow<SaveManagerWindowAsset>
    {
        static SaveManagerWindowAsset asset;

        protected override SaveManagerWindowAsset AssetToSave
        {
            get
            {
                if (asset == null)
                    asset = new SaveManagerWindowAsset();

                return asset;
            }
        }

        protected override string AssetKey => "SaveManagerWindow";

        bool TargetPlatformIsMobileOrWeb => EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS ||
                                            EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
                                            EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL;

        bool TargetPlatformIsLinux => EditorUserBuildSettings.activeBuildTarget == BuildTarget.EmbeddedLinux ||
                                      EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinux64;

        bool TargetPlatformIsWindows => EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows ||
                                        EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64;

        [MenuItem("Heartfield Productions/Serialization/Save Manager")]
        static void Create()
        {
            var window = GetWindow<SaveManagerWindow>(false, "Save Manager");
            window.Show();
        }

        string GetDirectory(string root)
        {
            string directory = string.Empty;

            if (asset.includeCompanyName)
                directory = Path.Combine(root, Application.companyName);

            if (asset.includeProductName)
            {
                if (asset.includeCompanyName)
                    directory = Path.Combine(directory, Application.productName);
                else
                    directory = Path.Combine(root, Application.productName);
            }

            if (asset.includeCompanyName || asset.includeProductName)
                directory = Path.Combine(directory, asset.path);
            else
                directory = Path.Combine(root, asset.path);

            return directory;
        }

        void CheckSavePath()
        {
            bool useGameDataPath = asset.specialFolders == SpecialFolders.GameData;

            if (!asset.useCustomDirectory)
            {
                asset.rootDirectory = Application.persistentDataPath;
                asset.finalPath = asset.rootDirectory;
            }
            else
            {
                if (useGameDataPath)
                    asset.rootDirectory = Application.dataPath;
                else
                    asset.rootDirectory = Environment.GetFolderPath((Environment.SpecialFolder)asset.specialFolders);

                asset.finalPath = GetDirectory(asset.rootDirectory);
            }

            SaveSettings.fileExtension = asset.extension;

            if (TargetPlatformIsLinux)
                asset.validFolder = asset.specialFolders != SpecialFolders.Favorites && asset.specialFolders != SpecialFolders.InternetCache && asset.specialFolders != SpecialFolders.ProgramFiles;
            else
                asset.validFolder = true;

            if (asset.validFolder)
            {
                SaveSettings.path = asset.finalPath;

                if (useGameDataPath)
                    asset.previewPath = $"{GetDirectory("path to executablename_Data folder")}/{SaveSettings.GetFilePath("TestSave", 1)}";
                else
                    asset.previewPath = SaveSettings.GetFullFilePath("TestSave", 1);
            }
            else
                asset.previewPath = "Invalid path. Please choose another Special Folder";
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CheckSavePath();
        }

        bool TargetPlatformIsCurrentPlatform()
        {
            bool onWindows = Application.platform == RuntimePlatform.WindowsEditor && TargetPlatformIsWindows;
            bool onLinux = Application.platform == RuntimePlatform.LinuxEditor && TargetPlatformIsLinux;
            bool onOSX = Application.platform == RuntimePlatform.OSXEditor && EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX;

            return onLinux || onWindows || onOSX;
        }

        void OnGUI()
        {
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Target Platform", EditorWindowUtilities.AddSpacesToSentence(EditorUserBuildSettings.activeBuildTarget), EditorStyles.largeLabel);

            EditorGUILayoutExtension.DrawSeparatorLine();

            EditorGUI.BeginDisabledGroup(TargetPlatformIsMobileOrWeb);
            EditorGUI.BeginChangeCheck();
            asset.useCustomDirectory = EditorGUILayout.BeginToggleGroup("Use Custom Directory", asset.useCustomDirectory);

            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
            asset.specialFolders = (SpecialFolders)EditorGUILayout.EnumPopup("Special Folders", asset.specialFolders);

            bool usingGameDataFolder = asset.specialFolders == SpecialFolders.GameData;

            EditorGUI.BeginDisabledGroup(usingGameDataFolder);
            asset.includeCompanyName = EditorGUILayout.Toggle("Include Company Name", asset.includeCompanyName);
            asset.includeProductName = EditorGUILayout.Toggle("Include Product Name", asset.includeProductName);
            EditorGUI.EndDisabledGroup();

            if (usingGameDataFolder)
            {
                asset.includeCompanyName = false;
                asset.includeProductName = false;
            }

            asset.path = EditorGUILayout.TextField("Path", asset.path);
            asset.extension = EditorGUILayout.TextField("Extension", asset.extension);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndToggleGroup();
            if (EditorGUI.EndChangeCheck())
            {
                if (TargetPlatformIsMobileOrWeb)
                    asset.useCustomDirectory = false;

                CheckSavePath();
                SaveChanges();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Separator();

            if (TargetPlatformIsCurrentPlatform())
                EditorGUILayout.HelpBox($"Destination preview: {asset.previewPath}", asset.validFolder ? MessageType.Info : MessageType.Error);

            EditorGUILayout.Separator();

            EditorGUI.BeginDisabledGroup(!asset.validFolder || !TargetPlatformIsCurrentPlatform());

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            if (GUILayout.Button("Create Test Save Data", EditorStyles.miniButtonRight))
            {
                SaveManager.Save("TestSave", 1, out _);
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Open Directory", EditorStyles.miniButtonRight))
            {
                if (Directory.Exists(SaveSettings.path))
                    Process.Start(SaveSettings.path);
                else
                    Process.Start(asset.rootDirectory);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }
    }
}