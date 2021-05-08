using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMatch3 : MonoBehaviour
{
    public static BoardMatch3 instance;


    [SerializeField] private int[,] board;
    [SerializeField] private Vector3[,] onBoardPositions;

    [SerializeField] private GameObject[] blocksObj;

    public BoxCollider sampleOfBlockCell;
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
    }
}
