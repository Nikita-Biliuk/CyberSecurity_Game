using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    private class Node
    {
        public Vector2 pos;
        public float g;
        public float f;
        public Node parent;
        public Node(Vector2 p) { pos = p; g = float.PositiveInfinity; f = float.PositiveInfinity; parent = null; }
    }

    // A* pathfinding from startWorld to goalWorld
    public static List<Vector2> FindPath(Vector2 startWorld, Vector2 goalWorld)
    {
        if (GridMap.Instance == null) return new List<Vector2>();

        Vector2 start = GridMap.Instance.WorldToCellCenter(startWorld);
        Vector2 goal = GridMap.Instance.WorldToCellCenter(goalWorld);

        if (!GridMap.Instance.IsWalkable(start)) 
        {
            // find nearest walkable
            start = FindNearbyWalkable(start);
            if (start == Vector2.positiveInfinity) return new List<Vector2>();
        }
        if (!GridMap.Instance.IsWalkable(goal))
        {
            goal = FindNearbyWalkable(goal);
            if (goal == Vector2.positiveInfinity) return new List<Vector2>();
        }

        Dictionary<Vector2, Node> all = new Dictionary<Vector2, Node>();
        List<Node> open = new List<Node>();
        Node startNode = new Node(start) { g = 0f, f = Heuristic(start, goal) };
        all[start] = startNode;
        open.Add(startNode);

        while (open.Count > 0)
        {
            // get node in open with lowest f
            Node current = open[0];
            for (int i = 1; i < open.Count; i++)
                if (open[i].f < current.f) current = open[i];

            if (Vector2.Distance(current.pos, goal) <= GridMap.Instance.cellSize * 0.1f)
                return Reconstruct(current);

            open.Remove(current);

            foreach (Vector2 nbrPos in GetNeighbors(current.pos))
            {
                if (!all.TryGetValue(nbrPos, out Node nbr))
                {
                    nbr = new Node(nbrPos);
                    all[nbrPos] = nbr;
                }

                float tentativeG = current.g + Vector2.Distance(current.pos, nbr.pos);
                if (tentativeG < nbr.g)
                {
                    nbr.parent = current;
                    nbr.g = tentativeG;
                    nbr.f = tentativeG + Heuristic(nbr.pos, goal);
                    if (!open.Contains(nbr)) open.Add(nbr);
                }
            }
        }

        return new List<Vector2>();
    }

    private static List<Vector2> Reconstruct(Node node)
    {
        List<Vector2> path = new List<Vector2>();
        Node cur = node;
        while (cur != null)
        {
            path.Add(cur.pos);
            cur = cur.parent;
        }
        path.Reverse();
        path = SimplifyPath(path);
        return path;
    }

    // Delete collinear points
    private static List<Vector2> SimplifyPath(List<Vector2> raw)
    {
        if (raw.Count < 3) return raw;
        List<Vector2> outp = new List<Vector2> { raw[0] };
        for (int i = 1; i < raw.Count - 1; i++)
        {
            Vector2 a = outp[outp.Count - 1];
            Vector2 b = raw[i];
            Vector2 c = raw[i + 1];
            if (!Collinear(a, b, c)) outp.Add(b);
        }
        outp.Add(raw[raw.Count - 1]);
        return outp;
    }

    private static bool Collinear(Vector2 a, Vector2 b, Vector2 c)
    {
        // Area of triangle abc is zero
        float area = Mathf.Abs((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x));
        return area < 0.001f;
    }

    private static IEnumerable<Vector2> GetNeighbors(Vector2 center)
    {
        float s = GridMap.Instance.cellSize;
        Vector2[] dirs = new Vector2[]
        {
            new Vector2( s, 0),
            new Vector2(-s, 0),
            new Vector2(0,  s),
            new Vector2(0, -s),
            new Vector2( s, s),
            new Vector2(-s, s),
            new Vector2( s,-s),
            new Vector2(-s,-s)
        };

        foreach (var d in dirs)
        {
            Vector2 pos = center + d;
            if (GridMap.Instance.IsWalkable(pos))
                yield return pos;
        }
    }

    private static float Heuristic(Vector2 a, Vector2 b)
    {
        return Vector2.Distance(a, b); 
    }

    // Find nearest walkable cell around origin
    private static Vector2 FindNearbyWalkable(Vector2 origin)
    {
        float s = GridMap.Instance.cellSize;
        int maxSteps = 6; // search radius
        for (int r = 1; r <= maxSteps; r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    if (Mathf.Abs(dx) != r && Mathf.Abs(dy) != r) continue; // only perimeter
                    Vector2 candidate = origin + new Vector2(dx * s, dy * s);
                    if (GridMap.Instance.IsWalkable(candidate)) return candidate;
                }
            }
        }
        return Vector2.positiveInfinity;
    }
}
