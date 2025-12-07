using UnityEngine;

public class GridMap : MonoBehaviour
{
    public static GridMap Instance;

    [Header("Grid")]
    public float cellSize = 0.5f;

    [Header("Obstacles")]
    public LayerMask obstacleMask;
    public float obstacleCheckRadiusFactor = 0.4f; // fraction of cellSize

    void Awake()
    {
        Instance = this;
    }

    // Checks if the cell at worldPos is walkable (not blocked by obstacles)
    public bool IsWalkable(Vector2 worldPos)
    {
        float r = cellSize * obstacleCheckRadiusFactor;
        bool blocked = Physics2D.OverlapCircle(worldPos, r, obstacleMask);
        return !blocked;
    }

    // Converts world position to the center of the nearest grid cell
    public Vector2 WorldToCellCenter(Vector2 worldPos)
    {
        float x = Mathf.Round(worldPos.x / cellSize) * cellSize;
        float y = Mathf.Round(worldPos.y / cellSize) * cellSize;
        return new Vector2(x, y);
    }
}
