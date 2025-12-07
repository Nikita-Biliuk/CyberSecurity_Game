using UnityEngine;

public class GridMap : MonoBehaviour
{
    public static GridMap Instance;

    [Header("Grid")]
    public float cellSize = 0.5f;

    [Header("Obstacles")]
    public LayerMask obstacleMask;
    public float obstacleCheckRadiusFactor = 0.4f; // радиус = cellSize * factor

    void Awake()
    {
        Instance = this;
    }

    // Проверяет, можно ли встать в этой точке (центр клетки).
    public bool IsWalkable(Vector2 worldPos)
    {
        float r = cellSize * obstacleCheckRadiusFactor;
        bool blocked = Physics2D.OverlapCircle(worldPos, r, obstacleMask);
        return !blocked;
    }

    // Возвращает центр ближайшей ячейки для worldPos
    public Vector2 WorldToCellCenter(Vector2 worldPos)
    {
        float x = Mathf.Round(worldPos.x / cellSize) * cellSize;
        float y = Mathf.Round(worldPos.y / cellSize) * cellSize;
        return new Vector2(x, y);
    }
}
