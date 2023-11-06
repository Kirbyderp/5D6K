using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track2D : MonoBehaviour
{
    public int trackNum;
    private bool isHolding = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsHolding()
    {
        return isHolding;
    }

    public void SetIsHolding(bool holdIn)
    {
         isHolding = holdIn;
    }

    public void MoveNotesDown(float distance)
    {
        RectTransform[] allChildren = GetComponentsInChildren<RectTransform>();
        foreach (RectTransform child in allChildren)
        {
            if (child.gameObject.CompareTag("Note"))
            {
                child.localPosition = new Vector3(child.localPosition.x,
                                                  child.localPosition.y - distance,
                                                  child.localPosition.z);
            }
        }
    }
}
