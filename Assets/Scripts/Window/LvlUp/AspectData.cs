using UnityEngine;

[CreateAssetMenu(fileName = "NewAspect", menuName = "Game/Aspect")]
public class AspectData : ScriptableObject
{
    public AspectName name_asp;
    public Color32 BG_Color;
    public Color32 icon_color;
    public float min_value;
    public float max_value;
    public Sprite sprite;
}
