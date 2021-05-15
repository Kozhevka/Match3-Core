using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsHolder : MonoBehaviour
{
    [SerializeField] private TextAsset[] levelsArray;

    

    public void GetLevel(int levelNumber)
    {
        if (levelNumber < levelsArray.Length)
        {
            BoardMatch3.instance.LoadLevel(levelsArray[levelNumber]);
        }
    }
    public void RandomLevel(int sideSize)
    {
        if (sideSize < 21 || sideSize > 2) //kinda min-max size
        {
            BoardMatch3.instance.CreateDesk(sideSize, sideSize);
        }
    }
}
