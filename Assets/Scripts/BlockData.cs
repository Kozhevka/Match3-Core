using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData : MonoBehaviour
{
    public int type;
    public int x;
    public int y;

    
    private Vector3 passiveScale;
    private BoxCollider sampleCell;
    private Vector3 activeScale;

    public static Transform first;
    public static Transform second;

    private void Start()
    {
        sampleCell = BoardMatch3.instance.sampleOfBlockCell;

        passiveScale = transform.localScale;
        activeScale = sampleCell.transform.localScale;
    }

    private void OnMouseOver()
    {
        SetScale(activeScale);
        
        if(Input.GetMouseButtonDown(0))
        {
            if(first == null)
            {
                first = transform;

            }
            else if (first != transform && second == null)
            {
                second = transform;
            }
        }    
    }

    private void OnMouseExit()
    {
        SetScale(passiveScale);
    }

    private void SetScale(Vector3 newScale)
    {
        transform.localScale = newScale;
    }
}
