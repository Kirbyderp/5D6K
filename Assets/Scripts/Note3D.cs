using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note3D
{
    private int trackNum, type;
    private float time;
    private Vector3 startingPos, hitPos;
    private bool hasSpawned, wasHit, wasMissed;

    private int spawnIndex;

    public Note3D(int trackIn, int typeIn, float timeIn, Vector3 startPosIn, Vector3 hitPosIn)
    {
        trackNum = trackIn;
        time = timeIn;
        type = typeIn;
        startingPos = startPosIn;
        hitPos = hitPosIn;
        hasSpawned = false;
        wasHit = false;
        wasMissed = false;
        //Debug.Log(this);
    }

    public Note3D()
    {

    }

    public int GetTrackNum()
    {
        return trackNum;
    }

    public int GetNoteType()
    {
        return type;
    }

    public float GetTime()
    {
        return time;
    }

    public Vector3 GetStartingPos()
    {
        return startingPos;
    }

    public Vector3 GetHitPos()
    {
        return hitPos;
    }

    public bool HasSpawned()
    {
        return hasSpawned;
    }

    public void Spawn()
    {
        hasSpawned = true;
    }

    public bool WasHit()
    {
        return wasHit;
    }

    public void Hit()
    {
        wasHit = true;
    }

    public int GetSpawnIndex()
    {
        return spawnIndex;
    }

    public void SetSpawnIndex(int index)
    {
        spawnIndex = index;
    }

    public override string ToString()
    {
        return "Track " + trackNum + " at " + time + " seconds from position " + startingPos + " to " + hitPos + ".";
    }
}
