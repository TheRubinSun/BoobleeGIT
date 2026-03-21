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
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadGameScene());
    }
    //private IEnumerator LoadGameScene()
    //{
    //    string savePath = GlobalData.SavePath ?? "";

    //    // Загружаем данные в GameDataHolder
    //    yield return LoadData(savePath);
    //    yield return LoadArtifact(savePath);
    //    yield return LoadWorldData(savePath);

    //    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(GlobalData.NAME_NEW_LOCATION);
    //    asyncLoad.allowSceneActivation = false;

    //    // Фаза 1: Загрузка самой сцены (ресурсов)
    //    while (asyncLoad.progress < 0.9f)
    //    {
    //        float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
    //        UpdateUI(progress * 0.5f); // Это первые 50% прогресса
    //        yield return null;
    //    }

    //    // Активируем сцену
    //    asyncLoad.allowSceneActivation = true;

    //    // Фаза 2: Ждем, пока GameManager в новой сцене скажет "Я готов"
    //    // Мы используем флаг, который вы уже создали в GlobalData
    //    while (!GlobalData.LoadedGame)
    //    {
    //        // Здесь можно крутить какую-то анимацию загрузки
    //        UpdateUI(0.5f + 0.5f * 0.8f); // Имитируем прогресс до 90%
    //        yield return null;
    //    }

    //    UpdateUI(1f); // 100% готовность

    //    // Плавно скрываем экран загрузки
    //    yield return StartCoroutine(FadeOut());

    //    Destroy(gameObject); // Теперь можно удалять
    //}
    //private void Start()
    //{
    //    StartCoroutine(LoadGameScene());
    //}

    private IEnumerator LoadGameScene()
    {
        string savePath = GlobalData.SavePath;
        if (savePath == null) savePath = "";

        // Загружаем данные и сохраняем в GameDataHolder
        yield return LoadData(savePath);
        yield return LoadArtifact(savePath);
        yield return LoadWorldData(savePath);

        // Загружаем игровую сцену асинхронно
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(GlobalData.NAME_NEW_LOCATION);
        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            progressBar.value = progress;
            progressText.text = $"Loading: {progress * 100:F0}%";
            if (asyncLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1f);
                asyncLoad.allowSceneActivation = true;
            }
        }
        yield return null;
        yield return new WaitForSeconds(1f);
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
    private IEnumerator WaitAndStartGame()
    {
        while(!GlobalData.LoadedGame)
        {
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
