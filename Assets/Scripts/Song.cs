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
            all2DNotes[i] = new Note2D(int.Parse(line.Substring(0, 1)),
                                       int.Parse(line.Substring(2, 1)),
                                       float.Parse(line.Substring(4)));
            //Debug.Log(all2DNotes[i]);
        }
        line = reader.ReadLine();
        for (int i = 0; i < note3DCount; i++)
        {
            line = reader.ReadLine();
            int[] intsNeeded = new int[2] {int.Parse(line.Substring(0, 1)),
                                           int.Parse(line.Substring(2, 1)) };
            line = line.Substring(4);
            float[] floatsNeeded = new float[7];
            for (int j = 0; j < 7; j++)
            {
                if (j < 6)
                {
                    spaceIndex = line.IndexOf(" ");
                    floatsNeeded[j] = float.Parse(line.Substring(0, spaceIndex));
                    line = line.Substring(spaceIndex + 1);
                }
                else
                {
                    floatsNeeded[j] = float.Parse(line);
                }
            }
            all3DNotes[i] = new Note3D(intsNeeded[0], intsNeeded[1], floatsNeeded[0],
                            new Vector3(floatsNeeded[1], floatsNeeded[2], floatsNeeded[3]),
                            new Vector3(floatsNeeded[4], floatsNeeded[5], floatsNeeded[6]));
        }
    }

    public Note2D[] GetAll2DNotes()
    {
        return all2DNotes;
    }

    public Note3D[] GetAll3DNotes()
    {
        return all3DNotes;
    }

    public float GetBeatLength()
    {
        return beatLength;
    }
}