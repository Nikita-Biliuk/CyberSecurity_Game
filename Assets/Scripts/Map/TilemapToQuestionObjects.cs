using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapToQuestionObjects : MonoBehaviour
{
    public Tilemap sourceTilemap;
    public GameObject questionPrefab;

    [ContextMenu("Convert Tilemap To Question Objects")]
    void Convert()
    {
        if (sourceTilemap == null || questionPrefab == null)
        {
            Debug.LogError("Source Tilemap or Question Prefab is not assigned.");
            return;
        }

        foreach (var pos in sourceTilemap.cellBounds.allPositionsWithin)
        {
            if (sourceTilemap.HasTile(pos))
            {
                Vector3 worldPos = sourceTilemap.CellToWorld(pos) + sourceTilemap.tileAnchor;
                Instantiate(questionPrefab, worldPos, Quaternion.identity, transform);
            }
        }

        Debug.Log("Convertation complete.");
    }
}
