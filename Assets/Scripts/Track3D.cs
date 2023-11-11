using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track3D : MonoBehaviour
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

    public void MoveNotesForwards(float deltaDistance, Note3D[] note3Ds)
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.CompareTag("Note"))
            {
                child.localPosition += (note3Ds[child.GetComponent<Note3DID>().GetNoteID()].GetHitPos() -
                                       note3Ds[child.GetComponent<Note3DID>().GetNoteID()].GetStartingPos()) * deltaDistance;
            }
        }
    }
}
