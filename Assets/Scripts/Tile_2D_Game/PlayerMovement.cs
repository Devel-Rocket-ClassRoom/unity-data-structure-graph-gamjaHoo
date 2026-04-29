using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Stage stage;
    private Animator animator;

    public int currentTileId;
    public float moveSpeed = 5f;

    private bool _isMoving = false;
    private List<int> _pendingPath = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator != null) animator.speed = 0f;

        var findGo = GameObject.FindWithTag("Map");
        stage = findGo.GetComponent<Stage>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            int targetId = stage.ScreenPosToTileId(Input.mousePosition);
            var tiles = stage.Map.Tiles;
            if (targetId >= 0 && tiles[targetId].IsVisited && tiles[targetId].CanMove)
            {
                var path = FindPath(currentTileId, targetId);
                if (path != null && path.Count > 0)
                {
                    if (_isMoving)
                        _pendingPath = path;
                    else
                        StartCoroutine(WalkPath(path));
                }
            }
        }

        if (!_isMoving)
        {
            int h = 0, v = 0;
            if (Input.GetKeyDown(KeyCode.RightArrow)) h = 1;
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) h = -1;
            else if (Input.GetKeyDown(KeyCode.UpArrow)) v = 1;
            else if (Input.GetKeyDown(KeyCode.DownArrow)) v = -1;

            var direction = Sides.None;
            if (v == 1) direction = Sides.Top;
            else if (v == -1) direction = Sides.Bottom;
            else if (h == 1) direction = Sides.Right;
            else if (h == -1) direction = Sides.Left;

            if (direction != Sides.None)
            {
                var targetTile = stage.Map.Tiles[currentTileId].Adjacents[(int)direction];
                if (targetTile != null && targetTile.CanMove && targetTile.IsVisited)
                    StartCoroutine(WalkPath(new List<int> { targetTile.Id }));
            }
        }
    }
    private void DrawPath(List<int> path)
    {
        var lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null) return;
        lineRenderer.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            lineRenderer.SetPosition(i, stage.GetTilePos(path[i]) + Vector3.up * 0.1f);
        }
    }
    private IEnumerator WalkPath(List<int> path)
    {
        _isMoving = true;
        if (animator != null) animator.speed = 1f;

        int index = 0;
        while (index < path.Count)
        {
            //DrawPath(path);
            currentTileId = path[index];
            Vector3 startPos = transform.position;
            Vector3 endPos = stage.GetTilePos(path[index]);

            float duration = 1f / moveSpeed;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                yield return null;
            }
            transform.position = endPos;

            if (_pendingPath != null)
            {
                path = _pendingPath;
                _pendingPath = null;
                index = 0;
            }
            else
            {
                index++;
            }
        }

        if (animator != null) animator.speed = 0f;
        _isMoving = false;
    }

    private List<int> FindPath(int fromId, int toId)
    {
        if (fromId == toId) return null;

        var tiles = stage.Map.Tiles;
        int mapWidth = stage.MapWidth;

        var dist = new Dictionary<int, int>();
        var parent = new Dictionary<int, int>();
        var pq = new PriorityQueue<int, int>();

        dist[fromId] = 0;
        pq.Enqueue(fromId, Heuristic(fromId, toId, mapWidth));

        var visited = new HashSet<int>();

        while (pq.Count > 0)
        {
            int current = pq.Dequeue();
            if (visited.Contains(current)) continue;
            visited.Add(current);

            if (current == toId)
            {
                var path = new List<int>();
                int node = toId;
                while (node != fromId)
                {
                    path.Add(node);
                    node = parent[node];
                }
                path.Reverse();
                return path;
            }

            foreach (var neighbor in tiles[current].Adjacents)
            {
                if (neighbor == null || visited.Contains(neighbor.Id)) continue;
                if (!neighbor.CanMove || !neighbor.IsVisited) continue;

                int newDist = dist[current] + neighbor.MoveCost;
                if (!dist.ContainsKey(neighbor.Id) || newDist < dist[neighbor.Id])
                {
                    dist[neighbor.Id] = newDist;
                    parent[neighbor.Id] = current;
                    pq.Enqueue(neighbor.Id, newDist + Heuristic(neighbor.Id, toId, mapWidth));
                }
            }
        }

        return null;
    }

    private int Heuristic(int a, int b, int mapWidth)
    {
        return Mathf.Abs(a % mapWidth - b % mapWidth) + Mathf.Abs(a / mapWidth - b / mapWidth);
    }

    public void MoveTo(int tileId)
    {
        StopAllCoroutines();
        _isMoving = false;
        if (animator != null) animator.speed = 0f;
        currentTileId = tileId;
        transform.position = stage.GetTilePos(currentTileId);
    }
}
