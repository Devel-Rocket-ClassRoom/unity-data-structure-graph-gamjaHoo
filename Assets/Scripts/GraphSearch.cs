using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GraphSearch
{
    private Graph graph;
    public List<GraphNode> path = new List<GraphNode>();
    //private int[] Distances;
    public void Init(Graph graph)
    {
        this.graph = graph;
    }

    public void DFS(GraphNode node)
    {
        path.Clear();

        var visited = new HashSet<GraphNode>(); // HashSet은 키, 밸류가 아니라 키만 있음
        var stack = new Stack<GraphNode>();

        stack.Push(node);
        visited.Add(node);

        while(stack.Count > 0)
        {
            var currentNode = stack.Pop();
            path.Add(currentNode);

            foreach(var adjacent in currentNode.adjacents)
            {
                if(!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }
                visited.Add(adjacent);
                stack.Push(adjacent);
            }
        }
    }

    public void BFS(GraphNode node)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();
        queue.Enqueue(node);
        visited.Add(node);
        while(queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            path.Add(currentNode);
            foreach(var adjacent in currentNode.adjacents)
            {
                if(!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }
                visited.Add(adjacent);
                queue.Enqueue(adjacent);
            }
        }
    }

    public void DFSRecursive(GraphNode node, int depth = 0)
    {
        if(depth == 0)
            path.Clear();

        path.Add(node);
        foreach (var adjacent in node.adjacents)
        {
            if (!adjacent.CanVisit || path.Contains(adjacent))
            {
                continue;
            }
            DFSRecursive(adjacent, depth + 1);
        }
    }


    public bool PathFindingBFS(GraphNode start, GraphNode end)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();
        queue.Enqueue(start);
        visited.Add(start);

        bool success = false;
        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            if(currentNode == end)
            {
                // 경로 추적
                var temp = currentNode;
                while(temp != null)
                {
                    path.Add(temp);

                    temp = temp.previous;
                }
                path.Reverse(); // 역순으로 저장된 경로를 올바른 순서로 변경
                success = true;
                break;
            }
            foreach(var adjacent in currentNode.adjacents)
            {
                if(!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }
                visited.Add(adjacent);
                adjacent.previous = currentNode; // 이전 노드 정보 저장
                queue.Enqueue(adjacent);
            }
        }
        return success;
    }

    public void Dijkstra(GraphNode start, GraphNode end)
    {
        path.Clear();
        graph.ResetNodePrevious();
        var distances = new int[graph.nodes.Length];

        for(int i = 0; i < distances.Length; i++)
        {
            distances[i] = int.MaxValue;
        }
        distances[start.id] = 0;

        PriorityQueue<GraphNode, int> pq = new PriorityQueue<GraphNode, int>();
        pq.Enqueue(start, 0);
        var visited = new HashSet<GraphNode>();

        //visited.Add(start);
        while (pq.Count > 0)
        {
            var u = pq.Dequeue();
            if (visited.Contains(u))
            {
                continue;
            }
            visited.Add(u);
            if(u == end)
            {
                var temp = u;
                while(temp != null)
                {
                    path.Add(temp);
                    temp = temp.previous;
                }
                path.Reverse();
                break;
            }
            foreach(var v in u.adjacents)
            {
                if(!v.CanVisit || visited.Contains(v))
                {
                    continue;
                }
                int newDist = distances[u.id] + v.weight;
                if (v.previous == null || newDist < distances[v.id])
                {
                    distances[v.id] = newDist;
                    v.previous = u;
                    pq.Enqueue(v, newDist);
                }
            }
        }
    }
    public void Astar(GraphNode start, GraphNode end)
    {
        path.Clear();
        graph.ResetNodePrevious();
        var distances = new int[graph.nodes.Length];

        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = int.MaxValue;
        }
        distances[start.id] = 0;

        PriorityQueue<GraphNode, int> pq = new PriorityQueue<GraphNode, int>();
        pq.Enqueue(start, 0 + Heuristic(start, end));
        var visited = new HashSet<GraphNode>();

        //visited.Add(start);
        while (pq.Count > 0)
        {
            var u = pq.Dequeue();
            if (visited.Contains(u))
            {
                continue;
            }
            visited.Add(u);
            if (u == end)
            {
                var temp = u;
                while (temp != null)
                {
                    path.Add(temp);
                    temp = temp.previous;
                }
                path.Reverse();
                break;
            }
            foreach (var v in u.adjacents)
            {
                if (!v.CanVisit || visited.Contains(v))
                {
                    continue;
                }
                int newDist = distances[u.id] + v.weight;
                if (v.previous == null || newDist < distances[v.id])
                {
                    distances[v.id] = newDist;
                    v.previous = u;
                    pq.Enqueue(v, newDist + Heuristic(v, end));
                }
            }
        }
    }
    private int Heuristic(GraphNode a, GraphNode b)
    {
        int ax = a.id % graph.cols;
        int ay = a.id / graph.cols;

        int bx = b.id % graph.cols;
        int by = b.id / graph.cols;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }
}
