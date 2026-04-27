using System.Collections.Generic;
using UnityEngine;

public class GraphNode
{
    public int id;
    public int weight = 1; // weight가 0이면 장애물, -1이면 방문 불가 노드로 간주 (경로 탐색에서 제외)

    public GraphNode previous = null; // 이전 노드 정보 (경로 추적용)

    public List<GraphNode> adjacents = new List<GraphNode>();

    public bool CanVisit => adjacents.Count > 0 && weight > 0; 
}
