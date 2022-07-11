using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Heartfield.Serialization;
using SaveType = Heartfield.Serialization.SaveType;

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

    sealed class SaveManagerWindow : EditorWindow<SaveManagerWindowAsset>
    {
        static Type saveSettings;
        readonly object[] testSaveArgs = new object[] { "xxTestSave", 1 };

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

            if (GetAsset.includeCompanyName)
                directory = Path.Combine(root, Application.companyName);

            if (GetAsset.includeProductName)
            {
                if (GetAsset.includeCompanyName)
                    directory = Path.Combine(directory, Application.productName);
                else
                    directory = Path.Combine(root, Application.productName);
            }

            const string path = "Saves";

            if (GetAsset.includeCompanyName || GetAsset.includeProductName)
                directory = Path.Combine(directory, path);
            else
                directory = Path.Combine(root, path);

            return directory;
        }

        void CreateSettingsAsset()
        {
            var resolution = GetAsset.screenshotNativeResolution ? new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height) :
                                                                    GetAsset.screenshotResolution;

            var settings = new SettingsSerialization
            {
                directory = GetAsset.finalDirectory,
                takeScreenshot = GetAsset.takeScreenshot,
                screenshotResolution = resolution,
                countTotalPlayedTime = GetAsset.countTotalPlayedTime
            };

            settings.CreateAsset();
            SaveSettings.LoadAsset();
        }

        void CheckPath()
        {
            bool useGameDataPath = GetAsset.specialFolders == SpecialFolders.GameData;

            if (!GetAsset.useCustomDirectory)
            {
                GetAsset.rootDirectory = Application.persistentDataPath;
                GetAsset.finalDirectory = GetAsset.rootDirectory;
            }
            else
            {
                if (useGameDataPath)
                    GetAsset.rootDirectory = Application.dataPath;
                else
                    GetAsset.rootDirectory = Environment.GetFolderPath((Environment.SpecialFolder)GetAsset.specialFolders);

                GetAsset.finalDirectory = GetDirectory(GetAsset.rootDirectory);
            }

            if (TargetPlatformIsLinux)
                GetAsset.validFolder = GetAsset.specialFolders != SpecialFolders.Favorites && GetAsset.specialFolders != SpecialFolders.InternetCache && GetAsset.specialFolders != SpecialFolders.ProgramFiles;
            else
                GetAsset.validFolder = true;

            if (GetAsset.validFolder)
            {
                string path = SaveSettings.GetFilePath((string)testSaveArgs[0]);

                if (useGameDataPath)
                    GetAsset.previewPath = $"{GetDirectory(".../Data")}/{path}";
                else
                    GetAsset.previewPath = $"{GetAsset.finalDirectory}/{testSaveArgs[0]}.sav";
            }
            else
                GetAsset.previewPath = "Invalid path. Please choose another Special Folder";
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (saveSettings == null)
                saveSettings = EditorUtilities.GetClassType("Heartfield.Serialization", "Heartfield.Serialization.SaveSettings");

            CheckPath();
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            CheckPath();
            CreateSettingsAsset();
        }

        bool TargetPlatformIsCurrentPlatform()
        {
            bool onWindows = Application.platform == RuntimePlatform.WindowsEditor && TargetPlatformIsWindows;
            bool onLinux = Application.platform == RuntimePlatform.LinuxEditor && TargetPlatformIsLinux;
            bool onOSX = Application.platform == RuntimePlatform.OSXEditor && EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX;

            return onLinux || onWindows || onOSX;
        }

        void DrawCustomDirectoryGUI()
        {
            EditorGUI.BeginDisabledGroup(TargetPlatformIsMobileOrWeb);
            EditorGUI.BeginChangeCheck();
            GetAsset.useCustomDirectory = EditorGUILayout.BeginToggleGroup("Use Custom Directory", GetAsset.useCustomDirectory);

            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
            GetAsset.specialFolders = (SpecialFolders)EditorGUILayout.EnumPopup("Special Folders", GetAsset.specialFolders);

            bool usingGameDataFolder = GetAsset.specialFolders == SpecialFolders.GameData;

            EditorGUI.BeginDisabledGroup(usingGameDataFolder);
            GetAsset.includeCompanyName = EditorGUILayout.Toggle("Include Company Name", GetAsset.includeCompanyName);
            GetAsset.includeProductName = EditorGUILayout.Toggle("Include Product Name", GetAsset.includeProductName);

            if (EditorGUI.EndChangeCheck())
            {
                if (TargetPlatformIsMobileOrWeb)
                    GetAsset.useCustomDirectory = false;

                if (usingGameDataFolder)
                {
                    GetAsset.includeCompanyName = false;
                    GetAsset.includeProductName = false;
                }

                CheckPath();
                GetAsset.hasChangesNotSaved = true;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndToggleGroup();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Separator();

            if (TargetPlatformIsCurrentPlatform())
            {
                var messageType = GetAsset.validFolder ? MessageType.Info : MessageType.Error;
                EditorGUILayout.HelpBox($"Destination preview: {GetAsset.previewPath}", messageType);
            }
        }

        void DrawMoreSettingsGUI()
        {
            GetAsset.showMoreSettings = EditorGUILayout.BeginFoldoutHeaderGroup(GetAsset.showMoreSettings, "More Settings");

            if (GetAsset.showMoreSettings)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
                GetAsset.takeScreenshot = EditorGUILayout.BeginToggleGroup("Take Screenshot", GetAsset.takeScreenshot);

                if (GetAsset.takeScreenshot)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
                    EditorGUILayout.BeginVertical("Box");

                    EditorGUILayout.Separator();

                    GetAsset.screenshotNativeResolution = EditorGUILayout.Toggle("Native Resolution", GetAsset.screenshotNativeResolution);

                    EditorGUI.BeginDisabledGroup(GetAsset.screenshotNativeResolution);

                    EditorGUILayout.BeginHorizontal();
                    GetAsset.screenshotResolution = EditorGUILayout.Vector2IntField("Resolution", GetAsset.screenshotResolution);
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.Separator();

                    GetAsset.countTotalPlayedTime = EditorGUILayout.Toggle("Count Total Played Time", GetAsset.countTotalPlayedTime);
                    GetAsset.countWhilePaused = EditorGUILayout.Toggle("Count While Paused", GetAsset.countWhilePaused);

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndToggleGroup();
                EditorGUILayout.EndVertical();

                if (EditorGUI.EndChangeCheck())
                {
                    GetAsset.hasChangesNotSaved = true;
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void DrawDebugGUI()
        {
            EditorGUI.BeginDisabledGroup(!GetAsset.validFolder || !TargetPlatformIsCurrentPlatform());

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("Create Test Save Data", EditorStyles.miniButtonRight))
            {
                SaveManager.SaveToFile(SaveType.Manual);
            }

            if (GUILayout.Button("Load Test Save Data", EditorStyles.miniButtonRight))
            {
                SaveManager.LoadFromFile(SaveType.Manual);// (string)testSaveArgs[0]);
            }
            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Open Directory", EditorStyles.miniButtonRight))
            {
                string path = saveSettings.GetFieldValue<string>("directory");

                if (Directory.Exists(path))
                    Process.Start(path);
                else
                    Process.Start(GetAsset.rootDirectory);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();
        }

        void OnGUI()
        {
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Target Platform", EditorWindowUtilities.AddSpacesToSentence(EditorUserBuildSettings.activeBuildTarget), EditorStyles.largeLabel);

            EditorGUILayoutExtension.DrawSeparatorLine();

            DrawCustomDirectoryGUI();

            EditorGUILayout.Separator();

            DrawMoreSettingsGUI();

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Changes"))
            {
                SaveChanges();
            }

            if (GUILayout.Button("Revert Defaults"))
            {
                GetAsset.RevertDefults();
            }
            EditorGUILayout.EndHorizontal();

            if (GetAsset.hasChangesNotSaved)
            {
                EditorGUILayout.HelpBox("There's changes not saved!", MessageType.Warning);
            }

            EditorGUILayoutExtension.DrawSeparatorLine();

            DrawDebugGUI();
        }
    }
}