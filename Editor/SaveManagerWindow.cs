using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Diagnostics;
using Heartfield.Serialization;

namespace HeartfieldEditor.Serialization
{
    sealed class SaveManagerWindow : EditorWindow
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

        bool useCustomDirectory;
        SpecialFolders specialFolders;
        bool includeCompanyName;
        bool includeProductName;
        string path = "Saves";
        string extension = "sav";
        static string rootDirectory;
        static string finalPath;
        static string previewPath;
        bool validFolder;

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

            if (includeCompanyName)
                directory = Path.Combine(root, Application.companyName);

            if (includeProductName)
            {
                if (includeCompanyName)
                    directory = Path.Combine(directory, Application.productName);
                else
                    directory = Path.Combine(root, Application.productName);
            }

            if (includeCompanyName || includeProductName)
                directory = Path.Combine(directory, path);
            else
                directory = Path.Combine(root, path);

            return directory;
        }

        void CheckSavePath()
        {
            bool useGameDataPath = specialFolders == SpecialFolders.GameData;

            if (!useCustomDirectory)
            {
                rootDirectory = Application.persistentDataPath;
                finalPath = rootDirectory;
            }
            else
            {
                if (useGameDataPath)
                    rootDirectory = Application.dataPath;
                else
                    rootDirectory = Environment.GetFolderPath((Environment.SpecialFolder)specialFolders);

                finalPath = GetDirectory(rootDirectory);
            }

            SaveSettings.fileExtension = extension;

            if (TargetPlatformIsLinux)
                validFolder = specialFolders != SpecialFolders.Favorites && specialFolders != SpecialFolders.InternetCache && specialFolders != SpecialFolders.ProgramFiles;
            else
                validFolder = true;

            if (validFolder)
            {
                SaveSettings.path = finalPath;

                if (useGameDataPath)
                    previewPath = $"{GetDirectory("path to executablename_Data folder")}/{SaveSettings.GetFilePath("TestSave", 1)}";
                else
                    previewPath = SaveSettings.GetFullFilePath("TestSave", 1);
            }
            else
                previewPath = "Invalid path. Please choose another Special Folder";
        }

        void OnEnable()
        {
            CheckSavePath();
        }

        bool TargetPlatformIsCurrentPlatform()
        {
            bool onWindows = Application.platform == RuntimePlatform.WindowsEditor && TargetPlatformIsWindows;
            bool onLinux = Application.platform == RuntimePlatform.LinuxEditor && TargetPlatformIsLinux;
            bool onOSX = Application.platform == RuntimePlatform.OSXEditor && EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX;

            return onLinux || onWindows || onOSX;
        }

        string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);

            for (int i = 1; i < text.Length; i++)
            {
                if ((char.IsUpper(text[i]) && text[i - 1] != ' ') || (char.IsNumber(text[i]) && char.IsLetter(text[i - 1])))
                    newText.Append(' ');

                newText.Append(text[i]);
            }

            return newText.ToString();
        }

        void OnGUI()
        {
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Target Platform", AddSpacesToSentence(EditorUserBuildSettings.activeBuildTarget.ToString()), EditorStyles.largeLabel);

            EditorGUILayoutExtension.DrawSeparatorLine();

            EditorGUI.BeginDisabledGroup(TargetPlatformIsMobileOrWeb);
            EditorGUI.BeginChangeCheck();
            useCustomDirectory = EditorGUILayout.BeginToggleGroup("Use Custom Directory", useCustomDirectory);

            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
            specialFolders = (SpecialFolders)EditorGUILayout.EnumPopup("Special Folders", specialFolders);
            includeCompanyName = EditorGUILayout.Toggle("Include Company Name", includeCompanyName);
            includeProductName = EditorGUILayout.Toggle("Include Product Name", includeProductName);
            path = EditorGUILayout.TextField("Path", path);
            extension = EditorGUILayout.TextField("Extension", extension);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndToggleGroup();

            if (EditorGUI.EndChangeCheck())
            {
                if (TargetPlatformIsMobileOrWeb)
                    useCustomDirectory = false;

                CheckSavePath();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Separator();

            if (TargetPlatformIsCurrentPlatform())
                EditorGUILayout.HelpBox($"Destination preview: {previewPath}", validFolder ? MessageType.Info : MessageType.Error);

            EditorGUILayout.Separator();

            EditorGUI.BeginDisabledGroup(!validFolder || !TargetPlatformIsCurrentPlatform());

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
                    Process.Start(rootDirectory);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }
    }
}