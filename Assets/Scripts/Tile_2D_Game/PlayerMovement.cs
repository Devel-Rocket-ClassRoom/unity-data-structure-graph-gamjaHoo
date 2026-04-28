using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Stage stage;
    private Map map;
    private Animator animator;

    public int currentTileId;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0f;

        var findGo = GameObject.FindWithTag("Map");
        stage = findGo.GetComponent<Stage>();
        map = stage.Map;
    }
    void Update()
    {
        int h = 0;
        int v = 0;
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            h = 1;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            h = -1;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            v = 1;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            v = -1;
        }


        var direction = Sides.None;
        if( v == 1)
        {
            direction = Sides.Top;
        }
        else if (v == -1)
        {
            direction = Sides.Bottom;
        }
        else if(h == 1)
        {
            direction = Sides.Right;
        }
        else if(h == -1)
        {
            direction = Sides.Left;
        }

        if(direction != Sides.None)
        {
            var targetTile = stage.Map.Tiles[currentTileId].Adjacents[(int)direction];
            if(targetTile != null && targetTile.CanMove)
            {
                MoveTo(targetTile.Id);
            }
        }
        
    }

    public void MoveTo(int tileId)
    {
        currentTileId = tileId;

        transform.position = stage.GetTilePos(currentTileId);
    }
}
