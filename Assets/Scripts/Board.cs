using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    //Attach to 
    [SerializeField] int[,] board;
    [SerializeField] Transform[] blockTr;
    [SerializeField] Transform cameraTr;

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
                b.iD = randomBlock;
            }
        }
        cameraTr.transform.position = new Vector3((sizeX * 0.5f - 0.5f), //first 0.5ff - halfleght, second halfBlock because center of 1st block on (0,0,0)
                                                    sizeY * 0.5f - 0.5f,
                                                     -sizeY); //size y because screen 16:9 (height more important height)
    }
    //spawn additional block if someone was destroyed;
    //create pool of block 
    // if on start spawn block make match3. destroy them and respawn. or make some spawn algorytm

    private void Update()
    {
        if (Block.first && Block.second)
        {
            if (CheckIfNeibor()) 
            {
                Swap();
            }

            else
            {
                Block.first = null;
                Block.second = null;
            }

        }
    }

    private bool CheckIfNeibor()
    {
        Block _first = Block.first.gameObject.GetComponent<Block>();
        Block _second = Block.second.gameObject.GetComponent<Block>();

        //test
        if (_first.x - 1 == _second.x && _first.y == _second.y)
            return true;
        else if (_first.x + 1 == _second.x && _first.y == _second.y)
            return true;
        else if (_first.x == _second.x && _first.y + 1 == _second.y)
            return true;
        else if (_first.x == _second.x && _first.y - 1 == _second.y)
            return true;
        Debug.Log("Picked not neibor");
        return false;

    }

    private void Swap()
    {
        Block _first = Block.first.gameObject.GetComponent<Block>();
        Block _second = Block.second.gameObject.GetComponent<Block>();

        Vector3 bootPos = _first.transform.position;
        int bootX = _first.x;
        int bootY = _first.y;

        _first.transform.position = _second.transform.position;
        _second.transform.position = bootPos;


        //swap data
        _first.x = _second.x;
        _first.y = _second.y;

        _second.x = bootX;
        _second.y = bootX;

        board[_first.x, _first.y] = _first.iD;
        board[_second.x, _second.y] = _second.iD;
    }
}
