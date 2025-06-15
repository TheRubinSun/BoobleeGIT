using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Artifacts : MonoBehaviour
{
    public static Artifacts Instance { get; private set; }
    public List<ArtifactObj> artifacts { get; private set; } = new List<ArtifactObj>();

    private void Awake()
    {
        Instance = this;
    }
    public int AddNewArtifact(int levelArtefact)
    {
        artifacts.Add(new ArtifactObj(artifacts.Count, levelArtefact));
        return artifacts.Count - 1;
    }
    public int AddNewArtifact(int levelArtefact, System.Random random)
    {
        artifacts.Add(new ArtifactObj(artifacts.Count, levelArtefact, random));
        return artifacts.Count - 1;
    }
    public void LoadOrNew(List<ArtifactObj> loadArt)
    {
        loadArt = ClearExstraArtif(loadArt);
        if (loadArt.Count == 0) 
            artifacts = new List<ArtifactObj>() { new ArtifactObj(0)};
        else
        {
            artifacts = loadArt;
        }
    }
    public ArtifactObj GetArtifact(int index)
    {
        if (artifacts == null || artifacts.Count == 0 || index < 0 || index >= artifacts.Count)
            return artifacts[0];
        return artifacts[index];
    }
    public List<ArtifactObj> ClearExstraArtif(List<ArtifactObj> artif)
    {
        artif.RemoveAll(artifacts => artifacts.ID_Art != 0 && artifacts.isAllNull());
        return artif;
    }
    public ArtifactObj GetArtifactForID(int id) => artifacts[id];

}
