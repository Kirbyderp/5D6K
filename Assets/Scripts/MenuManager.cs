using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private GameObject mainMenu, songMenu, pauseMenu, creditsMenu, optionsMenu, endMenu;
    private GameObject tutInfo, lavInfo, bDTInfo;
    private Slider optionVolSlider, pauseVolSlider;
    private SongManager songManager;
    private bool waitingForMenuAnim = false, startingSong = false, unpausingSong = false, restartingSong = false;

    // Start is called before the first frame update
    void Start()
    {
        songManager = GameObject.Find("SongManager").GetComponent<SongManager>();
        songManager.SetColorManager(GameObject.Find("Color Canvas").GetComponent<ColorManager>());
        songManager.SetStatsSummary(GameObject.Find("Song Stats Summary Text").GetComponent<TMPro.TextMeshProUGUI>());
        tutInfo = GameObject.Find("Tutorial Info Menu");
        lavInfo = GameObject.Find("Lavender Info Menu");
        bDTInfo = GameObject.Find("Dream Team Info Menu");
        optionVolSlider = GameObject.Find("Options Volume Slider").GetComponent<Slider>();
        pauseVolSlider = GameObject.Find("Pause Volume Slider").GetComponent<Slider>();

        mainMenu = GameObject.Find("Main Menu");
        songMenu = GameObject.Find("Song Menu");
        pauseMenu = GameObject.Find("Pause Menu");
        creditsMenu = GameObject.Find("Credits Menu");
        optionsMenu = GameObject.Find("Options Menu");
        endMenu = GameObject.Find("End Song Menu");
        songMenu.transform.localScale = Vector3.zero;
        songMenu.SetActive(false);
        pauseMenu.transform.localScale = Vector3.zero;
        pauseMenu.SetActive(false);
        creditsMenu.transform.localScale = Vector3.zero;
        creditsMenu.SetActive(false);
        optionsMenu.transform.localScale = Vector3.zero;
        optionsMenu.SetActive(false);
        endMenu.transform.localScale = Vector3.zero;
        endMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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
            lavInfo.SetActive(false);
            bDTInfo.SetActive(false);
            tutInfo.SetActive(true);
            songManager.SetSongTut();
        }
    }

    public void ShowLavStats()
    {
        if (!waitingForMenuAnim)
        {
            tutInfo.SetActive(false);
            bDTInfo.SetActive(false);
            lavInfo.SetActive(true);
            songManager.SetSongLav();
        }
    }

    public void ShowBDTStats()
    {
        if (!waitingForMenuAnim)
        {
            tutInfo.SetActive(false);
            lavInfo.SetActive(false);
            bDTInfo.SetActive(true);
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

    public void EndToSong()
    {
        if (!waitingForMenuAnim)
        {
            waitingForMenuAnim = true;
            StartCoroutine(SwitchMenuAnim(endMenu, songMenu));
        }
    }
}
