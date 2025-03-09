using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour 
{
    public Slider progressBar;
    public TextMeshProUGUI progressText;
    private void Start()
    {
        StartCoroutine(LoadGameScene());
    }
    private IEnumerator LoadGameScene()
    {

        // Загружаем данные и сохраняем в GameDataHolder
        yield return LoadData();

        // Загружаем игровую сцену асинхронно
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu");
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
    }
    private async Task LoadData()
    {
        GameDataHolder.savesDataInfo = await SaveSystem.LoadDataAsync<SavesDataInfo>("saves_info.json");

        GameDataHolder.ItemsData = await SaveSystem.LoadDataAsync<ItemsData>("items.json");
        GameDataHolder.EnemyData = await SaveSystem.LoadDataAsync<EnemyData>("enemies.json");
        GameDataHolder.RoleClassesData = await SaveSystem.LoadDataAsync<RoleClassesData>("role_classes_data.json");
        GameDataHolder.ItemsDropOnEnemy = await SaveSystem.LoadDataAsync<ItemsDropOnEnemy>("item_drop.json");

        if(GameDataHolder.savesDataInfo.language != null)
        {
            LocalizationManager.Instance.LoadLocalization(GameDataHolder.savesDataInfo.language);
            GlobalData.cur_language = GameDataHolder.savesDataInfo.language;
            Debug.Log($"Загружен язык {GameDataHolder.savesDataInfo.language}");
        }
        else
        {
            LocalizationManager.Instance.LoadLocalization("en");
            GlobalData.cur_language = "en";
            Debug.Log($"Загружен стандартный en");
        }
        

        Debug.Log("Данные загружены в LoadingMenu.");
    }
}
