using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMatch3 : MonoBehaviour
{
    public static BoardMatch3 instance;


    [SerializeField] private int[,] board;
    [SerializeField] private Vector3[,] onBoardPositions;

    [SerializeField] private GameObject cameraObj;

    [SerializeField] private GameObject[] blocksObj;

    public BoxCollider sampleOfBlockCell;
    public GameObject targetLight;
    private float cellHeight;
    private float cellWight;


    private void Awake()
    {
        if (BoardMatch3.instance == null)
            instance = this;
        else if (BoardMatch3.instance != null)
        {
            Debug.LogError("BoardMatch3.instance already exist");
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        cellHeight = sampleOfBlockCell.size.y;
        cellWight = sampleOfBlockCell.size.x;

        CreateDesk(9, 9);
    }


    private void Update()
    {
        if (BlockData.first && BlockData.second) //if exist
        {
            if (CheckDidNeibor())
            {
                Swap();
            }
            else
            {
                UnpickBlocks();
            }
        }
    }

    private void CreateDesk(int sizeX, int sizeY)
    {
        board = new int[sizeX, sizeY];

        onBoardPositions = new Vector3[sizeX, sizeY];

        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                onBoardPositions[x, y] = new Vector3(cellHeight * x, cellWight * y, 0);

                int randomBlock = Random.Range(0, blocksObj.Length);

                GameObject block = Instantiate(blocksObj[randomBlock], onBoardPositions[x, y], Quaternion.identity);

                block.transform.parent = this.transform;

                board[x, y] = randomBlock; //set block identification

                BlockData blockInfo = block.gameObject.AddComponent<BlockData>();
                blockInfo.x = x;
                blockInfo.y = y;
                blockInfo.type = randomBlock;
                
            }
        }
        CameraPosition(sizeX, sizeY);
    }

    private void CameraPosition(int sizeX, int sizeY)
    {
        cameraObj.transform.position = new Vector3((sizeX * (0.5f * cellWight) - (0.5f * cellWight)), //first 0.5ff - halfleght, second halfBlock because center of 1st block on (0,0,0)
                                                    sizeY * (0.5f * cellHeight) - (0.5f * cellHeight),
                                                     -sizeY); //size y because screen 16:9 (height more important height)
    }

    private bool CheckDidNeibor()
    {
        BlockData frst = BlockData.first.gameObject.GetComponent<BlockData>();
        BlockData scnd = BlockData.second.gameObject.GetComponent<BlockData>();
        //Direction test

        if (frst.x - 1 == scnd.x && frst.y == scnd.y)
            return true; //Left
        else if (frst.x + 1 == scnd.x && frst.y == scnd.y)
            return true; // Right
        else if (frst.x == scnd.x && frst.y - 1 == scnd.y)
            return true; // Down
        else if (frst.x == scnd.x && frst.y + 1  == scnd.y)
            return true; // Up
        Debug.Log("Block not neibor");
        return false;
    }

    private void Swap()
    {
        BlockData frst = BlockData.first.gameObject.GetComponent<BlockData>();
        BlockData scnd = BlockData.second.gameObject.GetComponent<BlockData>();

        //boot first pos
        Vector3 bootPos = frst.transform.position;
        int bootX = frst.x;
        int bootY = frst.y;

        frst.transform.position = scnd.transform.position;
        scnd.transform.position = bootPos;

        // first data same as second
        frst.x = scnd.x;
        frst.y = scnd.y;

        //second data same as boot
        scnd.x = bootX;
        scnd.y = bootY;

        //block type on board data;
        board[frst.x, frst.y] = frst.type;
        board[scnd.x, scnd.y] = scnd.type;

        UnpickBlocks();
    }

    private void UnpickBlocks()
    {
        BlockData.first = null;
        BlockData.second = null;
        targetLight.SetActive(false);
    }
}

