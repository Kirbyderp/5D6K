using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Song
{
    private Note2D[] all2DNotes;
    private Note3D[] all3DNotes;
    private float length, beatLength;
    
    public Song(string songPath)
    {
        StreamReader reader = new StreamReader(songPath);
        
        string line = reader.ReadLine();
        int spaceIndex = line.IndexOf(" ");
        length = float.Parse(line.Substring(0, spaceIndex));
        line = line.Substring(spaceIndex + 1);
        beatLength = 60/float.Parse(line);

        int note2DCount = -1, note3DCount = -1;

        while (line != "-----")
        {
            line = reader.ReadLine();
            note2DCount++;
        }
        while (line != "EOF")
        {
            line = reader.ReadLine();
            note3DCount++;
        }

        reader = new StreamReader(songPath);
        line = reader.ReadLine();

        all2DNotes = new Note2D[note2DCount];
        all3DNotes = new Note3D[note3DCount];
        for (int i = 0; i < note2DCount; i++)
        {
            line = reader.ReadLine();
            all2DNotes[i] = new Note2D(int.Parse(line.Substring(0, 1)), float.Parse(line.Substring(2)));
            //Debug.Log(all2DNotes[i]);
        }
        line = reader.ReadLine();
        for (int i = 0; i < note3DCount; i++)
        {
            line = reader.ReadLine();
            //Initialize 3D Note Entry
        }
    }

    public Note2D[] GetAll2DNotes()
    {
        return all2DNotes;
    }

    public float GetBeatLength()
    {
        return beatLength;
    }
}