using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note2D
{
    private int trackNum; //1-4
    private int type; //0: single hit, 1: hold start, 2: hold end
    private float time;
    private bool hasSpawned, wasHit, wasMissed, wasFound;

    private int spawnIndex;

    public Note2D(int trackIn, int typeIn, float timeIn)
    {
        trackNum = trackIn;
        type = typeIn;
        time = timeIn;
        hasSpawned = false;
        wasHit = false;
        wasMissed = false;
        wasFound = false;
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

    public bool WasFound()
    {
        return wasFound;
    }

    public void Find()
    {
        wasFound = true;
    }

    public override string ToString()
    {
        return "Track " + trackNum + " at " + time + " seconds.";
    }
}
