using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    private int curTrack = 1;
    private Vector3 tempColor;
    public Image previewNote;
    public Image[] trackButtons;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i <= 6; i++)
        {
            if (!PlayerPrefs.HasKey("Track " + i + " R"))
            {
                PlayerPrefs.SetFloat("Track " + i + " R", .5f);
                PlayerPrefs.SetFloat("Track " + i + " G", .5f);
                PlayerPrefs.SetFloat("Track " + i + " B", .5f);
            }
            trackButtons[i - 1].color = new Color(PlayerPrefs.GetFloat("Track " + i + " R"),
                                                  PlayerPrefs.GetFloat("Track " + i + " G"),
                                                  PlayerPrefs.GetFloat("Track " + i + " B"));
        }
        curTrack = 1;
        tempColor = new Vector3(PlayerPrefs.GetFloat("Track 1 R"),
                                PlayerPrefs.GetFloat("Track 1 G"),
                                PlayerPrefs.GetFloat("Track 1 B"));
        UpdatePreview(tempColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCurTrack(int trackNum)
    {
        curTrack = trackNum;
    }

    public void UpdatePreview(Vector3 colorIn)
    {
        previewNote.color = new Color(colorIn.x, colorIn.y, colorIn.z);
    }

    public void ApplyColor()
    {
        PlayerPrefs.SetFloat("Track " + curTrack + " R", tempColor.x);
        PlayerPrefs.SetFloat("Track " + curTrack + " G", tempColor.y);
        PlayerPrefs.SetFloat("Track " + curTrack + " B", tempColor.z);
        trackButtons[curTrack - 1].color = new Color(PlayerPrefs.GetFloat("Track " + curTrack + " R"),
                                                     PlayerPrefs.GetFloat("Track " + curTrack + " G"),
                                                     PlayerPrefs.GetFloat("Track " + curTrack + " B"));
    }
    
    public void CancelColor()
    {
        tempColor = new Vector3(PlayerPrefs.GetFloat("Track " + curTrack + " R"),
                                PlayerPrefs.GetFloat("Track " + curTrack + " G"),
                                PlayerPrefs.GetFloat("Track " + curTrack + " B"));
        UpdatePreview(tempColor);
    }

    public void AddRed()
    {
        if (tempColor.x < 1)
        {
            tempColor.x += .1f;
            if (tempColor.x > 1)
            {
                tempColor.x = 1;
            }
            UpdatePreview(tempColor);
        }
    }

    public void RemoveRed()
    {
        if (tempColor.x > 0)
        {
            tempColor.x -= .1f;
            if (tempColor.x < 0)
            {
                tempColor.x = 0;
            }
            UpdatePreview(tempColor);
        }
    }

    public void AddGreen()
    {
        if (tempColor.y < 1)
        {
            tempColor.y += .1f;
            if (tempColor.y > 1)
            {
                tempColor.y = 1;
            }
            UpdatePreview(tempColor);
        }
    }

    public void RemoveGreen()
    {
        if (tempColor.y > 0)
        {
            tempColor.y -= .1f;
            if (tempColor.y < 0)
            {
                tempColor.y = 0;
            }
            UpdatePreview(tempColor);
        }
    }

    public void AddBlue()
    {
        if (tempColor.z < 1)
        {
            tempColor.z += .1f;
            if (tempColor.z > 1)
            {
                tempColor.z = 1;
            }
            UpdatePreview(tempColor);
        }
    }

    public void RemoveBlue()
    {
        if (tempColor.z > 0)
        {
            tempColor.z -= .1f;
            if (tempColor.z < 0)
            {
                tempColor.z = 0;
            }
            UpdatePreview(tempColor);
        }
    }

    public void AddWhite()
    {
        AddRed();
        AddGreen();
        AddBlue();
    }

    public void RemoveWhite()
    {
        RemoveRed();
        RemoveGreen();
        RemoveBlue();
    }
}
