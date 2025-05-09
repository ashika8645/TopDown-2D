using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WalkerGenerator : MonoBehaviour
{
    public enum GridType { EMPTY, FLOOR, WALL }

    [Header("Tilemaps")]
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Tilemap cosmeticTilemap;

    [Header("Tiles")]
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase cosmeticTile;

    [Header("Map Settings")]
    public int desiredFloorTiles = 1500;
    public int maxWalkers = 100;
    public float changeChance = 0.5f;
    public int floorChunkSize = 2;
    public int wallThickness = 2;

    [Header("Player Settings")]
    public GameObject playerSpawnPrefab;

    [Header("Spawnable Prefabs")]
    public GameObject[] destructiblePrefabs;
    public GameObject[] treePrefabs;
    public GameObject[] enemyPrefabs;
    public int enemyCount = 5;
    public int destructibleClusterCount = 15;
    public int treeCount = 10;

    [Header("Special Prefabs")]
    public GameObject shopPrefab;

    [Header("UI References")]
    public GameObject loadingScreen;

    private Dictionary<Vector2Int, GridType> grid = new();
    private List<WalkerObject> walkers = new();
    private int floorCount = 0;
    private Vector2Int playerSpawnPos;
    private Vector2Int shopPos;
    private HashSet<Vector2Int> cosmeticPositions = new(); 

    void Start()
    {
        ShowLoadingScreen(true);
        GenerateMap();
    }

    void GenerateMap()
    {
        Vector2Int startPos = Vector2Int.zero;
        walkers.Add(new WalkerObject(startPos, GetRandomDirection(), changeChance));
        SetFloorChunk(startPos.x, startPos.y);
        StartCoroutine(WalkRoutine());
    }

    IEnumerator WalkRoutine()
    {
        while (floorCount < desiredFloorTiles)
        {
            foreach (WalkerObject walker in walkers)
            {
                SetFloorChunk(Mathf.RoundToInt(walker.Position.x), Mathf.RoundToInt(walker.Position.y));
            }

            ChanceToRedirect();
            ChanceToCreate();
            ChanceToRemove();
            UpdateWalkers();
            yield return null;
        }

        CreateWalls();
        GenerateCosmetic();
        SpawnObjects();
        SpawnPlayer();
        ShowLoadingScreen(false);
    }

    void SetFloorChunk(int baseX, int baseY)
    {
        for (int dx = 0; dx < floorChunkSize; dx++)
        {
            for (int dy = 0; dy < floorChunkSize; dy++)
            {
                int x = baseX + dx;
                int y = baseY + dy;
                Vector2Int pos = new Vector2Int(x, y);

                if (!grid.ContainsKey(pos) || grid[pos] != GridType.FLOOR)
                {
                    grid[pos] = GridType.FLOOR;
                    floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                    floorCount++;
                }
            }
        }
    }

    void CreateWalls()
    {
        List<Vector2Int> wallPositions = new();

        foreach (var pair in grid)
        {
            if (pair.Value != GridType.FLOOR) continue;

            int x = pair.Key.x;
            int y = pair.Key.y;

            for (int dx = -wallThickness; dx <= wallThickness; dx++)
            {
                for (int dy = -wallThickness; dy <= wallThickness; dy++)
                {
                    Vector2Int pos = new Vector2Int(x + dx, y + dy);
                    if (!grid.ContainsKey(pos))
                    {
                        wallPositions.Add(pos);
                    }
                }
            }
        }

        foreach (var pos in wallPositions)
        {
            grid[pos] = GridType.WALL;
            wallTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), wallTile);
            floorTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), floorTile);
        }
    }

    void GenerateCosmetic()
    {
        int outerMinX = -4;
        int outerMaxX = 4;
        int outerMinY = -4;
        int outerMaxY = 4;
        int thickness = 2;

        for (int i = 0; i < thickness; i++)
        {
            int minX = outerMinX + i;
            int maxX = outerMaxX - i;
            int minY = outerMinY + i;
            int maxY = outerMaxY - i;

            for (int x = minX; x <= maxX; x++)
            {
                TrySetCosmeticTile(new Vector2Int(x, minY));
                TrySetCosmeticTile(new Vector2Int(x, maxY));
            }

            for (int y = minY + 1; y < maxY; y++)
            {
                TrySetCosmeticTile(new Vector2Int(minX, y));
                TrySetCosmeticTile(new Vector2Int(maxX, y));
            }
        }

        GenerateCosmeticBranches(new Vector2Int(0, outerMaxY + 1), Vector2Int.up);
        GenerateCosmeticBranches(new Vector2Int(0, outerMinY - 1), Vector2Int.down);
        GenerateCosmeticBranches(new Vector2Int(outerMinX - 1, 0), Vector2Int.left);
        GenerateCosmeticBranches(new Vector2Int(outerMaxX + 1, 0), Vector2Int.right);

        GenerateIsolatedCosmeticTiles(30);
    }

    void GenerateCosmeticBranches(Vector2Int startPos, Vector2Int initialDir)
    {
        Vector2Int pos = startPos;
        Vector2Int dir = initialDir;
        int steps = 0;
        int maxSteps = 100;

        for (int i = 0; i < maxSteps; i++)
        {
            Vector2Int perpendicular = new Vector2Int(-dir.y, dir.x);

            for (int offset = -1; offset <= 1; offset++)
            {
                Vector2Int drawPos = pos + perpendicular * offset;
                TrySetCosmeticTile(drawPos);
            }

            pos += dir;
            steps++;

            if (steps % 5 == 0 && Random.value < 0.3f)
            {
                dir = GetTurnedDirection(dir, Random.value < 0.5f);
            }

            if (Mathf.Abs(pos.x) > 50 || Mathf.Abs(pos.y) > 50)
                break;
        }
    }

    void TrySetCosmeticTile(Vector2Int pos)
    {
        if (grid.ContainsKey(pos) && grid[pos] == GridType.FLOOR && !cosmeticPositions.Contains(pos))
        {
            cosmeticTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), cosmeticTile);
            cosmeticPositions.Add(pos);
        }
    }

    Vector2Int GetTurnedDirection(Vector2Int currentDir, bool turnLeft)
    {
        if (currentDir == Vector2Int.up) return turnLeft ? Vector2Int.left : Vector2Int.right;
        if (currentDir == Vector2Int.down) return turnLeft ? Vector2Int.right : Vector2Int.left;
        if (currentDir == Vector2Int.left) return turnLeft ? Vector2Int.down : Vector2Int.up;
        if (currentDir == Vector2Int.right) return turnLeft ? Vector2Int.up : Vector2Int.down;
        return Vector2Int.right;
    }

    void GenerateIsolatedCosmeticTiles(int count)
    {
        List<Vector2Int> candidates = new();

        foreach (var pos in grid.Keys)
        {
            if (grid[pos] != GridType.FLOOR || cosmeticPositions.Contains(pos)) continue;

            bool hasNeighbor = false;
            for (int dx = -1; dx <= 1 && !hasNeighbor; dx++)
            {
                for (int dy = -1; dy <= 1 && !hasNeighbor; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    Vector2Int neighbor = new(pos.x + dx, pos.y + dy);
                    if (cosmeticPositions.Contains(neighbor)) hasNeighbor = true;
                }
            }

            if (!hasNeighbor)
            {
                candidates.Add(pos);
            }
        }

        Shuffle(candidates);

        for (int i = 0; i < Mathf.Min(count, candidates.Count); i++)
        {
            Vector2Int pos = candidates[i];
            cosmeticTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), cosmeticTile);
            cosmeticPositions.Add(pos);
        }
    }

    void UpdateWalkers()
    {
        for (int i = 0; i < walkers.Count; i++)
        {
            WalkerObject w = walkers[i];
            w.Position += w.Direction;
            walkers[i] = w;
        }
    }

    void ChanceToRedirect()
    {
        for (int i = 0; i < walkers.Count; i++)
        {
            if (Random.value < walkers[i].ChanceToChange)
            {
                WalkerObject w = walkers[i];
                w.Direction = GetRandomDirection();
                walkers[i] = w;
            }
        }
    }

    void ChanceToCreate()
    {
        int initialCount = walkers.Count;
        for (int i = 0; i < initialCount; i++)
        {
            if (Random.value < walkers[i].ChanceToChange && walkers.Count < maxWalkers)
            {
                WalkerObject w = new WalkerObject(walkers[i].Position, GetRandomDirection(), changeChance);
                walkers.Add(w);
            }
        }
    }

    void ChanceToRemove()
    {
        if (walkers.Count <= 1) return;

        for (int i = 0; i < walkers.Count; i++)
        {
            if (Random.value < walkers[i].ChanceToChange)
            {
                walkers.RemoveAt(i);
                break;
            }
        }
    }

    Vector2 GetRandomDirection()
    {
        return Random.Range(0, 4) switch
        {
            0 => Vector2.up,
            1 => Vector2.down,
            2 => Vector2.left,
            _ => Vector2.right,
        };
    }

    void SpawnPlayer()
    {
        if (playerSpawnPrefab == null)
        {
            Debug.LogWarning("playerSpawnPrefab chưa được gán!");
            ShowLoadingScreen(false);
            return;
        }

        Vector2Int closest = Vector2Int.zero;
        float minDist = float.MaxValue;

        foreach (var pos in grid.Keys)
        {
            if (grid[pos] == GridType.FLOOR)
            {
                float dist = pos.sqrMagnitude;
                if (dist < minDist)
                {
                    closest = pos;
                    minDist = dist;
                }
            }
        }

        playerSpawnPos = closest;
        Vector3 worldPos = floorTilemap.CellToWorld(new Vector3Int(closest.x, closest.y, 0)) + new Vector3(0.5f, 0.5f, -5);
        Instantiate(playerSpawnPrefab, worldPos, Quaternion.identity);
    }

    void SpawnObjects()
    {
        HashSet<Vector2Int> usedPositions = new();
        List<Vector2Int> floorPositions = new();

        foreach (var pair in grid)
        {
            if (pair.Value == GridType.FLOOR)
            {
                floorPositions.Add(pair.Key);
            }
        }

        Shuffle(floorPositions);

        int index = 0;

        PlaceShop(usedPositions);

        bool IsNearRestrictedArea(Vector2Int pos)
        {
            return Vector2Int.Distance(pos, playerSpawnPos) <= 4f || Vector2Int.Distance(pos, shopPos) <= 4f;
        }

        for (int i = 0; i < destructibleClusterCount && index < floorPositions.Count; i++)
        {
            int clusterWidth = 2;
            int clusterHeight = Random.value < 0.5f ? 2 : 1;

            Vector2Int basePos = floorPositions[index];
            index++;

            bool canPlace = true;
            for (int dx = 0; dx < clusterWidth && canPlace; dx++)
            {
                for (int dy = 0; dy < clusterHeight && canPlace; dy++)
                {
                    Vector2Int pos = new Vector2Int(basePos.x + dx, basePos.y + dy);
                    if (!grid.ContainsKey(pos) || grid[pos] != GridType.FLOOR || usedPositions.Contains(pos) || IsNearRestrictedArea(pos) || cosmeticPositions.Contains(pos))
                        canPlace = false;
                }
            }

            if (!canPlace) continue;

            for (int dx = 0; dx < clusterWidth; dx++)
            {
                for (int dy = 0; dy < clusterHeight; dy++)
                {
                    Vector2Int pos = new Vector2Int(basePos.x + dx, basePos.y + dy);
                    usedPositions.Add(pos);
                    GameObject prefab = destructiblePrefabs[Random.Range(0, destructiblePrefabs.Length)];
                    Instantiate(prefab, new Vector3(pos.x, pos.y, 5f), Quaternion.identity);
                }
            }
        }

        for (int i = 0; i < treeCount && index < floorPositions.Count; i++, index++)
        {
            Vector2Int pos = floorPositions[index];
            if (usedPositions.Contains(pos) || IsNearRestrictedArea(pos) || cosmeticPositions.Contains(pos)) continue;
            usedPositions.Add(pos);
            GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
            Instantiate(prefab, new Vector3(pos.x, pos.y, 6f), Quaternion.identity);
        }

        for (int i = 0; i < enemyCount && index < floorPositions.Count; i++, index++)
        {
            Vector2Int pos = floorPositions[index];
            if (usedPositions.Contains(pos) || IsNearRestrictedArea(pos) || cosmeticPositions.Contains(pos)) continue;
            usedPositions.Add(pos);
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(prefab, new Vector3(pos.x, pos.y, 5f), Quaternion.identity);
        }
    }

    void PlaceShop(HashSet<Vector2Int> usedPositions)
    {
        List<Vector2Int> candidates = new();

        foreach (var pair in grid)
        {
            Vector2Int pos = pair.Key;

            if (grid[pos] != GridType.FLOOR || usedPositions.Contains(pos) || cosmeticPositions.Contains(pos)) continue;

            if (Vector2Int.Distance(pos, playerSpawnPos) < 10f) continue;

            bool areaClear = true;
            for (int dx = -4; dx <= 4 && areaClear; dx++)
            {
                for (int dy = -4; dy <= 4 && areaClear; dy++)
                {
                    Vector2Int check = new(pos.x + dx, pos.y + dy);
                    if (!grid.ContainsKey(check) || grid[check] != GridType.FLOOR || usedPositions.Contains(check) || cosmeticPositions.Contains(check))
                    {
                        areaClear = false;
                    }
                }
            }

            if (areaClear)
            {
                candidates.Add(pos);
            }
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning("Không tìm thấy khu vực trống để đặt Shop.");
            return;
        }

        Vector2Int center = candidates[Random.Range(0, candidates.Count)];
        shopPos = center;

        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                usedPositions.Add(new Vector2Int(center.x + dx, center.y + dy));
            }
        }

        Vector3 worldPos = new Vector3(center.x + 0.5f, center.y + 0.5f, 5f);
        Instantiate(shopPrefab, worldPos, Quaternion.identity);
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    void ShowLoadingScreen(bool show)
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(show);
        }
    }

    public struct WalkerObject
    {
        public Vector2 Position;
        public Vector2 Direction;
        public float ChanceToChange;

        public WalkerObject(Vector2 pos, Vector2 dir, float change)
        {
            Position = pos;
            Direction = dir;
            ChanceToChange = change;
        }
    }
}
