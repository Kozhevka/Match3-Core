using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardMatch3 : MonoBehaviour
{
    public static BoardMatch3 instance;

    public enum MoveStageEnum
    {
        Menu,
        PlayerMakeMove,
        BlocksMoving,
        DestroyBlock,
        AfterFallCheck
    }

    private MoveStageEnum movePhaseEnum;

    [SerializeField] private int[,] board;

    [SerializeField] private GameObject cameraObj;

    public GameObject targetLight;

    [SerializeField] private int deskSizeX;
    [SerializeField] private int deskSizeY;

    [SerializeField] private float moveTime = 1;
    
    
    private readonly int emptyCellType = 100;

    [SerializeField] private List<BlockData> blocksWhatNeedCheck;

    private ScoreCounter scoreCounterScript;

    private int scoreNeedToAdd = 0;
    private int scoreMultiplier = 1;

    private BlockPool blockPoolScript;
    private int blockTypes;

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
        movePhaseEnum = MoveStageEnum.PlayerMakeMove;
        scoreCounterScript = ScoreCounter.instance;

        blockPoolScript = BlockPool.instance;
        blockTypes = BlockPool.instance.blockWhatNeedToPool.Count;
    }


    private void Update()
    {
        if (BlockData.first && BlockData.second && movePhaseEnum == MoveStageEnum.PlayerMakeMove) //if exist
        {
            BlockData frst = BlockData.first.gameObject.GetComponent<BlockData>();
            BlockData scnd = BlockData.second.gameObject.GetComponent<BlockData>();
            if (CheckDidNeibor(frst, scnd))
            {
                 Swap(frst, scnd);
                 StartCoroutine(WaitForFinishSwap(frst, scnd));
            }
            else
            {
                UnpickBlocks();
            }
        }
        
    }

    MoveStageEnum ChangeStageEnum(MoveStageEnum nextStage)
    {
        movePhaseEnum = nextStage;

        if(movePhaseEnum == MoveStageEnum.PlayerMakeMove)
        {
            scoreMultiplier = 1;
            scoreCounterScript.AddScore(0, 1);
        }
        
        if (movePhaseEnum == MoveStageEnum.AfterFallCheck)
        {
            bool getMatch = false;


            BlockData[] allBlockData = FindObjectsOfType(typeof(BlockData)) as BlockData[];

            foreach (BlockData _blockData in blocksWhatNeedCheck)
            {
                if(_blockData)
                    if(CheckMatch(_blockData, allBlockData))
                {
                    getMatch = true;
                }
                
            }
            blocksWhatNeedCheck.RemoveAll(BlockData => BlockData == null);

            if (!getMatch)
            {
                ChangeStageEnum(MoveStageEnum.PlayerMakeMove);
            }
            else
            {
                MoveAllBlockDown();
                AddScoreToScoreCounter();
            }
        }
        return nextStage;
    }

    

    public void CreateDesk(int sizeX, int sizeY)
    {
        RemoveBoard(); //remove previous desk

        deskSizeX = sizeX;
        deskSizeY = sizeY;
        board = new int[sizeX, sizeY];

        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                int blockType = Random.Range(0, blockTypes);
                SpawnBlockAtPosition(x, y, blockType);
            }
        }
        CameraPosition(sizeX, sizeY);
        movePhaseEnum = MoveStageEnum.PlayerMakeMove;
    }

    public void LoadLevel(TextAsset levelTextAsset)
    {
        RemoveBoard();

        List<string> levelStringData;
        levelStringData = levelTextAsset.text.Split(';').ToList();

        int[] lvlData = new int[levelStringData.Count];

        

        for (int i = 0; i < levelStringData.Count; i++)
        {
            if(int.TryParse(levelStringData[i], out int stringInt))
            {
                lvlData[i] = stringInt;
            }
            else
            {
                Debug.LogError($"Not correct levelConfig on {i} string");
            }
        }
        deskSizeX = lvlData[0];
        deskSizeY = lvlData[1];

        board = new int[deskSizeX, deskSizeY];
        int blockTypeCount = 2;

        for (int y = deskSizeY-1; y >= 0; y--)//levels load left-right > up-down;
        {
            for (int x = 0; x < deskSizeX; x++)
            {
                
                SpawnBlockAtPosition(x, y, lvlData[blockTypeCount]);
                blockTypeCount++;

            }
        }
        CameraPosition(lvlData[0], lvlData[1]);
        movePhaseEnum = MoveStageEnum.PlayerMakeMove;

    }

    private void SpawnBlockAtPosition(int x, int y, int type)
    {
        GameObject block = Instantiate(blockPoolScript.GetPooledBlock(type), new Vector3(x, y, 0f), Quaternion.identity);
        block.SetActive(true);

        board[x, y] = type; //set block identification

        BlockData blockInfo = block.gameObject.GetComponent<BlockData>();
        blockInfo.x = x;
        blockInfo.y = y;
        blockInfo.type = type;
        blocksWhatNeedCheck.Add(blockInfo);
    }

    private void CameraPosition(int sizeX, int sizeY)
    {
        cameraObj.transform.position = new Vector3((sizeX * 0.5f - 0.5f), //first 0.5ff - halfleght, second halfBlock because center of 1st block on (0,0,0)
                                                    sizeY * 0.5f - 0.5f,
                                                     -sizeY); //size y because screen 16:9 (height more important)
    }

    private bool CheckDidNeibor(BlockData frst, BlockData scnd)
    {
        if (!frst || !scnd)
            return false;
        //Direction test
        if (frst.x - 1 == scnd.x && frst.y == scnd.y)
            return true; //Left
        else if (frst.x + 1 == scnd.x && frst.y == scnd.y)
            return true; // Right
        else if (frst.x == scnd.x && frst.y - 1 == scnd.y)
            return true; // Down
        else if (frst.x == scnd.x && frst.y + 1  == scnd.y)
            return true; // Up

        return false;
    }

    private bool Swap(BlockData frst, BlockData scnd)
    {
        movePhaseEnum = MoveStageEnum.BlocksMoving;
        
        //boot first pos
        Vector3 bootPos = frst.transform.position;
        int bootX = frst.x;
        int bootY = frst.y;

        StartCoroutine(MoveBlockToNextPosition(frst, scnd.transform.position));
        StartCoroutine(MoveBlockToNextPosition(scnd, bootPos));

        // first data same as second
        frst.x = scnd.x;
        frst.y = scnd.y;

        //second data same as boot
        scnd.x = bootX;
        scnd.y = bootY;

        //block type on board data;
        board[frst.x, frst.y] = frst.type;
        board[scnd.x, scnd.y] = scnd.type;

        return true;
        
    }

    private void UnpickBlocks()
    {
        BlockData.first = null;
        BlockData.second = null;
        targetLight.SetActive(false);
        movePhaseEnum = MoveStageEnum.PlayerMakeMove;
    }

    private bool CheckMatch(BlockData block, BlockData[] allBlockData)
    {
        int countRight = 0;
        int countLeft = 0;
        int countUp = 0;
        int countDown = 0;

        //check horizontal
        //right
        for (int rightDir = block.x + 1; rightDir < deskSizeX; rightDir++) 
        {
            if (board[rightDir, block.y] == block.type)
            {
                countRight++;
            }
            else
                break;
        }
        //left
        for (int leftDir = block.x - 1; leftDir >= 0; leftDir--)
        {
            if (board[leftDir, block.y] == block.type)
            {
                countLeft++;
            }
            else 
                break;
        }

        //check vertical
        //up
        for(int upDir = block.y + 1; upDir < deskSizeY; upDir++)
        {
            if (board[block.x, upDir] == block.type)
            {
                countUp++;
            }
            else
                break;
        }
        //down
        for (int downDir = block.y - 1; downDir >= 0; downDir--)
        {
            if (board[block.x, downDir] == block.type)
            {
                countDown++;
            }
            else
                break;
        }


        //destroy block
        
        int horizontalMatches = countRight + countLeft;
        int verticalMatches = countUp + countDown;
        bool verticalDestroy = false;
        bool horizontalDestroy = false;

        if (countRight + countLeft >= 2) //+ 1 = 3.  1- its current block
            horizontalDestroy = true;
        if (countUp + countDown >= 2)
            verticalDestroy = true;

        if (horizontalMatches == 3)
            FourMatches(block, 'y', allBlockData);
        if (verticalMatches == 3)
            FourMatches(block, 'x', allBlockData);

        if (horizontalMatches > 3 || verticalMatches > 3)
            FiveMatches(block, allBlockData);
        
        if (horizontalDestroy) 
        {
            //right destroy
            for (int destrRight = 1; destrRight <= countRight; destrRight++)
            {
                foreach (BlockData blockData in allBlockData)
                {
                    if (blockData.x == block.x + destrRight && blockData.y == block.y)
                    {
                        DestroyBlock(blockData);
                    }
                }
            }
            //left destroy
            for (int destrLeft = 1; destrLeft <= countLeft; destrLeft++)
            {
                foreach (BlockData blockData in allBlockData)
                {
                    if (blockData.x == block.x - destrLeft && blockData.y == block.y)
                    {
                        DestroyBlock(blockData);
                    }
                }
            }
        }

        if (verticalDestroy)
        {
            //up destroy
            for (int destrUp = 1; destrUp <= countUp; destrUp++)
            {
                foreach (BlockData blockData in allBlockData)
                {
                    if (blockData.x == block.x && blockData.y == block.y + destrUp)
                    {
                       DestroyBlock(blockData);
                    }
                }
            }
            //down destroy
            for (int destrDown = 1; destrDown <= countDown; destrDown++)
            {
                foreach (BlockData blockData in allBlockData)
                {
                    if (blockData.x == block.x && blockData.y == block.y - destrDown)
                    {
                        DestroyBlock(blockData);
                    }
                }
            }
        }

        if (horizontalDestroy || verticalDestroy)
        {
            ChangeStageEnum(MoveStageEnum.DestroyBlock);
            DestroyBlock(block); //block activator

            ChangeStageEnum(MoveStageEnum.BlocksMoving);
            return true;
        }

        return false;
    }

    private void DestroyBlock(BlockData blockInfo)
    {
        if (blockInfo)
        {
            board[blockInfo.x, blockInfo.y] = emptyCellType;

            Destroy(blockInfo);
            blockInfo.gameObject.SetActive(false);
            
            scoreNeedToAdd++;
        }
    }

    IEnumerator  MoveBlockToNextPosition(BlockData _blockData, Vector3 nextPosition)
    {
        _blockData.blockMoving = true;

        while (_blockData && _blockData.transform.position != nextPosition)
        {
            yield return new WaitForEndOfFrame();
            if(_blockData)
                _blockData.transform.position = Vector3.MoveTowards(_blockData.transform.position, nextPosition, Time.deltaTime / moveTime);
            _blockData.blockMoving = false;
        }
    }
    

    IEnumerator WaitForFinishSwap(BlockData frst, BlockData scnd)
    {
        yield return new WaitForSeconds(moveTime);

        BlockData[] allBlockData = FindObjectsOfType(typeof(BlockData)) as BlockData[];
        bool firstBlock = CheckMatch(frst, allBlockData);
        bool secondBlock = CheckMatch(scnd, allBlockData);

        if (firstBlock || secondBlock)
        {
            AddScoreToScoreCounter();
            UnpickBlocks();
            MoveAllBlockDown();
        }
        else
        {
            Swap(frst, scnd);
            UnpickBlocks();
        }
    }

    private void MoveAllBlockDown()
    {
        BlockData[] allBlockData = FindObjectsOfType(typeof(BlockData)) as BlockData[];

        for (int x = 0; x < deskSizeX; x++)
        {
            for (int y = 1; y < deskSizeY; y++) //minus lowest
            {
                if (board[x,y] !=emptyCellType && board[x, y-1] == emptyCellType)
                {
                    BlockData blockOnXYPosition = null;

                    foreach (BlockData _blockData in allBlockData)
                    {
                        if (_blockData.x == x && _blockData.y == y)
                        {
                            blockOnXYPosition = _blockData;
                            break;
                        }
                    }
                    Vector3 loverPosition = new Vector3(x, y - 1, 0f);

                    if (blockOnXYPosition != null)
                    {
                        StartCoroutine(MoveBlockToNextPosition(blockOnXYPosition, loverPosition));
                        
                    }
                    else
                        Debug.LogError("Cant find _blockData");

                    UpdateBoardCellInfoAfterFall(blockOnXYPosition, loverPosition);

                }
                
            }
        }

        StartCoroutine(WaitForBlockFall());
        
    }

    IEnumerator WaitForBlockFall()
    {
        yield return new WaitForSeconds(moveTime);
        SpawnNewBlock();

        bool haveEmptyCell = false;

        for (int x = 0; x < deskSizeX; x++)
        {
            for (int y = 1; y < deskSizeY; y++)
            {
                if (board[x, y] != emptyCellType && board[x, y - 1] == emptyCellType)
                {
                    haveEmptyCell = true;
                    break;
                }
            }
        }
        if (haveEmptyCell)
        {
            MoveAllBlockDown();
        }
        else
        {
            ChangeStageEnum(MoveStageEnum.AfterFallCheck);
        }
    }

    private void UpdateBoardCellInfoAfterFall(BlockData _blockData, Vector2 nextPos)
    {
        board[_blockData.x, _blockData.y] = emptyCellType;

        _blockData.x = (int)nextPos.x;
        _blockData.y = (int)nextPos.y;
        board[(int)nextPos.x, (int)nextPos.y] = _blockData.type;
    }

    private void SpawnNewBlock()
    {
        for (int x = 0; x < deskSizeX; x++)
        {
            if (board[x, deskSizeY-1] == emptyCellType)
            {
                int blockType = Random.Range(0, blockTypes);
                SpawnBlockAtPosition(x, deskSizeY-1, blockType);
            }
        }
    }

    private void AddScoreToScoreCounter()
    {
        scoreCounterScript.AddScore(scoreNeedToAdd, scoreMultiplier);
        scoreNeedToAdd = 0;
        scoreMultiplier++;
    }

    private void FourMatches(BlockData startBlock, char xOrYDirection, BlockData[] allBlocks)
    {
        if (xOrYDirection == 'x') //vertical
        {
            foreach (BlockData _blockData in allBlocks)
            {
                if (_blockData.x == startBlock.x)
                    DestroyBlock(_blockData);
            }
        }
        else if (xOrYDirection == 'y') //horizontal
        {
            foreach (BlockData _blockData in allBlocks)
            {
                if (_blockData.y == startBlock.y)
                    DestroyBlock(_blockData);
            }
        }
    }
    private void FiveMatches(BlockData startBlock, BlockData[] allBlocks)
    {
        foreach (BlockData _blockData in allBlocks)
        {
            if (_blockData.type == startBlock.type)
                DestroyBlock(_blockData);
        }
    }

    private void RemoveBoard()
    {
        blocksWhatNeedCheck.Clear();

        BlockData[] allBlockData = FindObjectsOfType(typeof(BlockData)) as BlockData[];
        if (allBlockData != null)
        {
            foreach (BlockData _blockData in allBlockData)
            {
                DestroyBlock(_blockData);
            }
        }
        board = null;
        
        scoreNeedToAdd = 0;
        
        scoreCounterScript.ResetScore();
    }
}

