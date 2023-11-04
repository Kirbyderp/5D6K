using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track2D : MonoBehaviour
{
    public int trackNum;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveNotesDown(float distance)
    {
        RectTransform[] allChildren = GetComponentsInChildren<RectTransform>();
        foreach (RectTransform child in allChildren)
        {
            if (child.gameObject.CompareTag("Note"))
            {
                child.position = new Vector3(child.position.x, child.position.y - distance, child.position.z);
            }
        }
    }
}
