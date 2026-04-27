using UnityEngine;

public class Graph // 그리드 배열로 표현
{
    public int rows = 0;
    public int cols = 0;

    public GraphNode[] nodes;
    
    public void Init(int[,] grid)
    {
        rows = grid.GetLength(0);
        cols = grid.GetLength(1);

        nodes = new GraphNode[grid.Length];
        for(int i = 0; i < nodes.Length; ++i)
        {
            nodes[i] = new GraphNode();
            nodes[i].id = i;
        }

        for(int r = 0; r < rows; ++r)
        {
            for(int c = 0; c < cols; ++c)
            {
                int index = r * cols + c;
                nodes[index].weight = grid[r, c];

                if (grid[r,c] == -1)
                {
                    continue; // 방문 불가 노드 (인접 X)
                }

                if(r - 1 >= 0 && grid[r - 1, c] >= 0) // 왼쪽은 안에 있는지 검사, 오른쪽은 방문 가능한지 검사
                {
                    nodes[index].adjacents.Add(nodes[index - cols]); // up
                }
                if (c + 1 < cols && grid[r, c + 1] >= 0)
                {
                    nodes[index].adjacents.Add(nodes[index + 1]); // right
                }
                if(r + 1 < rows && grid[r + 1, c] >= 0)
                {
                    nodes[index].adjacents.Add(nodes[index + cols]); // down
                }
                if(c - 1 >= 0 && grid[r, c - 1] >= 0)
                {
                    nodes[index].adjacents.Add(nodes[index - 1]); // left
                }
            }
        }
    }

    public void ResetNodePrevious()
    {
        foreach(var node in nodes)
        {
            node.previous = null;
        }
    }
}
