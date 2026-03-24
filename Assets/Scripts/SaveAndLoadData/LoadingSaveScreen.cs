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
    public CanvasGroup canvasGroup; // Добавьте компонент CanvasGroup для плавного исчезновения
    public float fadeSpeed = 1.5f;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadGameScene());
    }
    private IEnumerator LoadGameScene()
    {
        string savePath = GlobalData.SavePath ?? "";

        // Загружаем данные и сохраняем в GameDataHolder
        Task taskData = LoadData(savePath);
        Task taskArt = LoadArtifact(savePath);
        Task taskWorld = LoadWorldData(savePath);

        while (!taskData.IsCompleted || !taskArt.IsCompleted || !taskWorld.IsCompleted)
        {
            UpdateUI(0.2f); //Загрузка 20% когда загрузятся все данные
            yield return null;
        }

        // Загружаем игровую сцену асинхронно
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(GlobalData.NAME_NEW_LOCATION);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            // Масштабируем 0.9 в диапазон 0.2 - 0.8 для плавности UI
            float progress = 0.2f + (asyncLoad.progress / 0.9f) * 0.6f;
            UpdateUI(progress);
            yield return null;
        }
        //Разрешаем активацию сцены
        asyncLoad.allowSceneActivation = true;

        //Ждем, пока сцена загрузится полностью
        while(!asyncLoad.isDone)
            yield return null;

        //Ждем инициализации объектов в новой сцене
        while(!GlobalData.LoadedGame)
        {
            UpdateUI(0.95f);
            yield return null;
        }
        UpdateUI(1f);

        yield return StartCoroutine(FadeOut());
        Destroy(this.gameObject);

    }
    private void UpdateUI(float progress)
    {
        progressBar.value = progress;
        progressText.text = $"Loading: {progress * 100:F0}%";
    }
    private IEnumerator FadeOut()
    {
        if (canvasGroup == null) yield break;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * 2f;
            yield return null;
        }
    }
    private async Task LoadData(string savePath)
    {
        GameDataHolder.PlayerData = await SaveSystem.LoadDataAsync<PlayerData>(savePath + "player.json");

        Debug.Log("Данные загружены в LoadingScene.");
    }
    private async Task LoadArtifact(string savePath)
    {
        GameDataHolder.ArtifactsData = await SaveSystem.LoadDataAsync<ArtifactsData>(savePath + "artifacts.json");
    }
    private async Task LoadWorldData(string savePath)
    {
        GameDataHolder.WorldData = await SaveSystem.LoadDataAsync<WorldData>(savePath + "world_data.json");
    }
}
