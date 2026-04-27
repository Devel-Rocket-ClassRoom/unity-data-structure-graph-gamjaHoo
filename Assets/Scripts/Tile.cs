using UnityEngine;

public enum Sides
{
    Bottom,     // 3
    Right,      // 2
    Left,       // 1
    Top,        // 0
}
public class Tile
{
    public int id; // node에서 사용한 2차원 배열 1차원으로 만든거
    public Tile[] adjacents = new Tile[4]; // 0: bottom, 1: right, 2: left, 3: top

    public int autoTileId;

    public bool isVisited = false;

    public void UpdateAutoTileId()
    {
        autoTileId = 0;
        for (int i = 0; i < adjacents.Length; ++i)
        {
            if (adjacents[i] != null)
            {
                autoTileId |= 1 << adjacents.Length - 1 - i; // 2^(3-i) = 1 << (3-i)
                // autoTileId += (int)Mathf.Pow(2, 3-i);
            }
        }
    }
}
