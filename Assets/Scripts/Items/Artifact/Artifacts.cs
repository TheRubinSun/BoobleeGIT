using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Artifacts : MonoBehaviour
{
    public static Artifacts Instance { get; private set; }
    public List<ArtifactObj> artifacts {  get; private set; } = new List<ArtifactObj>();

    private void Awake()
    {
        Instance = this;
    }
    public int AddNewArtifact(int levelArtefact)
    {
        artifacts.Add(new ArtifactObj(artifacts.Count, levelArtefact));
        return artifacts.Count - 1;
    }
    public void LoadOrNew(List<ArtifactObj> loadArt)
    {
        if(loadArt.Count == 0) artifacts = new List<ArtifactObj>() { new ArtifactObj(0)};
        else
        {
            artifacts = loadArt;
        }
    }
    public ArtifactObj GetArtifactForID(int id) => artifacts[id];

}
