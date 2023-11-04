using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note2D
{
    private int trackNum;
    private float time;
    private bool hasSpawned, wasHit;
    private int spawnIndex;

    public Note2D(int trackIn, float timeIn)
    {
        trackNum = trackIn;
        time = timeIn;
        hasSpawned = false;
    }

    public int GetTrackNum()
    {
        return trackNum;
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

    public override string ToString()
    {
        return "Track " + trackNum + " at " + time + " seconds.";
    }
}
