using UnityEngine;

public enum TileTypes
{
    Empty = -1,
    // 0~14 -> 해안선타일
    Grass = 15,
    Tree,
    Hills,
    Mountains,
    Towns,
    Castle,
    Monster,
}
public class Map
{
    public int rows = 0;
    public int cols = 0;

    public Tile[] tiles;

    public void Init(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        tiles = new Tile[rows * cols];
        for (int i = 0; i < tiles.Length; ++i)
        {
            tiles[i] = new Tile();
            tiles[i].id = i;
        }

        for(int r = 0; r < rows; ++r)
        {
            for(int c = 0; c < cols; ++c)
            {
                int id = r * cols + c;
                if (r > 0) tiles[id].adjacents[(int)Sides.Top] = tiles[(r - 1) * cols + c];
                if (r < rows - 1) tiles[id].adjacents[(int)Sides.Bottom] = tiles[(r + 1) * cols + c];
                if (c > 0) tiles[id].adjacents[(int)Sides.Left] = tiles[r * cols + (c - 1)];
                if (c < cols - 1) tiles[id].adjacents[(int)Sides.Right] = tiles[r * cols + (c + 1)];
            }
        }

        for( int i = 0; i < tiles.Length; ++i)
        {
            tiles[i].UpdateAutoTileId();
        }
    }
}
