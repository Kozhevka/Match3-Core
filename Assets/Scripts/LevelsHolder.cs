using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsHolder : MonoBehaviour
{
    [SerializeField] private TextAsset[] levelsArray;

    private void Start()
    {
        GetLevel(0);
    }

    public void GetLevel(int levelNumber)
    {
        if (levelNumber < levelsArray.Length)
        {
            BoardMatch3.instance.LoadLevel(levelsArray[levelNumber]);
        }
    }
}
