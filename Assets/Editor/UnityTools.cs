#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class UnityTools
{
    // Путь к стартовой сцене (как в Build Settings)
    private const string SettingsAssetPath = "Assets/Editor/StartGame.asset";
    private const string PreviousSceneKey = "UnityTools_PreviousScenePath";

    private static StartSceneScr _settings;
    static UnityTools()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        LoadSettings();

        if (_settings == null || !_settings.enabled || _settings.startScene == null)
            return;

        switch (state)
        {
            case PlayModeStateChange.ExitingEditMode:
                string currentScene = SceneManager.GetActiveScene().path;
                string targetScene = AssetDatabase.GetAssetPath(_settings.startScene);

                if (currentScene != targetScene)
                {
                    EditorPrefs.SetString(PreviousSceneKey, currentScene);

                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(targetScene);
                    }
                    else
                    {
                        EditorApplication.isPlaying = false;
                    }
                }
                break;

            case PlayModeStateChange.EnteredEditMode:
                if (EditorPrefs.HasKey(PreviousSceneKey))
                {
                    string previousScene = EditorPrefs.GetString(PreviousSceneKey);
                    EditorPrefs.DeleteKey(PreviousSceneKey); // Очистка ключа

                    if (!string.IsNullOrEmpty(previousScene))
                    {
                        EditorSceneManager.OpenScene(previousScene);
                    }
                }
                break;
        }
    }

    private static void LoadSettings()
    {
        if (_settings == null)
        {
            _settings = AssetDatabase.LoadAssetAtPath<StartSceneScr>(SettingsAssetPath);
        }
    }
}
#endif