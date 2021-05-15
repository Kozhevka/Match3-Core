using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData : MonoBehaviour
{
    public int type;
    public int x;
    public int y;
    public bool blockMoving = false;
    
    private Vector3 passiveScale;
    private Vector3 activeScale;

    public static Transform first;
    public static Transform second;

    private GameObject targetLight;

    private void Start()
    {
        targetLight = BoardMatch3.instance.targetLight;
        passiveScale = transform.localScale;

        activeScale = Vector3.one;
    }

    private void OnMouseOver()
    {
        SetScale(activeScale);
        
        if(Input.GetMouseButtonDown(0))
        {
            if(first == null)
            {
                first = transform;

                targetLight.transform.position = this.transform.position;
                targetLight.SetActive(true);
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
