using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private GameObject mainMenu, songMenu, pauseMenu, creditsMenu, optionsMenu, endMenu;
    private GameObject tutInfo, lavInfo, bDTInfo;
    private GameObject smInd, sdInd;
    private TMPro.TextMeshProUGUI durText, countText, missText, lmLabel, accText, comboText;
    private Slider optionVolSlider, pauseVolSlider;
    private SongManager songManager;
    private bool waitingForMenuAnim = false, startingSong = false, unpausingSong = false, restartingSong = false;
    private float[] indXLocalPos = { -48.30002f, -.9000242f, 46.49997f };
    private int storedSongMode = -1;
    private bool optionsMenuFound = false, colorMenuDone = false;

    // Start is called before the first frame update
    void Start()
    {
        optionsMenu = GameObject.Find("Options Menu");
        optionsMenuFound = true;
        songManager = GameObject.Find("SongManager").GetComponent<SongManager>();
        songManager.SetColorManager(GameObject.Find("Color Canvas").GetComponent<ColorManager>());
        songManager.SetStatsSummary(GameObject.Find("Song Stats Summary Text").GetComponent<TMPro.TextMeshProUGUI>());
        tutInfo = GameObject.Find("Tutorial Info Menu");
        lavInfo = GameObject.Find("Lavender Info Menu");
        bDTInfo = GameObject.Find("Dream Team Info Menu");
        optionVolSlider = GameObject.Find("Options Volume Slider").GetComponent<Slider>();
        pauseVolSlider = GameObject.Find("Pause Volume Slider").GetComponent<Slider>();
        smInd = GameObject.Find("Song Mode Indicator");
        sdInd = GameObject.Find("Song Diff Indicator");
        durText = GameObject.Find("Duration Text").GetComponent<TMPro.TextMeshProUGUI>();
        countText = GameObject.Find("Note Count Text").GetComponent<TMPro.TextMeshProUGUI>();
        lmLabel = GameObject.Find("Least Misses Label").GetComponent<TMPro.TextMeshProUGUI>();
        missText = GameObject.Find("Least Misses Text").GetComponent<TMPro.TextMeshProUGUI>();
        accText = GameObject.Find("Highest Acc Text").GetComponent<TMPro.TextMeshProUGUI>();
        comboText = GameObject.Find("Highest Combo Text").GetComponent<TMPro.TextMeshProUGUI>();
        UpdateStatsText(74, 90, 2, 0, 0);

        mainMenu = GameObject.Find("Main Menu");
        songMenu = GameObject.Find("Song Menu");
        pauseMenu = GameObject.Find("Pause Menu");
        creditsMenu = GameObject.Find("Credits Menu");
        endMenu = GameObject.Find("End Song Menu");
        songMenu.transform.localScale = Vector3.zero;
        songMenu.SetActive(false);
        pauseMenu.transform.localScale = Vector3.zero;
        pauseMenu.SetActive(false);
        creditsMenu.transform.localScale = Vector3.zero;
        creditsMenu.SetActive(false);
        optionsMenu.transform.localScale = Vector3.zero;
        endMenu.transform.localScale = Vector3.zero;
        endMenu.SetActive(false);
        lavInfo.SetActive(false);
        bDTInfo.SetActive(false);
        smInd.SetActive(false);
        sdInd.SetActive(false);
        tutInfo.SetActive(true);
        if (colorMenuDone)
        {
            optionsMenu.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ColorHasSetUp()
    {
        if (optionsMenuFound)
        {
            optionsMenu.SetActive(false);
        }
        else
        {
            colorMenuDone = true;
        }
    }

        IEnumerator ShrinkMenuAnim(GameObject prevMenu)
    {
        for (int frame = 0; frame < 50; frame++)
        {
            prevMenu.transform.localScale -= new Vector3(1 / 50f, 1 / 50f, 1 / 50f);
            yield return new WaitForSeconds(1 / 100f);
        }
        prevMenu.transform.localScale = Vector3.zero;
        prevMenu.SetActive(false);
        waitingForMenuAnim = false;
        if (startingSong)
        {
            startingSong = false;
            songManager.PlaySong();
        }
        if (unpausingSong)
        {
            unpausingSong = false;
            songManager.UnpauseSong();
        }
        if (restartingSong)
        {
            restartingSong = false;
            songManager.PlaySong();
        }
    }

    IEnumerator ExpandMenuAnim(GameObject nextMenu)
    {
        nextMenu.SetActive(true);
        for (int frame = 0; frame < 50; frame++)
        {
            nextMenu.transform.localScale += new Vector3(1 / 50f, 1 / 50f, 1 / 50f);
            yield return new WaitForSeconds(1 / 100f);
        }
        nextMenu.transform.localScale = Vector3.one;
        waitingForMenuAnim = false;
    }

    IEnumerator SwitchMenuAnim(GameObject prevMenu, GameObject nextMenu)
    {
        for (int frame = 0; frame < 50; frame++)
        {
            prevMenu.transform.localScale -= new Vector3(1 / 50f, 1 / 50f, 1 / 50f);
            yield return new WaitForSeconds(1 / 100f);
        }
        prevMenu.transform.localScale = Vector3.zero;
        prevMenu.SetActive(false);
        nextMenu.SetActive(true);
        for (int frame = 0; frame < 50; frame++)
        {
            nextMenu.transform.localScale += new Vector3(1 / 50f, 1 / 50f, 1 / 50f);
            yield return new WaitForSeconds(1 / 100f);
        }
        nextMenu.transform.localScale = Vector3.one;
        waitingForMenuAnim = false;
    }

    public void StartGame()
    {
        if (!waitingForMenuAnim)
        {
            waitingForMenuAnim = true;
            StartCoroutine(SwitchMenuAnim(mainMenu, songMenu));
        }
    }

    public void ViewCredits()
    {
        if (!waitingForMenuAnim)
        {
            waitingForMenuAnim = true;
            StartCoroutine(SwitchMenuAnim(mainMenu, creditsMenu));
        }
    }

    public void ViewOptions()
    {
        if (!waitingForMenuAnim)
        {
            waitingForMenuAnim = true;
            optionsMenu.SetActive(true);
            optionVolSlider.value = songManager.GetComponent<AudioSource>().volume;
            StartCoroutine(SwitchMenuAnim(mainMenu, optionsMenu));
        }
    }

    public void OptionsToMain()
    {
        if (!waitingForMenuAnim)
        {
            waitingForMenuAnim = true;
            StartCoroutine(SwitchMenuAnim(optionsMenu, mainMenu));
        }
    }

    public void CreditsToMain()
    {
        if (!waitingForMenuAnim)
        {
            waitingForMenuAnim = true;
            StartCoroutine(SwitchMenuAnim(creditsMenu, mainMenu));
        }
    }

    public void SongToMain()
    {
        if (!waitingForMenuAnim)
        {
            waitingForMenuAnim = true;
            StartCoroutine(SwitchMenuAnim(songMenu, mainMenu));
        }
    }

    public void ShowTutStats()
    {
        if (!waitingForMenuAnim)
        {
            storedSongMode = songManager.GetSongMode();
            songManager.SetSongMode(0);
            lavInfo.SetActive(false);
            bDTInfo.SetActive(false);
            smInd.SetActive(false);
            sdInd.SetActive(false);
            tutInfo.SetActive(true);
            songManager.SetSongTut();
        }
    }

    public void ShowLavStats()
    {
        if (!waitingForMenuAnim)
        {
            if (storedSongMode > -1)
            {
                songManager.SetSongMode(storedSongMode);
            }
            tutInfo.SetActive(false);
            bDTInfo.SetActive(false);
            lavInfo.SetActive(true);
            smInd.SetActive(true);
            sdInd.SetActive(true);
            songManager.SetSongLav();
        }
    }

    public void ShowBDTStats()
    {
        if (!waitingForMenuAnim)
        {
            if (storedSongMode > -1)
            {
                songManager.SetSongMode(storedSongMode);
            }
            tutInfo.SetActive(false);
            lavInfo.SetActive(false);
            bDTInfo.SetActive(true);
            smInd.SetActive(true);
            sdInd.SetActive(true);
            songManager.SetSongBDT();
        }
    }

    public void StartSong()
    {
        if (!waitingForMenuAnim)
        {
            waitingForMenuAnim = true;
            startingSong = true;
            StartCoroutine(ShrinkMenuAnim(songMenu));
        }
    }

    public bool InMenuAnim()
    {
        return waitingForMenuAnim;
    }

    public void PauseSong()
    {
        if (!waitingForMenuAnim)
        {
            waitingForMenuAnim = true;
            pauseMenu.SetActive(true);
            pauseVolSlider.value = songManager.GetComponent<AudioSource>().volume;
            StartCoroutine(ExpandMenuAnim(pauseMenu));
        }
    }

    public void UnpauseSong()
    {
        if (!waitingForMenuAnim)
        {
            waitingForMenuAnim = true;
            unpausingSong = true;
            StartCoroutine(ShrinkMenuAnim(pauseMenu));
        }
    }

    public void RestartSong()
    {
        if (!waitingForMenuAnim)
        {
            waitingForMenuAnim = true;
            restartingSong = true;
            StartCoroutine(ShrinkMenuAnim(pauseMenu));
        }
    }

    public void PauseToSong()
    {
        if (!waitingForMenuAnim)
        {
            waitingForMenuAnim = true;
            StartCoroutine(SwitchMenuAnim(pauseMenu, songMenu));
        }
    }

    public void ActivateEndMenu()
    {
        endMenu.SetActive(true);
    }

    public void EndSongMenu()
    {
        if (!waitingForMenuAnim)
        {
            waitingForMenuAnim = true;
            StartCoroutine(ExpandMenuAnim(endMenu));
        }
    }

    public void RestartSongFromEnd()
    {
        if (!waitingForMenuAnim)
        {
            waitingForMenuAnim = true;
            restartingSong = true;
            StartCoroutine(ShrinkMenuAnim(endMenu));
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void EndToSong()
    {
        if (!waitingForMenuAnim)
        {
            waitingForMenuAnim = true;
            StartCoroutine(SwitchMenuAnim(endMenu, songMenu));
        }
    }

    public void MoveSMInd(int modeIn)
    {
        storedSongMode = (modeIn + 1) % 3;
        smInd.GetComponent<RectTransform>().localPosition = new Vector3(indXLocalPos[modeIn],
                                                                        smInd.GetComponent<RectTransform>().localPosition.y,
                                                                        smInd.GetComponent<RectTransform>().localPosition.z);
    }

    public void MoveSDInd(int diffIn)
    {
        sdInd.GetComponent<RectTransform>().localPosition = new Vector3(indXLocalPos[diffIn],
                                                                        sdInd.GetComponent<RectTransform>().localPosition.y,
                                                                        sdInd.GetComponent<RectTransform>().localPosition.z);
    }

    public void UpdateStatsText(int songDur, int noteCount, int songIndex, int songDiff, int songMode)
    {
        durText.text = songDur / 60 + ":" + songDur % 60;
        countText.text = "" + noteCount;

        if (songMode == -1 && storedSongMode == -1)
        {
            if (PlayerPrefs.GetInt((("leastMisses" + songIndex) + songDiff) + songManager.GetSongMode()) == int.MaxValue)
            {
                lmLabel.alignment = TMPro.TextAlignmentOptions.Left;
                lmLabel.text = "Least\nMisses:";
                missText.text = "n/a";
            }
            else if (PlayerPrefs.GetInt((("leastMisses" + songIndex) + songDiff) + songManager.GetSongMode()) == 0)
            {
                lmLabel.alignment = TMPro.TextAlignmentOptions.Center;
                lmLabel.text = "Full Cleared!";
                missText.text = "";
            }
            else
            {
                lmLabel.alignment = TMPro.TextAlignmentOptions.Left;
                lmLabel.text = "Least\nMisses:";
                missText.text = "" + PlayerPrefs.GetInt((("leastMisses" + songIndex) + songDiff) + songManager.GetSongMode());
            }
            accText.text = (((float)((int)(PlayerPrefs.GetFloat((("highestAccuracy" + songIndex) + songDiff) + songManager.GetSongMode()) * 10000))) / 100) + "%";
            comboText.text = "" + PlayerPrefs.GetInt((("maxCombo" + songIndex) + songDiff) + songManager.GetSongMode());
        }
        else if (songMode == -1)
        {
            if (PlayerPrefs.GetInt((("leastMisses" + songIndex) + songDiff) + storedSongMode) == int.MaxValue)
            {
                lmLabel.alignment = TMPro.TextAlignmentOptions.Left;
                lmLabel.text = "Least\nMisses:";
                missText.text = "n/a";
            }
            else if (PlayerPrefs.GetInt((("leastMisses" + songIndex) + songDiff) + storedSongMode) == 0)
            {
                lmLabel.alignment = TMPro.TextAlignmentOptions.Center;
                lmLabel.text = "Full Cleared!";
                missText.text = "";
            }
            else
            {
                lmLabel.alignment = TMPro.TextAlignmentOptions.Left;
                lmLabel.text = "Least\nMisses:";
                missText.text = "" + PlayerPrefs.GetInt((("leastMisses" + songIndex) + songDiff) + storedSongMode);
            }
            accText.text = (((float)((int)(PlayerPrefs.GetFloat((("highestAccuracy" + songIndex) + songDiff) + storedSongMode) * 10000))) / 100) + "%";
            comboText.text = "" + PlayerPrefs.GetInt((("maxCombo" + songIndex) + songDiff) + storedSongMode);
        }
        else
        {
            if (PlayerPrefs.GetInt((("leastMisses" + songIndex) + songDiff) + songMode) == int.MaxValue)
            {
                lmLabel.alignment = TMPro.TextAlignmentOptions.Left;
                lmLabel.text = "Least\nMisses:";
                missText.text = "n/a";
            }
            else if (PlayerPrefs.GetInt((("leastMisses" + songIndex) + songDiff) + songMode) == 0)
            {
                lmLabel.alignment = TMPro.TextAlignmentOptions.Center;
                lmLabel.text = "Full Cleared!";
                missText.text = "";
            }
            else
            {
                lmLabel.alignment = TMPro.TextAlignmentOptions.Left;
                lmLabel.text = "Least\nMisses:";
                missText.text = "" + PlayerPrefs.GetInt((("leastMisses" + songIndex) + songDiff) + songMode);
            }
            accText.text = (((float)((int)(PlayerPrefs.GetFloat((("highestAccuracy" + songIndex) + songDiff) + storedSongMode) * 10000))) / 100) + "%";
            comboText.text = "" + PlayerPrefs.GetInt((("maxCombo" + songIndex) + songDiff) + songMode);
        }
    }
}
