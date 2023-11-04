using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note3D : MonoBehaviour
{
    private int trackNum;
    private float time;
    private Vector3 startingPos;

    public Note3D(int trackIn, float timeIn, Vector3 posIn)
    {
        trackNum = trackIn;
        time = timeIn;
        startingPos = posIn;
    }

    public int GetTrackNum()
    {
        return trackNum;
    }

    public float GetTime()
    {
        return time;
    }
}
