using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LandscapeManager : MonoBehaviour
{
    //用1,2,4,8,16,32表示各种地形
    public Dictionary<Vector2Int, int> landscapeMap;
    public Dictionary<Vector2Int, bool> visibilityMap;
    public Dictionary<Vector2Int, bool> buildabilityMap;
    public HashSet<Vector2Int> magnetics;
    public HashSet<Vector2Int> airTowers;
    public Tilemap tilemap;
    public int maxSize = 30;
    public static LandscapeManager Instance
    {
        get; private set;
    }
    private void Awake()
    {
        Instance = this;
        landscapeMap = new Dictionary<Vector2Int, int>();
        visibilityMap = new Dictionary<Vector2Int, bool>();
        for (int i = -maxSize; i <= maxSize; ++i)
        {
            for (int j = -20; j <= 20; ++j)
            {
                visibilityMap[new Vector2Int(i, j)] = false;
            }
        }
        for (int i = -10; i <= 10; ++i)
        {
            for (int j = -10; j <= 10; ++j)
            {
                visibilityMap[new Vector2Int(i, j)] = true;
            }
        }
        buildabilityMap = new Dictionary<Vector2Int, bool>();
        for (int i = -maxSize; i <= maxSize; ++i)
        {
            for (int j = -maxSize; j <= maxSize; ++j)
            {
                buildabilityMap[new Vector2Int(i, j)] = false;
            }
        }
        for (int i = -10; i <= 10; ++i)
        {
            for (int j = -10; j <= 10; ++j)
            {
                buildabilityMap[new Vector2Int(i, j)] = true;
            }
        }
        magnetics = new HashSet<Vector2Int>();
        airTowers = new HashSet<Vector2Int>();
        for (int i = -10; i <= 10; ++i)
        {
            for (int j = -10; j <= 10; ++j)
            {
                Vector2Int v = new(i, j);
                if (tilemap.GetTile(new(i, j, 0)) == null)
                {
                    landscapeMap[v] = 0;//虚空
                    continue;
                }
                string tileName = tilemap.GetTile(new(i, j, 0)).name;//卧槽这么写也能认识
                if (tileName.IndexOf("grass", StringComparison.OrdinalIgnoreCase) != -1)
                    landscapeMap[v] = 1;
                else if (tileName.IndexOf("scorched", StringComparison.OrdinalIgnoreCase) != -1)
                    landscapeMap[v] = 4;
                else if (tileName.IndexOf("land", StringComparison.OrdinalIgnoreCase) != -1)
                    landscapeMap[v] = 2;
                else if (tileName.IndexOf("desert", StringComparison.OrdinalIgnoreCase) != -1)
                    landscapeMap[v] = 8;
                else if (tileName.IndexOf("water", StringComparison.OrdinalIgnoreCase) != -1)
                    landscapeMap[v] = 16;
                else if (tileName.IndexOf("rock", StringComparison.OrdinalIgnoreCase) != -1)
                    landscapeMap[v] = 32;
            }
        }
        RecalculateVisibility();
        RecalculateBuildability();
    }
    private void Start()
    {

    }
    public void RecalculateVisibility()
    {
        float range = 20.1f;//硬编码
        for (int i = -maxSize; i <= maxSize; ++i)
        {
            for (int j = -maxSize; j <= maxSize; ++j)
            {
                visibilityMap[new Vector2Int(i, j)] = false;
            }
        }
        for (int i = -10; i <= 10; ++i)
        {
            for (int j = -10; j <= 10; ++j)
            {
                visibilityMap[new Vector2Int(i, j)] = true;
            }
        }
        foreach (Vector2Int v in magnetics)
        {
            int t = Mathf.CeilToInt(range);
            for (int ii = -t; ii <= t; ++ii)
            {
                for (int jj = -t; jj <= t; ++jj)
                {
                    if (ii * ii + jj * jj <= range * range)
                    {
                        visibilityMap[v + new Vector2Int(ii, jj)] = true;
                    }
                }
            }
        }
        foreach (var building in BuildingManager.Instance.buildings)
        {
            if (building.IsVisible())
            {
                TileBase tile = GetBuildingTile(building.name);
                RealTimeBuilder.Instance.tilemap.SetTile(new Vector3Int(building.position.x, building.position.y, 3), tile);
            }
        }
        Vector2Int v_;
        for (int i = -maxSize; i <= maxSize; ++i)
        {
            for (int j = -maxSize; j <= maxSize; ++j)
            {
                v_ = new Vector2Int(i, j);
                var register = BuildingManager.Instance.landUseRegister;
                if (!visibilityMap[v_])
                {
                    if (!register.ContainsKey(v_) || register[v_] == null || !register[v_].IsVisible())
                        RealTimeBuilder.Instance.tilemap.SetTile(new Vector3Int(v_.x, v_.y, 3), RealTimeBuilder.Instance.mistTile);
                    else
                    {
                        RealTimeBuilder.Instance.tilemap.SetTile(new Vector3Int(v_.x, v_.y, 3), null);
                    }
                }
                else
                {
                    if (RealTimeBuilder.Instance.tilemap.GetTile(new Vector3Int(v_.x, v_.y, 3)) == RealTimeBuilder.Instance.mistTile)
                    {
                        RealTimeBuilder.Instance.tilemap.SetTile(new Vector3Int(v_.x, v_.y, 3), null);
                    }
                }
            }
        }
    }
    public TileBase GetBuildingTile(string name)
    {
        return Resources.Load<TileBase>("Tiles/Building/" + name);
    }
    public void RecalculateBuildability()
    {
        float range = 12.1f;//硬编码
        for (int i = -maxSize; i <= maxSize; ++i)
        {
            for (int j = -maxSize; j <= maxSize; ++j)
            {
                buildabilityMap[new Vector2Int(i, j)] = false;
            }
        }
        for (int i = -10; i <= 10; ++i)
        {
            for (int j = -10; j <= 10; ++j)
            {
                buildabilityMap[new Vector2Int(i, j)] = true;
            }
        }
        foreach (Vector2Int v in airTowers)
        {
            int t = Mathf.CeilToInt(range);
            for (int ii = -t; ii <= t; ++ii)
            {
                for (int jj = -t; jj <= t; ++jj)
                {
                    if (ii * ii + jj * jj <= range * range)
                    {
                        buildabilityMap[v + new Vector2Int(ii, jj)] = true;
                    }
                }
            }
        }
        for (int i = -maxSize; i <= maxSize; ++i)
        {
            for (int j = -maxSize; j <= maxSize; ++j)
            {
                RealTimeBuilder.Instance.tilemap.SetTile(new Vector3Int(i, j, 1), buildabilityMap[new Vector2Int(i, j)] ? null : RealTimeBuilder.Instance.unbuildableTile);
            }
        }
    }
}
