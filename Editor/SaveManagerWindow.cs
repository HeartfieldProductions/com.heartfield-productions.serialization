using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEditor;
using Heartfield.Serialization;

namespace HeartfieldEditor.SerializableData
{
    class SaveManagerWindow : EditorWindow
    {
        bool useCustomDirectory;
        Environment.SpecialFolder specialFolders;
        bool includeCompanyName;
        bool includeProductName;
        string path;
        string extension = "sav";
        string finalPath;

        [MenuItem("Heartfield Productions/Serialization/Save Manager")]
        static void Create()
        {
            var window = (SaveManagerWindow)GetWindow(typeof(SaveManagerWindow), false, "Save Manager");
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.Space();

            useCustomDirectory = EditorGUILayout.BeginToggleGroup("Use Custom Directory", useCustomDirectory);

            specialFolders = (Environment.SpecialFolder)EditorGUILayout.EnumPopup("Special Folders", specialFolders);
            includeCompanyName = EditorGUILayout.Toggle("Include Company Name", includeCompanyName);
            includeProductName = EditorGUILayout.Toggle("Include Product Name", includeProductName);
            path = EditorGUILayout.TextField("Path", path);

            finalPath = Environment.GetFolderPath(specialFolders);

            if (includeCompanyName)
                finalPath = Path.Combine(finalPath, Application.companyName);

            if (includeProductName)
                finalPath = Path.Combine(finalPath, Application.productName);

            finalPath = Path.Combine(finalPath, path);

            EditorGUILayout.EndToggleGroup();

            if (!useCustomDirectory)
                finalPath = Application.persistentDataPath;

            Settings.path = finalPath;

            EditorGUI.BeginChangeCheck();
            extension = EditorGUILayout.TextField("Extension", extension);

            if (EditorGUI.EndChangeCheck())
                Settings.fileExtension = extension;

            string fileName = $"[save name][slot].{Settings.fileExtension}";
            EditorGUILayout.LabelField(Path.Combine(Settings.path, fileName), EditorStyles.miniLabel);

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Test Save Data"))
            {
                SaveManager.Save("Test", 1, out _);
            }

            bool hasSave = SaveManager.SaveFilesAmount() > 0;

            if (hasSave)
            {
                if (GUILayout.Button("Open Directory"))
                {
                    Process.Start(Settings.path);
                }
            }
        }
    }
}