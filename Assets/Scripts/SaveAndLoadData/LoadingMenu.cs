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

    private float progress = 0f;

    public string spriteSheetPath;
    private void Start()
    {
        StartCoroutine(LoadGameScene());
    }
    private IEnumerator LoadGameScene()
    {
        int totalSteps = 4;
        int currentStep = 0;

        // Загружаем данные и сохраняем в GameDataHolder
        yield return StartCoroutine(LoadDataCoroutine());
        UpdateProgress(++currentStep, totalSteps);

        yield return StartCoroutine(LoadLanguageCoroutine());
        UpdateProgress(++currentStep, totalSteps);

        yield return StartCoroutine(LoadSprites(spriteSheetPath));
        UpdateProgress(++currentStep, totalSteps);

        // Загружаем игровую сцену асинхронно
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu");
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float sceneProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            UpdateProgress(currentStep + sceneProgress, totalSteps);

            if (asyncLoad.progress >= 0.9f)
            {
                ItemsList.LoadSprites();

                Classes.LoadOrCreateClasses(GameDataHolder.RoleClassesData.role_Classes_data);

                ItemsList.LoadOrCreateItemList(GameDataHolder.ItemsData.item_List_data);

                EnemyList.LoadOrCreateMobsList(GameDataHolder.EnemyData.mob_list_data);

                yield return new WaitForSeconds(1f);
                asyncLoad.allowSceneActivation = true;
            }
        }


        yield return null;
    }
    private IEnumerator LoadDataCoroutine()
    {
        Task loadDataTask = LoadData();
        while (!loadDataTask.IsCompleted)
        {
            yield return null;
        }
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
    //private async Task LoadSprites(string addressableKey)
    //{
    //    List<Sprite> sprites = new List<Sprite> { null };
    //    AsyncOperationHandle<IList<Sprite>> handle = Addressables.LoadAssetAsync<IList<Sprite>>(addressableKey);
    //    await handle.Task;
    //    if (handle.Status == AsyncOperationStatus.Succeeded)
    //    {
    //        sprites.AddRange(handle.Result);
    //        Addressables.Release(handle);
    //        GameDataHolder.spriteList = sprites.ToArray();
    //    }
    //    else
    //    {
    //        Debug.LogError("Не удалось загрузить спрайты через Addressables");
    //    }
    //}
    private IEnumerator LoadSprites(string addressableKey)
    {
        //if (string.IsNullOrEmpty(addressableKey))
        //{
        //    Debug.LogError("LoadSprites: передан пустой addressableKey!");
        //    GameDataHolder.spriteList = new Sprite[0]; // Чтобы не было null-ошибок
        //    yield break;
        //}
        //Dictionary<int, Sprite> spriteById = new();
        //List<Sprite> sprites = new List<Sprite> { null }; //первый элемент `null`
        //AsyncOperationHandle<IList<Sprite>> handle = Addressables.LoadAssetAsync<IList<Sprite>>(addressableKey);

        //while (!handle.IsDone) // Ждем окончания загрузки
        //{
        //    progressBar.value = handle.PercentComplete; // Обновляем прогресс загрузки
        //    yield return null;
        //}

        //if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null && handle.Result.Count > 0)
        //{
        //    sprites.AddRange(handle.Result);
        //    GameDataHolder.spriteList = sprites.ToArray();
        //    Debug.Log($"Успешно загружено {GameDataHolder.spriteList.Length} спрайтов.");
        //}
        //else
        //{
        //    Debug.LogError("Не удалось загрузить спрайты через Addressables или список пуст.");
        //    GameDataHolder.spriteList = new Sprite[0]; // Чтобы избежать ошибок
        //}

        //Addressables.Release(handle);

        if (string.IsNullOrEmpty(addressableKey))
        {
            Debug.LogError("LoadSprites: пустой ключ");
            yield break;
        }

        AsyncOperationHandle<IList<Sprite>> handle =
            Addressables.LoadAssetAsync<IList<Sprite>>(addressableKey);

        while (!handle.IsDone)
        {
            progressBar.value = handle.PercentComplete;
            yield return null;
        }

        if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
        {
            Debug.LogError("LoadSprites: ошибка загрузки");
            yield break;
        }

        Dictionary<int, Sprite> spriteById = new Dictionary<int, Sprite>();

        foreach (Sprite sprite in handle.Result)
        {
            // Формат: SpriteSheet_0, SpriteSheet_1, ...
            int underscoreIndex = sprite.name.LastIndexOf('_');

            if (underscoreIndex < 0)
            {
                Debug.LogWarning($"Некорректное имя спрайта: {sprite.name}");
                continue;
            }

            if (int.TryParse(sprite.name.Substring(underscoreIndex + 1), out int id))
            {
                spriteById[id] = sprite;
            }
            else
            {
                Debug.LogWarning($"Не удалось извлечь ID из имени спрайта: {sprite.name}");
            }
        }

        GameDataHolder.spriteById = spriteById;

        Debug.Log($"Sprite Sheet загружен: {spriteById.Count} спрайтов");
    }
    private void UpdateProgress(float step, float totalStep)
    {
        progress = step / totalStep;
        progressBar.value = progress;
        progressText.text = $"Loading: {progress * 100:F0}%";
    }
}
