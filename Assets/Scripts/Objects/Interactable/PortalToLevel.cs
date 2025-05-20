using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToLevel : MonoBehaviour
{

    [SerializeField] private string nameNewLocation;
    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (nameNewLocation == null)
        {
            Debug.Log("Задай название сцены для перемещения"); 
            return;
        }
        if(collision.gameObject.layer == LayerManager.playerLayer)
        {
            Dictionary<string, string> localized_nameLoc_text = LocalizationManager.Instance.GetLocalizedValue("ui_text", "name_location");
            GlobalData.NAME_NEW_LOCATION = nameNewLocation;
            GlobalData.NAME_NEW_LOCATION_TEXT = localized_nameLoc_text[nameNewLocation];

            await UIControl.Instance.SaveData();
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LoadingScreen");
            while(!asyncLoad.isDone)
            {
                await Task.Yield();
            }
        }
    }
}
