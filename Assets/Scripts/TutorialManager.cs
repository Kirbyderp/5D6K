using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private float[] pauseTimes = { 0, 8.7f, 17.5f, 39.1f, 90 };
    private int[] numTextBoxes = { 4, 4, 3, 1, 0 };
    private int curTextBox = 0;
    private string[][] textBoxes = { new string[] { "Welcome to the RhythMulti tutorial!\n\n" +
                                                    "In this game, there are two kinds of notes that you will need to hit.",
                                                    "The first kind of notes are 2D notes.\n\n2D notes will move down the four " +
                                                    "tracks that are ahead of you. When a 2D note fully obscures the target " +
                                                    "at the bottom of the track, press the corresponding button on your " +
                                                    "controllers to hit it. Successfully hitting a note will cause the target " +
                                                    "in its track to flash.",
                                                    "To hit 2D notes, use the grip and trigger buttons on your controllers. \n\n" +
                                                    "If you hold your controllers like the ones ahead, the placement of the grip " +
                                                    "and trigger buttons will match with the placement of the tracks. You do not " +
                                                    "need to hold your controllers in any particular way to hit 2D notes, " +
                                                    "however.",
                                                    "Furthermore, notice that the buttons on your controller are color-coded " +
                                                    "to their corresponding track.\n\n" +
                                                    "Try practicing hitting some 2D notes!"},
                                     new string[] { "The second kind of notes are 3D notes, which will move towards you from in " +
                                                    "front of you.",
                                                    "If you look below you, you should see a red line.\n\n" +
                                                    "To hit 3D notes, move your corresponding hand so that it hits the note " +
                                                    "when it passes directly ofer the red line. 3D notes that appear on the " +
                                                    "left should be hit by your left hand, and those on the right hit by your " +
                                                    "right.", 
                                                    "It is recommended that you stand a foot or two behind the red line, but " +
                                                    "feel free to position yourself in whatever way works best for you.\n\n" +
                                                    "Finally, when you hit a 3D note with good timing, the corresponding " +
                                                    "controller will vibrate slightly. You can disable this in the options menu.",
                                                    "Try practicing hitting some 3D notes!"},
                                     new string[] { "There is one more kind of 2D note which requires being held.\n\n" +
                                                    "These 2D notes will have a line emanating from them connecting them to " +
                                                    "another 2D note. You must hold the corresponding button throughout the " +
                                                    "duration of the line, releasing it to hit the second part of the note.",
                                                    "If you ever release the button too early, or if you miss the first part " +
                                                    "of this note entirely, the connecting line will darken. If this happens, " +
                                                    "you will be able to hit the second part of the note like a normal 2D note.",
                                                    "Additionally, the target will repeatedly flash while successfully holding a " +
                                                    "2D held note.\n\nTry practicing hitting some 2D held notes!"},
                                     new string[] { "Finally, you can pause the game during a song using the triple bar button " +
                                                    "on your left controller.\n\nHave fun playing Rhythmulti!"},};
    private int nextPauseTimeIndex = 0;
    private bool waitingForTutAnim = false;
    private GameObject tutMenu;
    private TMPro.TextMeshProUGUI tutText;
    private SongManager songManager;
    public GameObject leftController, rightController;

    // Start is called before the first frame update
    void Start()
    {
        tutMenu = GameObject.Find("Tutorial Menu");
        tutText = GameObject.Find("Tutorial Info Text").GetComponent<TMPro.TextMeshProUGUI>();
        tutMenu.transform.localScale = Vector3.zero;
        tutMenu.SetActive(false);
        songManager = GameObject.Find("SongManager").GetComponent<SongManager>();
        leftController.SetActive(false);
        rightController.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetNextPauseTimeIndex()
    {
        nextPauseTimeIndex = 0;
        curTextBox = 0;
    }

    public void ShowTutMenu()
    {
        waitingForTutAnim = true;
        curTextBox = 0;
        tutMenu.SetActive(true);
        tutText.text = textBoxes[nextPauseTimeIndex][curTextBox];
        StartCoroutine(ExpandTutMenuAnim());
    }

    public void AdvanceNextPauseTime()
    {
        nextPauseTimeIndex++;
        StartCoroutine(ShrinkTutMenuAnim());
    }

    public float GetNextPauseTime()
    {
        return pauseTimes[nextPauseTimeIndex];
    }

    public void NextTextBox()
    {
        if (!waitingForTutAnim)
        {
            curTextBox++;
            if (nextPauseTimeIndex == 0 && curTextBox == 2)
            {
                leftController.SetActive(true);
                rightController.SetActive(true);
            }
            if (nextPauseTimeIndex == 0 && curTextBox == 3)
            {
                leftController.SetActive(false);
                rightController.SetActive(false);
            }
            if (curTextBox < numTextBoxes[nextPauseTimeIndex])
            {
                tutText.text = textBoxes[nextPauseTimeIndex][curTextBox];
            }
            else
            {
                waitingForTutAnim = true;
                AdvanceNextPauseTime();
            }
        }
    }

    IEnumerator ShrinkTutMenuAnim()
    {
        for (int frame = 0; frame < 50; frame++)
        {
            tutMenu.transform.localScale -= new Vector3(1 / 50f, 1 / 50f, 1 / 50f);
            yield return new WaitForSeconds(1 / 100f);
        }
        tutMenu.transform.localScale = Vector3.zero;
        tutMenu.SetActive(false);
        waitingForTutAnim = false;
        songManager.ContinueFromTut();
    }

    IEnumerator ExpandTutMenuAnim()
    {
        for (int frame = 0; frame < 50; frame++)
        {
            tutMenu.transform.localScale += new Vector3(1 / 50f, 1 / 50f, 1 / 50f);
            yield return new WaitForSeconds(1 / 100f);
        }
        tutMenu.transform.localScale = Vector3.one;
        waitingForTutAnim = false;
    }

    public void EndTutPremature()
    {
        ResetNextPauseTimeIndex();
        tutMenu.transform.localScale = Vector3.zero;
        tutMenu.SetActive(false);
    }
}
