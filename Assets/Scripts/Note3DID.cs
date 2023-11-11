using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note3DID : MonoBehaviour
{
    private int noteID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNoteID(int idIn)
    {
        noteID = idIn;
    }

    public int GetNoteID()
    {
        return noteID;
    }
}
