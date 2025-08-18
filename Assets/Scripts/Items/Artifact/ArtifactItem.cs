using UnityEngine;
public interface IArtifact
{
    public int artifactLevel { get; set; }
}
public class ArtifactItem : Item, IArtifact
{
    public int artifactLevel {  get; set; }
    public ArtifactItem(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description,  TypeItem typeItem = TypeItem.Other, int _artifactLevel = 0) : base(id, name, maxCount, spriteID, quality, cost, description, typeItem)
    {
        artifactLevel = _artifactLevel;
    }
}
