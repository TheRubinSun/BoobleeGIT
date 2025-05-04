using UnityEngine;



public class OpenTools : MonoBehaviour
{
    [SerializeField] Tools selectTools;
    [SerializeField] CraftTable craftTable;

    private void Update()
    {
        
    }
    public void OpenSelectTools()
    {
        switch(selectTools)
        {
            case Tools.None: return;
            case Tools.Craft:
                {
                    OpenStantion();
                    break;
                }
            case Tools.Trade:
                {
                    break;
                }

        }
    }
    private void OpenStantion()
    {
        switch (craftTable)
        {
            case CraftTable.None: return;
            case CraftTable.Workbench:
                {
                    Debug.Log("Open workbench");
                    break;
                }
            case CraftTable.Alchemy_Station:
                {
                    break;
                }
            case CraftTable.Smelter:
                {
                    break;
                }
            case CraftTable.Anvil:
                {
                    break;
                }
        }
    }
}

public enum Tools
{
    None,
    Craft,
    Trade
}
