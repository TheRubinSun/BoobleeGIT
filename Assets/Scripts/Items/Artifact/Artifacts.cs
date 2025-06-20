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
    //public int AddNewArtifact(int levelArtefact)
    //{
    //    ArtifactObj artif = new ArtifactObj(artifacts.Count, levelArtefact);
    //    if (artif.chars_level == 0 && artif.curse_level == 0)
    //    {
    //        return 0;
    //    }
    //    else
    //    {
    //        artifacts.Add(artif);
    //        return artifacts.Count - 1;
    //    }
    //}

    /// <summary>
    /// 0 элемент пустой, 1 элемент c нулевыми аттрибутами, 2 уже с ненулевыми параметрами
    /// </summary>
    public int AddNewArtifact(int levelArtefact, System.Random random = null)
    {
        ArtifactObj artif;
        if (random == null) artif = new ArtifactObj(artifacts.Count, levelArtefact);
        else
        {
            int seed = random.Next(1000000, 9999999);
            int id = HaveThatArtifactSeedOrNot(seed);

            if (id == -1)
                artif = new ArtifactObj(artifacts.Count, levelArtefact, random, seed);
            else return id;
        }
        if (artif.chars_level == 0 && artif.curse_level == 0)
        {
            return 1;
        }
        else
        {
            artifacts.Add(artif);
            return artifacts.Count - 1;
        }

    }
    private int HaveThatArtifactSeedOrNot(int seed)
    {
        for (int i = 1; i < artifacts.Count; i++) //Начинаем с 1 т.к. 0 = null
        {
            if (artifacts[i].SEED_Art == seed) return i;
        }
        return -1;
    }
    public void LoadOrNew(List<ArtifactObj> loadArt)
    {
        //loadArt = ClearExstraArtif(loadArt);
        if (loadArt.Count == 0) 
            artifacts = new List<ArtifactObj>() {null, new ArtifactObj(1)};
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
    //public List<ArtifactObj> ClearExstraArtif(List<ArtifactObj> artif)
    //{
    //    artif.RemoveAll(artifact => artifact.ID_Art != 0 && artifact.isAllNull());
    //    return artif;
    //}
    public ArtifactObj GetArtifactForID(int id) => artifacts[id];

}
