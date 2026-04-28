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
                    StopAllCoroutines();
                    _isMoving = false;
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

    private IEnumerator WalkPath(List<int> path)
    {
        _isMoving = true;
        if (animator != null) animator.speed = 1f;

        foreach (int tileId in path)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = stage.GetTilePos(tileId);

            float duration = 1f / moveSpeed;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                yield return null;
            }
            transform.position = endPos;
            currentTileId = tileId;
        }

        if (animator != null) animator.speed = 0f;
        _isMoving = false;
    }

    private List<int> FindPath(int fromId, int toId) // BFS
    {
        if (fromId == toId) return null;

        var visited = new HashSet<int> { fromId };
        var parent = new Dictionary<int, int>();
        var queue = new Queue<int>();
        queue.Enqueue(fromId);

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();
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

            foreach (var neighbor in stage.Map.Tiles[current].Adjacents)
            {
                if (neighbor == null || visited.Contains(neighbor.Id)) continue;
                if (!neighbor.CanMove || !neighbor.IsVisited) continue;
                visited.Add(neighbor.Id);
                parent[neighbor.Id] = current;
                queue.Enqueue(neighbor.Id);
            }
        }

        return null;
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
