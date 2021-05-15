using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPool : MonoBehaviour
{
    public static BlockPool instance;

    [SerializeField] private GameObject blockZero;
    [SerializeField] private GameObject blockOne;
    [SerializeField] private GameObject blockTwo;
    [SerializeField] private GameObject blockThree;

    [SerializeField] private List<GameObject> pooledObjectsZero;
    [SerializeField] private List<GameObject> pooledObjectsOne;
    [SerializeField] private List<GameObject> pooledObjectsTwo;
    [SerializeField] private List<GameObject> pooledObjectsThree;


    private List<List<GameObject>> pooledObjects;
    private List<GameObject> blockWhatNeedToPool;

    private int minimumAmountToPool = 15;

    private void Awake()
    {
        if (BlockPool.instance == null)
            instance = this;
        else
        {
            Debug.LogError("BlockPool.instance already exist");
            Destroy(this.gameObject);
        }


    }

    private void Start()
    {
        blockWhatNeedToPool = new List<GameObject>();
        blockWhatNeedToPool.Add(blockZero);
        blockWhatNeedToPool.Add(blockOne);
        blockWhatNeedToPool.Add(blockTwo);
        blockWhatNeedToPool.Add(blockThree);

        pooledObjects = new List<List<GameObject>>();
        pooledObjects.Add(pooledObjectsZero);
        pooledObjects.Add(pooledObjectsOne);
        pooledObjects.Add(pooledObjectsTwo);
        pooledObjects.Add(pooledObjectsThree);


        for (int i = 0; i < pooledObjects.Count; i++)
        {
            pooledObjects[i] = new List<GameObject>();

            GameObject pooledBlock;
            for (int p = 0; p < minimumAmountToPool; p++)
            {
                pooledBlock = Instantiate(blockWhatNeedToPool[i]);
                pooledBlock.SetActive(false);
                pooledObjects[i].Add(pooledBlock);
            }
        }
    }

    public GameObject GetPooledBlock(int blockType)
    {
        List<GameObject> pickedList = pooledObjects[blockType];
        for (int i = 0; i < pickedList.Count; i++)
        {
            if (!pickedList[i].activeInHierarchy)
            {
                return pickedList[i];
            }
        }
        return AddMoreBlocktoPool(blockType);
    }

    private GameObject AddMoreBlocktoPool(int blockType)
    {
        GameObject additionalBlock;
        additionalBlock = Instantiate(blockWhatNeedToPool[blockType]);
        additionalBlock.SetActive(false);
        pooledObjects[blockType].Add(additionalBlock);
        return additionalBlock;

        Debug.Log($"Cant add block type {blockType}");
        return null;
        
    }
}
