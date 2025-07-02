using System.Collections.Generic;
using UnityEngine;

public static class GlobalWorld
{
    public static int numbTotalPoints;
    public static Dictionary<int, FarmPoint> FarmsPoints = new Dictionary<int, FarmPoint>();

    public static void LoadData(int _numbTotalPoints, Dictionary<int, FarmPoint> _FarmsPoint)
    {
        numbTotalPoints = _numbTotalPoints;
        FarmsPoints = _FarmsPoint;
    }
    public static void AddStageGround()
    {
        foreach (FarmPoint point in FarmsPoints.Values)
        {
            if (point.stage_ground == 3) continue;
            point.stage_ground++;
        }
    }
    public static int AddFarmPoint(FarmPoint point)
    {
        numbTotalPoints++;
        FarmsPoints[numbTotalPoints] = point;
        return numbTotalPoints;
    }
    public static void RemoveFarmPoint(int id)
    {
        FarmsPoints.Remove(id);
    }
}
public class FarmPoint
{
    public int IdDirtBed {  get; set; }
    public float X {  get; set; }
    public float Y { get; set; }
    public string seed_type { get; set; }
    public int stage_ground {  get; set; }
    public FarmPoint() { }
    public FarmPoint(int _IdDirtBed, float x, float y, string seed_type, int stage_ground)
    {
        IdDirtBed = _IdDirtBed;
        X = x;
        Y = y;
        this.seed_type = seed_type;
        this.stage_ground = stage_ground;
    }
    public FarmPoint(int _IdDirtBed, Vector2 pos, string seed_type, int stage_ground)
    {
        IdDirtBed = _IdDirtBed;
        X = pos.x;
        Y = pos.y;
        this.seed_type = seed_type;
        this.stage_ground = stage_ground;
    }
    public Vector2 GetPos() => new Vector2(X, Y);
}
