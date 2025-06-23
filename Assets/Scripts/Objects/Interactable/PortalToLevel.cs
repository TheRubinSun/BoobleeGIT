using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToLevel : MonoBehaviour
{

    [SerializeField] private string nameNewLocation;
    [SerializeField] private bool add_lvl_left;
    [SerializeField] private bool saveZone;
    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (nameNewLocation == null)
        {
            Debug.Log("Задай название сцены для перемещения"); 
            return;
        }
        if(collision.gameObject.layer == LayerManager.playerLayer)
        {
            if(add_lvl_left) GlobalData.cur_lvl_left++;

            Dictionary<string, string> localized_nameLoc_text = LocalizationManager.Instance.GetLocalizedValue("ui_text", "name_location");
            GlobalData.NAME_NEW_LOCATION = nameNewLocation;
            GlobalData.NAME_NEW_LOCATION_TEXT = localized_nameLoc_text[nameNewLocation];
            GlobalData.saveZone = saveZone;

            await UIControl.Instance.SaveData();
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LoadingScreen");
            while(!asyncLoad.isDone)
            {
                await Task.Yield();
            }
        }
    }
}
