using System.Collections.Generic;
using UnityEngine;

public static class GlobalWorld
{
    public static List<FarmPoint> FarmsPoint = new List<FarmPoint>();

    public static void LoadData(List<FarmPoint> _FarmsPoint)
    {
        FarmsPoint = _FarmsPoint;
    }
}
public class FarmPoint
{
    public float X {  get; set; }
    public float Y { get; set; }
    public int seed_type { get; set; }
    public int stage_ground {  get; set; }
    public FarmPoint(float x, float y, int seed_type, int stage_ground)
    {
        X = x;
        Y = y;
        this.seed_type = seed_type;
        this.stage_ground = stage_ground;
    }
}
