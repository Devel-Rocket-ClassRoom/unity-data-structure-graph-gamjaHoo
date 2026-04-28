using System.Linq;
using UnityEngine;

public enum TileTypes
{
    Empty = -1,
    // Coast 0 ~ 14,
    Grass = 15,
    Tree,
    Hills,
    Mountains,
    Towns,
    Castle,
    Monster
}

public class Map
{
    public int Rows = 0;
    public int Columns = 0;

    public Tile[] Tiles;

    public Tile[] CoastTiles => Tiles.Where(t => t.AutoTileId >= 0 && t.AutoTileId < (int)TileTypes.Grass).ToArray();
    public Tile[] LandTiles => Tiles.Where(t => t.AutoTileId == (int)TileTypes.Grass).ToArray();

    public Tile startTile;
    public Tile castleTile;

    public void Init(int rows, int cols)
    {
        Rows = rows;
        Columns = cols;

        Tiles = new Tile[rows * cols];
        for (int i = 0; i < Tiles.Length; i++)
        {
            Tiles[i] = new();
            Tiles[i].Id = i;
        }

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                int index = r * Columns + c;
                var adgacents = Tiles[index].Adjacents;
                if (r - 1 >= 0)
                {
                    //if(index - Columns >= 0 )
                    adgacents[(int)Sides.Top] = Tiles[index - Columns]; // Up
                }

                if (c + 1 < Columns)
                {
                    //if(index+1 <=  Columns)
                    adgacents[(int)Sides.Right] = Tiles[index + 1]; // Right
                }

                if (c - 1 >= 0)
                {
                    //if(index - 1 >= 0)
                    adgacents[(int)Sides.Left] = Tiles[index - 1]; // Left
                }

                if (r + 1 < Rows)
                {
                    //if(index + Columns <= Rows * Columns)
                    adgacents[(int)Sides.Bottom] = Tiles[index + Columns]; // Down
                }
            }
        }

        for (int i = 0; i < Tiles.Length; i++)
        {
            Tiles[i].UpdateAutoTileId();
        }
    }

    public void ShuffleTiles(Tile[] tiles)
    {
        for (int i = tiles.Length - 1; i > 0; i--)
        {
            int rdmIdx = Random.Range(0, i + 1);
            (tiles[rdmIdx], tiles[i]) = (tiles[i], tiles[rdmIdx]);
        }
    }

    public void DecorateTiles(Tile[] tiles, float percent, TileTypes tileType)
    {
        ShuffleTiles(tiles);
        int total = Mathf.FloorToInt(tiles.Length * percent);
        for (int i = 0; i < total; i++)
        {
            if (tileType == TileTypes.Empty)
            {
                tiles[i].ClearAdjacents();
            }

            tiles[i].AutoTileId = (int)tileType;
        }
    }

    public bool CreateIsland(
        float erodePercent,
        int erodeIterations,
        float lakePercent,
        float treePercent,
        float hillPercent,
        float mountainPercent,
        float townPercent,
        float monsterPercent) // Castle 1개만
    {

        DecorateTiles(LandTiles, lakePercent, TileTypes.Empty);
        for (int i = 0; i < erodeIterations; i++)
        {
            DecorateTiles(CoastTiles, erodePercent, TileTypes.Empty);
        }

        DecorateTiles(LandTiles, treePercent, TileTypes.Tree);
        DecorateTiles(LandTiles, hillPercent, TileTypes.Hills);
        DecorateTiles(LandTiles, mountainPercent, TileTypes.Mountains);
        DecorateTiles(LandTiles, townPercent, TileTypes.Towns);
        DecorateTiles(LandTiles, monsterPercent, TileTypes.Monster);

        var towns = Tiles.Where(x => x.AutoTileId == (int)TileTypes.Towns).ToArray();
        ShuffleTiles(towns);
        castleTile = towns[0];
        startTile = towns[1];
        towns[0].AutoTileId = (int)TileTypes.Castle;

        return true;
    }
}