using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject TilePrefab;

    private GameObject[] _tileObjects;

    public int MapWidth = 20;
    public int MapHeight = 20;

    public int SearchRange = 3;

    [Range(0f, 0.9f)] public float ErodePercent = 0.5f;
    [Min(2)] public int ErodeIterations = 2;
    [Range(0f, 0.1f)] public float LakePercent = 0.2f;
    [Range(0f, 0.3f)] public float TreePercent = 0.2f;
    [Range(0f, 0.2f)] public float HillPercent = 0.2f;
    [Range(0f, 0.1f)] public float MountainPercent = 0.2f;
    [Range(0f, 0.1f)] public float TownPercent = 0.2f;
    [Range(0f, 0.1f)] public float MonsterPercent = 0.2f;

    public Vector2 TileSize = new(16, 16);
    public Sprite[] IslandSprites;
    public Sprite[] FowSprites;

    private Map _map;
    private Camera mainCamera;

    public PlayerMovement playerPrefab;
    private PlayerMovement player;

    private int prevTileId = -1;

    public Map Map => _map;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Debug.Log("space");
            ResetStage();
        }
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 screenPos = Input.mousePosition;
            Debug.Log(ScreenPosToTileId(screenPos));
            Debug.Log(GetTilePos(ScreenPosToTileId(screenPos)));
        }
        if(_tileObjects != null)
        {
            int currentTileId = ScreenPosToTileId(Input.mousePosition);
            if(prevTileId != currentTileId)
            {
                if (currentTileId >= 0 && currentTileId < MapWidth * MapHeight) 
                {
                    _tileObjects[currentTileId].GetComponent<SpriteRenderer>().color = Color.green;
                }
                if(prevTileId >= 0 && prevTileId < _tileObjects.Length)
                {
                    _tileObjects[prevTileId].GetComponent<SpriteRenderer>().color = Color.white;
                }
                prevTileId = currentTileId;
            }
        }
        UpdateFow();
    }

    private void ResetStage()
    {
        //Debug.Log("resetstage");
        _map = new();
        _map.Init(MapHeight, MapWidth);
        _map.CreateIsland(
            ErodePercent,
            ErodeIterations,
            LakePercent,
            TreePercent,
            HillPercent,
            MountainPercent,
            TownPercent,
            MonsterPercent
        );
        CreateGrid();
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        if(player != null)
        {
            Destroy(player.gameObject);
        }
        player = Instantiate(playerPrefab);
        player.MoveTo(_map.startTile.Id);
    }

    private void CreateGrid()
    {
        //Debug.Log("creategrid");
        if (_tileObjects != null)
        {
            foreach (var tile in _tileObjects)
            {
                Destroy(tile.gameObject);
            }
        }

        _tileObjects = new GameObject[MapWidth * MapHeight];

        var position = Vector3.zero;
        position.x -= (MapWidth * TileSize.x) / 2 - TileSize.x / 2;
        position.y += (MapHeight * TileSize.y) / 2 - TileSize.y / 2;
        for (int i = 0; i < MapHeight; i++)
        {
            for (int j = 0; j < MapWidth; j++)
            {
                var tileId = i * MapWidth + j;
                var newGO = Instantiate(TilePrefab, transform);
                newGO.transform.position = position;
                position.x += TileSize.x;

                _tileObjects[tileId] = newGO;
                DecorateTile(tileId);
            }
            position.x = -((MapWidth * TileSize.x) / 2 - TileSize.x / 2);
            position.y -= TileSize.y;
        }
    }
    private void UpdateFow()
    {
        if (player == null) return;
        int half = SearchRange / 2;
        for(int i = 0; i < SearchRange; i++)
        {
            for(int j = 0; j < SearchRange; j++)
            {
                var tileId = (player.currentTileId - (half + half * MapWidth)) + i * MapWidth + j;
                if (tileId >= 0 && tileId < MapWidth * MapHeight && _map.Tiles[tileId] != null)
                {
                    _map.Tiles[tileId].IsVisited = true;
                    foreach(var tile in _map.Tiles[tileId].Adjacents)
                    {
                        if(tile != null)
                        {
                            tile.UpdateFowTileId();
                            DecorateTile(tile.Id);
                        }
                    }
                    DecorateTile(tileId);
                }
            }
        }
    }
    public void DecorateTile(int tileId)
    {
        var tile = _map.Tiles[tileId];
        var tileGO = _tileObjects[tileId];
        var renderer = tileGO.GetComponent<SpriteRenderer>();
        if (tile.AutoTileId != (int)TileTypes.Empty)
        {
            renderer.sprite = IslandSprites[tile.AutoTileId];
        }
        else
        {
            renderer.sprite = null;
        }
        if (!tile.IsVisited)
        {
            renderer.sprite = FowSprites[tile.FowTileId];
        }
    }

    public int ScreenPosToTileId(Vector3 screenPos)
    {
        screenPos.z = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        
        return WorldPosToTileId(worldPos);
    }
    public int WorldPosToTileId(Vector3 worldPos)
    {
        var x = worldPos.x + (MapWidth * TileSize.x) / 2;
        var y = worldPos.y - (MapHeight * TileSize.y) / 2;
        var col = Mathf.FloorToInt(x / TileSize.x);
        var row = Mathf.FloorToInt(-y / TileSize.y);
        if (col < 0 || col >= MapWidth || row < 0 || row >= MapHeight)
        {
            return -1;
        }
        return row * MapWidth + col;
    }

    public Vector3 GetTilePos(int y, int x)
    {
        var pos = Vector3.zero;
        pos.x = x * TileSize.x - (MapWidth * TileSize.x) / 2 + TileSize.x / 2;
        pos.y = -(y * TileSize.y - (MapHeight * TileSize.y) / 2 + TileSize.y / 2);
        return pos;
    }

    public Vector3 GetTilePos(int tileId)
    {
        var y = tileId / MapWidth;
        var x = tileId % MapWidth;
        return GetTilePos(y, x);
    }
}