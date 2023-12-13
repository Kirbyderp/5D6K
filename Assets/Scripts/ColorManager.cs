using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    private int curTrack = 1;
    private Vector3 tempColor;
    public Image previewNote2D;
    public Image[] trackButtons;
    public MeshRenderer[] controllerButtons;
    public Material[] controllerButtonMats;
    public GameObject[] controllers;
    public Material[] note3DMats;
    public GameObject previewNote3D;
    private MenuManager menuManager;
    
    // Start is called before the first frame update
    void Start()
    {
        menuManager = GameObject.Find("Menu Canvas").GetComponent<MenuManager>();
        controllers[0].SetActive(true);
        controllers[1].SetActive(true);
        for (int i = 1; i <= 6; i++)
        {
            if (!PlayerPrefs.HasKey("Track " + i + " R"))
            {
                if (i == 1 || i == 4)
                {
                    PlayerPrefs.SetFloat("Track " + i + " R", 0f);
                    PlayerPrefs.SetFloat("Track " + i + " G", 1f);
                    PlayerPrefs.SetFloat("Track " + i + " B", .7f);
                }
                else if (i == 2 || i == 3)
                {
                    PlayerPrefs.SetFloat("Track " + i + " R", 1f);
                    PlayerPrefs.SetFloat("Track " + i + " G", .6f);
                    PlayerPrefs.SetFloat("Track " + i + " B", 0f);
                }
                else if (i == 5)
                {
                    PlayerPrefs.SetFloat("Track " + i + " R", 0f);
                    PlayerPrefs.SetFloat("Track " + i + " G", 0f);
                    PlayerPrefs.SetFloat("Track " + i + " B", .8f);
                }
                else
                {
                    PlayerPrefs.SetFloat("Track " + i + " R", 1f);
                    PlayerPrefs.SetFloat("Track " + i + " G", 0f);
                    PlayerPrefs.SetFloat("Track " + i + " B", 0f);
                }
            }
            trackButtons[i - 1].color = new Color(PlayerPrefs.GetFloat("Track " + i + " R"),
                                                  PlayerPrefs.GetFloat("Track " + i + " G"),
                                                  PlayerPrefs.GetFloat("Track " + i + " B"));
            if (i < 5)
            {
                controllerButtonMats[i - 1].color = new Color(PlayerPrefs.GetFloat("Track " + i + " R"),
                                                              PlayerPrefs.GetFloat("Track " + i + " G"),
                                                              PlayerPrefs.GetFloat("Track " + i + " B"));
                controllerButtons[i - 1].material = controllerButtonMats[i - 1];
            }
        }
        curTrack = 1;
        tempColor = new Vector3(PlayerPrefs.GetFloat("Track 1 R"),
                                PlayerPrefs.GetFloat("Track 1 G"),
                                PlayerPrefs.GetFloat("Track 1 B"));
        note3DMats[0].color = new Color(tempColor.x, tempColor.y, tempColor.z);
        previewNote3D.SetActive(false);
        note3DMats[1].color = trackButtons[4].color;
        note3DMats[2].color = trackButtons[5].color;
        UpdatePreview(tempColor);
        menuManager.ColorHasSetUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCurTrack(int trackNum)
    {
        if (curTrack < 5 && trackNum >= 5)
        {
            previewNote3D.SetActive(true);
            previewNote2D.transform.parent.gameObject.SetActive(false);
            curTrack = trackNum;
            UpdatePreview(tempColor);
        }
        else if (curTrack > 4 && trackNum <= 4)
        {
            previewNote2D.transform.parent.gameObject.SetActive(true);
            previewNote3D.SetActive(false);
            curTrack = trackNum;
            UpdatePreview(tempColor);
        }
        else
        {
            curTrack = trackNum;
        }
        
    }

    public void UpdatePreview(Vector3 colorIn)
    {
        if (curTrack < 5)
        {
            previewNote2D.color = new Color(colorIn.x, colorIn.y, colorIn.z);
        }
        else
        {
            note3DMats[0].color = new Color(colorIn.x, colorIn.y, colorIn.z);
        }
    }

    public void ApplyColor()
    {
        PlayerPrefs.SetFloat("Track " + curTrack + " R", tempColor.x);
        PlayerPrefs.SetFloat("Track " + curTrack + " G", tempColor.y);
        PlayerPrefs.SetFloat("Track " + curTrack + " B", tempColor.z);
        trackButtons[curTrack - 1].color = new Color(PlayerPrefs.GetFloat("Track " + curTrack + " R"),
                                                     PlayerPrefs.GetFloat("Track " + curTrack + " G"),
                                                     PlayerPrefs.GetFloat("Track " + curTrack + " B"));
        if (curTrack == 5)
        {
            note3DMats[1].color = trackButtons[4].color;
        }
        else if (curTrack == 6)
        {
            note3DMats[2].color = trackButtons[5].color;
        }
        else
        {
            controllerButtonMats[curTrack - 1].color = trackButtons[curTrack - 1].color;
        }
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

    public Material GetLeft3DMat()
    {
        return note3DMats[1];
    }

    public Material GetRight3DMat()
    {
        return note3DMats[2];
    }
}
