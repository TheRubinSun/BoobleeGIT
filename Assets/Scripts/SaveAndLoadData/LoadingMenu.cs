using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour 
{
    public Slider progressBar;
    public TextMeshProUGUI progressText;
    public CanvasGroup canvasGroup;
    public float fadeSpeed = 1.5f;

    [Header("Settings")]
    public string spriteItemsSheetPath;
    public string spritePlayerHairPath;
    public string spritePlayerHeadPath;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadGameScene());
    }
    private IEnumerator LoadGameScene()
    {
        int totalSteps = 6;
        float currentTargetProgress = 0f;

        // Загружаем данные и сохраняем в GameDataHolder
        yield return StartCoroutine(LoadDataCoroutine());
        currentTargetProgress = 1f / totalSteps;

        //Локализация
        yield return StartCoroutine(LoadLanguageCoroutine());
        currentTargetProgress = 2f / totalSteps;

        //Спрайты
        yield return StartCoroutine(LoadSprites(spriteItemsSheetPath, GameDataHolder.spriteItemsById, 2f, 3f, totalSteps));
        yield return StartCoroutine(LoadSprites(spritePlayerHairPath, GameDataHolder.spritePlayerHairById, 3f, 4f, totalSteps));
        yield return StartCoroutine(LoadSprites(spritePlayerHeadPath, GameDataHolder.spritePlayerHeadById, 4f, 5f, totalSteps));
        currentTargetProgress = 3f / totalSteps;

        // Загружаем игровую сцену асинхронно
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu");
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            float sceneProgress = asyncLoad.progress / 0.9f;
            float totalProgress = (5f + sceneProgress) / totalSteps;
            UpdateUI(totalProgress);
            yield return null;
        }

        // Выполняем тяжелую логику инициализации ПЕРЕД активацией
        ItemsList.LoadSprites();
        Classes.LoadOrCreateClasses(GameDataHolder.RoleClassesData.role_Classes_data);
        ItemsList.LoadOrCreateItemList(GameDataHolder.ItemsData.item_List_data);
        EnemyList.LoadOrCreateMobsList(GameDataHolder.EnemyData.mob_list_data);

        yield return null;

        ItemDropEnemy.LoadOrCreate(GameDataHolder.ItemsDropOnEnemy.namesKeys);
        RecipesCraft.LoadItemInCrafts();

        UpdateUI(1f); // Показываем 100%
        yield return new WaitForSeconds(0.5f);

        asyncLoad.allowSceneActivation = true;

        // Ждем пока сцена полностью станет активной
        while (!asyncLoad.isDone) yield return null;

        // ПЛАВНЫЙ ПЕРЕХОД (Исчезновение)
        yield return StartCoroutine(FadeOut());

        Destroy(gameObject);
    }
    private IEnumerator FadeOut()
    {
        if (canvasGroup == null) yield break;
        while(canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= 2f * Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator LoadDataCoroutine()
    {
        Task loadDataTask = LoadData();

        while (!loadDataTask.IsCompleted)
            yield return null;

        if(loadDataTask.IsFaulted)
            Debug.LogError(loadDataTask.Exception.ToString());
    }
    private async Task LoadData()
    {
        GameDataHolder.savesDataInfo = await SaveSystem.LoadDataAsync<SavesDataInfo>("saves_info.json");
        Hotkeys.LoadBind((await SaveSystem.LoadDataAsync<SaveDataBinds>("keyBinds.json")).saveKeyBindings);
        GameDataHolder.ItemsData = await SaveSystem.LoadDataAsync<ItemsData>("items.json");
        GameDataHolder.EnemyData = await SaveSystem.LoadDataAsync<EnemyData>("enemies.json");
        GameDataHolder.RoleClassesData = await SaveSystem.LoadDataAsync<RoleClassesData>("role_classes_data.json");

        GameDataHolder.ItemsDropOnEnemy = await SaveSystem.LoadDataAsync<ItemsDropOnEnemy>("item_drop.json");
        RecipesCraft.LoadAllCrafts((await SaveSystem.LoadDataAsync<CraftsRecipesData>("recipes_crafts_data.json")).craftsRecipesData);
        await GlobalPrefabs.LoadPrefabs();

        Debug.Log("Данные загружены в LoadingMenu.");
    }
    private IEnumerator LoadLanguageCoroutine()
    {
        Task loadLanguageTask = LoadLanguage();
        while (!loadLanguageTask.IsCompleted)
        {
            yield return null;
        }
    }
    private async Task LoadLanguage()
    {
        if (GameDataHolder.savesDataInfo.language != null)
        {
            await GlobalData.LocalizationManager.LoadLocalization(GameDataHolder.savesDataInfo.language);
            GlobalData.cur_language = GameDataHolder.savesDataInfo.language;
            Debug.Log($"Загружен язык {GameDataHolder.savesDataInfo.language}");
        }
        else
        {
            await GlobalData.LocalizationManager.LoadLocalization("en");
            GlobalData.cur_language = "en";
            Debug.Log($"Загружен стандартный en");
        }
    }
    private IEnumerator LoadSprites(string path, Dictionary<int, Sprite> spriteData, float startStep, float endStep, int total)
    {
        AsyncOperationHandle<IList<Sprite>> handle = Addressables.LoadAssetAsync<IList<Sprite>>(path);

        while (!handle.IsDone)
        {
            float subProgress = handle.PercentComplete;
            float totalProgress = Mathf.Lerp(startStep, endStep, subProgress) / total;
            UpdateUI(totalProgress);
            yield return null;
        }

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            ProgressSpriteSheet(handle.Result, spriteData);
        }
        Addressables.Release(handle);
        yield break;
    }
    private void ProgressSpriteSheet(IList<Sprite> sprites, Dictionary<int, Sprite> spriteData)
    {
        spriteData.Clear();
        foreach (Sprite sprite in sprites)
        {
            int underscoreIndex = sprite.name.LastIndexOf('_');
            if (underscoreIndex >= 0 && int.TryParse(sprite.name.Substring(underscoreIndex + 1), out int id)) //Очищяем все символы кроме id спрайта и парсим в int
                spriteData[id] = sprite; //Под этот id записываем спрайт
        }
    }
    private void UpdateUI(float value)
    {
        progressBar.value = Mathf.MoveTowards(progressBar.value, value, Time.deltaTime * 2f);
        progressText.text = $"Loading: {progressBar.value * 100:F0}%";
    }

    //private void UpdateProgress(float step, float totalStep)
    //{
    //    progress = step / totalStep;
    //    progressBar.value = progress;
    //    progressText.text = $"Loading: {progress * 100:F0}%";
    //}
}
