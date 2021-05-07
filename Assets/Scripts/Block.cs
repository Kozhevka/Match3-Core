using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int iD; //currentBlockType
    public int x; // xAxisPosition
    public int y; // yAxisPosition


    
    private Vector3 myScale;

   

    void Start()
    {
        myScale = transform.localScale;
        
    }

    

    //If same type block neibor after move
    //If pick this block
    public static Transform first;
    public static Transform second;

    private void OnMouseOver()
    {
        transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        
        if(Input.GetMouseButtonDown(0))
        {
            if(!first)
            {
                first = transform;
            }
            else if (first != transform && !second)
            {
                second = transform;
            }
        }
    }

    private void OnMouseExit()
    {
        transform.localScale = myScale;
    }
}
