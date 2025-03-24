using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSaveScreen : MonoBehaviour
{
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    private void Start()
    {
        StartCoroutine(LoadGameScene());
    }
    private IEnumerator LoadGameScene()
    {
        string savePath = GlobalData.SavePath;
        if (savePath == null) savePath = "";

        // Загружаем данные и сохраняем в GameDataHolder
        yield return LoadData(savePath);

        // Загружаем игровую сцену асинхронно
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameVillage");
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            progressBar.value = progress;
            progressText.text = $"Loading: {progress * 100:F0}%";

            if(asyncLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1f);
                asyncLoad.allowSceneActivation = true;
            }
        }

        yield return null;
    }
    private async Task LoadData(string savePath)
    {
        GameDataHolder.PlayerData = await SaveSystem.LoadDataAsync<PlayerData>(savePath + "player.json");

        Debug.Log("Данные загружены в LoadingScene.");
    }
}
