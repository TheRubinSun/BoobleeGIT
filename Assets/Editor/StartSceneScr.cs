using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "StartSceneSettings", menuName = "Tools/Start Scene Settings")]
public class StartSceneScr : ScriptableObject
{
    public bool enabled = true;
    public bool returnToPreviousScene = true;  // <-- добавь это поле!
    public SceneAsset startScene;
}
