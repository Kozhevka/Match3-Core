using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    //Attach to 
    [SerializeField] int[,] board;
    [SerializeField] Transform[] blockTr;

    private void Start()
    {
        CreateDesk(9, 9);
    }


    public void CreateDesk(int sizeX, int sizeY)
    {
        board = new int[sizeX, sizeY];

        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                int randomBlock = Random.Range(0, blockTr.Length);

                Transform block = (Transform)Instantiate(blockTr[randomBlock].transform, new Vector3(x,y,0f), Quaternion.identity) as Transform;
                block.parent = this.transform;
                block.name = $"block x- {x}, y- {y}. type- {randomBlock}";

                //block pos on board
                board[x, y] = randomBlock;


                Block b = block.gameObject.AddComponent<Block>();
                b.x = x;
                b.y = y;
                b.type = randomBlock;
            }
        }
    }
    //spawn additional block if someone was destroyed;
    //create pool of block 
    // if on start spawn block make match3. destroy them and respawn. or make some spawn algorytm
}
