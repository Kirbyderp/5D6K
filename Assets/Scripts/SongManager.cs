using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class SongManager : MonoBehaviour
{
    public static readonly string[,] songPaths = { { "Assets/Songs/Bowsers Dream Team Easy.txt", "Assets/Songs/Lavender Cemetery Easy.txt", "Assets/Songs/Tutorial.txt", "Assets/Songs/TestSong.txt" },
                                                   { "Assets/Songs/Bowsers Dream Team Medium.txt", "Assets/Songs/Lavender Cemetery Medium.txt", "Assets/Songs/Tutorial.txt", "Assets/Songs/TestSong.txt" },
                                                   { "Assets/Songs/Bowsers Dream Team Hard.txt", "Assets/Songs/Lavender Cemetery Hard.txt", "Assets/Songs/Tutorial.txt", "Assets/Songs/TestSong.txt"} };
    public static readonly string[] audioPaths = { "Audio/BowsersDreamTeam", "Audio/LavenderCemetery", "Audio/Tutorial", "Audio/TestSong" };
    private static readonly int[] songDurs = { 90, 88, 74 }; //songDurs[curSongIndex]
    private static readonly int[,,] notesInSong = new int[3, 3, 3] { { { 200, 78, 122 }, { 554, 199, 355 }, { 964, 521, 443 } },
                                                                     { { 107, 43, 64 }, { 184, 65, 119 }, { 272, 147, 125 } },
                                                                     { { 90, 55, 35 }, { 90, 55, 35 }, { 90, 55, 35 } } };
                                                                   //notesInSong[curSongIndex, songDiff, songMode]
    private Song curSong;
    private int curSongIndex = 2;
    private bool readyToPlay = false, isSongPlaying = false, isSongPaused = false, tutPause = false;
    private float curTime;
    private int curSec = 0;
    private float hitLeniency = .15f, hit3DLeniency = .25f, distance3DLeniency = .331f;
    private Track2D[] all2DTracks;
    private Track3D[] all3DTracks;
    public GameObject[] note2DObjects, note2DHolds, note3DObjects;
    private GameObject[] spawned2DNotes, spawned3DNotes;
    private int note2DSpawnIndex, note3DSpawnIndex;
    private List<GameObject>[] track2DHoldObjects;
    private float spawn2DBeatsInAdvance = 3.5f, spawn3DBeatsInAdvance = 3;
    private Note2D[] note2Ds;
    private Note3D[] note3Ds;
    private AudioClip songAudio;
    private AudioSource audioSource;
    private ColorManager colorManager;
    private MenuManager menuManager;
    private TutorialManager tutManager;

    private bool pressingLGrip = false;
    private bool pressingLTrigger = false;
    private bool pressingRTrigger = false;
    private bool pressingRGrip = false;
    private bool hasUnpressedMenu = true;

    private KeyCode track1Hit = KeyCode.D;
    private KeyCode track2Hit = KeyCode.F;
    private KeyCode track3Hit = KeyCode.H;
    private KeyCode track4Hit = KeyCode.J;
    
    //Controller Input and Feedback Vars
    public InputDevice leftHand, rightHand;
    public XRBaseControllerInteractor leftHaptics, rightHaptics;
    private float leftHapticCooldown = 0, rightHapticCooldown = 0;
    private int leftRapidHapticCount = 0, rightRapidHapticCount = 0;
    private bool usingHaptics = true;
    private GameObject hapticsCheckmark;

    //Song Stat Vars
    private int hit2Dcount, hit3Dcount;
    private int curCombo, maxCombo;
    private int numMisses;
    private float accuracy;
    private int accuracyWeight;
    private TMPro.TextMeshProUGUI statsSummary;
    private TMPro.TextMeshProUGUI iSCombo, iSAcc, iSNotes;

    //DEBUG VARS
    //public GameObject[] outlines;
    //public GameObject leftHandSphere, rightHandSphere;

    //0 = Easy, Both; 1 = Medium, 2D Only; 2 = Hard, 3D Only
    private int songDiff = 0, songMode = 0;

    private bool twoDebug = false;
    //public Image colorTest;

    // Start is called before the first frame update
    void Start()
    {
        hapticsCheckmark = GameObject.Find("Haptics Checkmark");
        if (!PlayerPrefs.HasKey("UsingHaptics"))
        {
            PlayerPrefs.SetInt("UsingHaptics", 1);
        }
        else if (PlayerPrefs.GetInt("UsingHaptics") == 0)
        {
            hapticsCheckmark.SetActive(false);
            usingHaptics = false;
        }
        menuManager = GameObject.Find("Menu Canvas").GetComponent<MenuManager>();
        menuManager.SongManHasSetUp();
        tutManager = GameObject.Find("Tutorial Canvas").GetComponent<TutorialManager>();
        audioSource = GetComponent<AudioSource>();
        LoadSong(curSongIndex);
        all2DTracks = new Track2D[4] { GameObject.Find("Track2D1").GetComponent<Track2D>(),
                                       GameObject.Find("Track2D2").GetComponent<Track2D>(),
                                       GameObject.Find("Track2D3").GetComponent<Track2D>(),
                                       GameObject.Find("Track2D4").GetComponent<Track2D>()};
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < all2DTracks[i].transform.childCount; j++)
            {
                if (all2DTracks[i].transform.GetChild(j).gameObject.CompareTag("AnimRing"))
                {
                    all2DTracks[i].SetAnimRing(all2DTracks[i].transform.GetChild(j).gameObject.GetComponent<Image>());
                    break;
                }
            }
        }
        all3DTracks = new Track3D[2] { GameObject.Find("Track3D5").GetComponent<Track3D>(),
                                       GameObject.Find("Track3D6").GetComponent<Track3D>()};
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    if (!PlayerPrefs.HasKey((("bestHit2Dcount" + i) + j) + k))
                    {
                        PlayerPrefs.SetInt((("bestHit2Dcount" + i) + j) + k, 0);
                    }
                    if (!PlayerPrefs.HasKey((("bestHit3Dcount" + i) + j) + k))
                    {
                        PlayerPrefs.SetInt((("bestHit3Dcount" + i) + j) + k, 0);
                    }
                    if (!PlayerPrefs.HasKey((("maxCombo" + i) + j) + k))
                    {
                        PlayerPrefs.SetInt((("maxCombo" + i) + j) + k, 0);
                    }
                    if (!PlayerPrefs.HasKey((("highestAccuracy" + i) + j) + k))
                    {
                        PlayerPrefs.SetFloat((("highestAccuracy" + i) + j) + k, 0);
                    }
                    if (!PlayerPrefs.HasKey((("leastMisses" + i) + j) + k))
                    {
                        PlayerPrefs.SetInt((("leastMisses" + i) + j) + k, int.MaxValue);
                    }
                }
            }
        }
        iSAcc = GameObject.Find("In Song Accuracy Text").GetComponent<TMPro.TextMeshProUGUI>();
        iSCombo = GameObject.Find("In Song Combo Text").GetComponent<TMPro.TextMeshProUGUI>();
        iSNotes = GameObject.Find("In Song Notes Hit Text").GetComponent<TMPro.TextMeshProUGUI>();

        /*outlines = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            outlines[i] = GameObject.Find("Outline" + (i + 1));
            outlines[i].GetComponent<Image>().color = Color.red;
        }*/

        if (twoDebug)
        {
            songDiff = 0;
            LoadSong(curSongIndex);
            StartCoroutine(Test());
        }
    }

    public void SetColorManager(ColorManager managerIn)
    {
        colorManager = managerIn;
    }

    public void SetStatsSummary(TMPro.TextMeshProUGUI textIn)
    {
        statsSummary = textIn;
    }

    public int GetSongMode()
    {
        return songMode;
    }

    public void SetSongMode(int modeIn)
    {
        songMode = modeIn;
    }

    IEnumerator Test()
    {
        yield return new WaitForSeconds(2);
        PlaySong();
    }

    // Update is called once per frame
    void Update()
    {
        //Make sure controllers are valid so we can read input from them
        if (!leftHand.isValid)
        {
            List<InputDevice> deviceList = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller |
                                                       InputDeviceCharacteristics.Left, deviceList);
            if (deviceList.Count > 0)
            {
                leftHand = deviceList[0];
            }
        }
        if (!rightHand.isValid)
        {
            List<InputDevice> deviceList = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller |
                                                       InputDeviceCharacteristics.Right, deviceList);
            if (deviceList.Count > 0)
            {
                rightHand = deviceList[0];
            }
        }

        if (isSongPlaying)
        {
            leftHand.TryGetFeatureValue(CommonUsages.menuButton, out bool isPressingMenu);
            if (isPressingMenu && hasUnpressedMenu)
            {
                hasUnpressedMenu = false;
                if (!menuManager.InMenuAnim())
                {
                    if (!isSongPaused)
                    {
                        audioSource.Pause();
                        isSongPaused = true;
                        menuManager.PauseSong();
                    }
                    else
                    {
                        menuManager.UnpauseSong();
                    }
                }
            }
            else if (!isPressingMenu && !hasUnpressedMenu)
            {
                hasUnpressedMenu = true;
            }
        }

        if (isSongPlaying && curSongIndex == 2 && curTime > tutManager.GetNextPauseTime() && !tutPause)
        {
            audioSource.Pause();
            tutPause = true;
            tutManager.ShowTutMenu();
        }

        if (isSongPlaying && !isSongPaused && !tutPause)
        {
            //Store the time that has advanced to keep it consistent throughout Update
            float deltaTime = Time.deltaTime;

            if (leftHapticCooldown > 0)
            {
                leftHapticCooldown -= deltaTime;
            }
            else if (leftRapidHapticCount > 0)
            {
                leftRapidHapticCount /= 2;
                leftHapticCooldown = .1f;
            }

            if (rightHapticCooldown > 0)
            {
                rightHapticCooldown -= deltaTime;
            }
            else if (rightRapidHapticCount > 0)
            {
                rightRapidHapticCount /= 2;
                rightHapticCooldown = .1f;
            }

            if (songMode != 2)
            {
                //Move all spawned notes down
                foreach (Track2D track in all2DTracks)
                {
                    track.MoveNotesDown(deltaTime / (curSong.GetBeatLength() * spawn2DBeatsInAdvance)
                                        * (track.gameObject.GetComponent<RectTransform>().sizeDelta.y));
                }
            }

            if (songMode != 1)
            {
                foreach (Track3D track in all3DTracks)
                {
                    track.MoveNotesForwards(deltaTime / (curSong.GetBeatLength() * spawn3DBeatsInAdvance), note3Ds);
                }
            }

            if (songMode != 2)
            {
                //Check if user pressed or released any buttons down for 2D Notes
                leftHand.TryGetFeatureValue(CommonUsages.gripButton, out bool curLGrip);
                leftHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool curLTrigger);
                rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool curRTrigger);
                rightHand.TryGetFeatureValue(CommonUsages.gripButton, out bool curRGrip);

                //Track 1
                if (curLGrip == true && pressingLGrip == false)
                {
                    HitTrack(1);
                    pressingLGrip = true;
                    //outlines[0].GetComponent<Image>().color = Color.blue;
                }
                else if (curLGrip == false && pressingLGrip == true)
                {
                    ReleaseTrack(1);
                    pressingLGrip = false;
                    //outlines[0].GetComponent<Image>().color = Color.red;
                }

                //Track 2
                if (curLTrigger == true && pressingLTrigger == false)
                {
                    HitTrack(2);
                    pressingLTrigger = true;
                    //outlines[1].GetComponent<Image>().color = Color.blue;
                }
                else if (curLTrigger == false && pressingLTrigger == true)
                {
                    ReleaseTrack(2);
                    pressingLTrigger = false;
                    //outlines[1].GetComponent<Image>().color = Color.red;
                }

                //Track 3
                if (curRTrigger == true && pressingRTrigger == false)
                {
                    HitTrack(3);
                    pressingRTrigger = true;
                    //outlines[2].GetComponent<Image>().color = Color.blue;
                }
                else if (curRTrigger == false && pressingRTrigger == true)
                {
                    ReleaseTrack(3);
                    pressingRTrigger = false;
                    //outlines[2].GetComponent<Image>().color = Color.red;
                }

                //Track 4
                if (curRGrip == true && pressingRGrip == false)
                {
                    HitTrack(4);
                    pressingRGrip = true;
                    //outlines[3].GetComponent<Image>().color = Color.blue;
                }
                else if (curRGrip == false && pressingRGrip == true)
                {
                    ReleaseTrack(4);
                    pressingRGrip = false;
                    //outlines[3].GetComponent<Image>().color = Color.red;
                }
            }

            if (songMode != 1)
            {
                //Check where the user's hands are for 3D notes
                leftHand.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftHandPos);
                rightHand.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightHandPos);
                //leftHandSphere.transform.position = new Vector3(-leftHandPos.z, leftHandPos.y, leftHandPos.x);
                //rightHandSphere.transform.position = new Vector3(-rightHandPos.z, rightHandPos.y, rightHandPos.x);
                Vector3 leftHandPosFixed = new Vector3(-leftHandPos.z, leftHandPos.y, leftHandPos.x);
                Vector3 rightHandPosFixed = new Vector3(-rightHandPos.z, rightHandPos.y, rightHandPos.x);

                //Track 5
                Hit3DNotes(5, leftHandPosFixed);

                //Track 6
                Hit3DNotes(6, rightHandPosFixed);
            }

            //DEBUG KEYBOARD 2D INPUTS
            if (Input.GetKeyDown(track1Hit))
            {
                HitTrack(1);
            }
            if (Input.GetKeyDown(track2Hit))
            {
                HitTrack(2);
            }
            if (Input.GetKeyDown(track3Hit))
            {
                HitTrack(3);
            }
            if (Input.GetKeyDown(track4Hit))
            {
                HitTrack(4);
            }

            if (Input.GetKeyUp(track1Hit))
            {
                ReleaseTrack(1);
            }
            if (Input.GetKeyUp(track2Hit))
            {
                ReleaseTrack(2);
            }
            if (Input.GetKeyUp(track3Hit))
            {
                ReleaseTrack(3);
            }
            if (Input.GetKeyUp(track4Hit))
            {
                ReleaseTrack(4);
            }

            if (songMode != 2)
            {
                //Spawn and despawn 2D notes based on the time that has elapsed
                foreach (Note2D note in note2Ds)
                {
                    //Despawning Notes
                    if (!note.WasHit() && (curTime - hitLeniency) > note.GetTime())
                    {
                        Destroy(spawned2DNotes[note.GetSpawnIndex()]);
                        note.Hit();
                        MissNote();
                        if (note.GetNoteType() == 1)
                        {
                            if (track2DHoldObjects[note.GetTrackNum() - 1].Count > 0)
                            {
                                track2DHoldObjects[note.GetTrackNum() - 1][0].GetComponent<Image>().color = new Color(track2DHoldObjects[note.GetTrackNum() - 1][0].GetComponent<Image>().color.r / 2,
                                                                                                                      track2DHoldObjects[note.GetTrackNum() - 1][0].GetComponent<Image>().color.g / 2,
                                                                                                                      track2DHoldObjects[note.GetTrackNum() - 1][0].GetComponent<Image>().color.b / 2);
                            }
                        }
                        if (note.GetNoteType() == 2)
                        {
                            all2DTracks[note.GetTrackNum() - 1].SetIsHolding(false);
                            all2DTracks[note.GetTrackNum() - 1].StopHoldingAnim();
                            if (track2DHoldObjects[note.GetTrackNum() - 1].Count > 0)
                            {
                                GameObject curHold = track2DHoldObjects[note.GetTrackNum() - 1][0];
                                track2DHoldObjects[note.GetTrackNum() - 1].RemoveAt(0);
                                Destroy(curHold);
                            }
                        }
                        //Debug.Log("Despawned");
                    }

                    //Spawning Notes
                    if (!note.HasSpawned() && (curTime + spawn2DBeatsInAdvance * curSong.GetBeatLength()) > note.GetTime())
                    {
                        if (note.GetNoteType() == 1)
                        {
                            Note2D endNote = FindHoldEnd(note);
                            GameObject curHold = Instantiate(note2DHolds[note.GetTrackNum() - 1], all2DTracks[note.GetTrackNum() - 1].transform);
                            spawned2DNotes[note2DSpawnIndex] = Instantiate(note2DObjects[note.GetTrackNum() - 1],
                                                                           all2DTracks[note.GetTrackNum() - 1].transform);
                            GameObject dispNoteChild = spawned2DNotes[note2DSpawnIndex].transform.GetChild(0).gameObject;
                            dispNoteChild.GetComponent<Image>().color = new Color(PlayerPrefs.GetFloat("Track " + note.GetTrackNum() + " R"),
                                                                                  PlayerPrefs.GetFloat("Track " + note.GetTrackNum() + " G"),
                                                                                  PlayerPrefs.GetFloat("Track " + note.GetTrackNum() + " B"));
                            float beatDif = (endNote.GetTime() - note.GetTime()) / curSong.GetBeatLength();
                            curHold.GetComponent<RectTransform>().localPosition = new Vector3(curHold.GetComponent<RectTransform>().localPosition.x,
                                                                                              spawned2DNotes[note2DSpawnIndex].GetComponent<RectTransform>().localPosition.y + beatDif / 2,
                                                                                              curHold.GetComponent<RectTransform>().localPosition.z);
                            curHold.GetComponent<RectTransform>().sizeDelta = new Vector2(curHold.GetComponent<RectTransform>().sizeDelta.x, beatDif);
                            curHold.GetComponent<Image>().color = new Color(PlayerPrefs.GetFloat("Track " + note.GetTrackNum() + " R"),
                                                                            PlayerPrefs.GetFloat("Track " + note.GetTrackNum() + " G"),
                                                                            PlayerPrefs.GetFloat("Track " + note.GetTrackNum() + " B"));
                            track2DHoldObjects[note.GetTrackNum() - 1].Add(curHold);
                        }
                        else
                        {
                            spawned2DNotes[note2DSpawnIndex] = Instantiate(note2DObjects[note.GetTrackNum() - 1],
                                                                           all2DTracks[note.GetTrackNum() - 1].transform);
                            GameObject dispNoteChild = spawned2DNotes[note2DSpawnIndex].transform.GetChild(0).gameObject;
                            dispNoteChild.GetComponent<Image>().color = new Color(PlayerPrefs.GetFloat("Track " + note.GetTrackNum() + " R"),
                                                                                  PlayerPrefs.GetFloat("Track " + note.GetTrackNum() + " G"),
                                                                                  PlayerPrefs.GetFloat("Track " + note.GetTrackNum() + " B"));
                        }
                        
                        note.Spawn();
                        note.SetSpawnIndex(note2DSpawnIndex);
                        note2DSpawnIndex++;
                        //Debug.Log("Spawned");
                    }
                    else if (!note.HasSpawned())
                    {
                        break;
                    }
                }
            }

            if (songMode != 1)
            {
                //Spawn and despawn 3D notes based on the time that has elapsed
                foreach (Note3D note in note3Ds)
                {
                    //Despawning Notes
                    if (!note.WasHit() && (curTime - hit3DLeniency) > note.GetTime())
                    {
                        Destroy(spawned3DNotes[note.GetSpawnIndex()]);
                        note.Hit();
                        MissNote();
                        //Debug.Log("Despawned");
                    }

                    //Spawning Notes
                    if (!note.HasSpawned() && (curTime + spawn3DBeatsInAdvance * curSong.GetBeatLength()) > note.GetTime())
                    {
                        spawned3DNotes[note3DSpawnIndex] = Instantiate(note3DObjects[note.GetNoteType()],
                                                                       note.GetStartingPos(), Quaternion.identity,
                                                                       all3DTracks[note.GetTrackNum() - 5].transform);
                        if (note.GetTrackNum() == 5)
                        {
                            spawned3DNotes[note3DSpawnIndex].GetComponent<MeshRenderer>().material = colorManager.GetLeft3DMat();
                        }
                        else
                        {
                            spawned3DNotes[note3DSpawnIndex].GetComponent<MeshRenderer>().material = colorManager.GetRight3DMat();
                        }
                        note.Spawn();
                        note.SetSpawnIndex(note3DSpawnIndex);
                        spawned3DNotes[note3DSpawnIndex].GetComponent<Note3DID>().SetNoteID(note3DSpawnIndex);
                        note3DSpawnIndex++;
                        //Debug.Log("Spawned");
                    }
                    else if (!note.HasSpawned())
                    {
                        break;
                    }
                }
            }

            if (curTime >= curSong.GetLength())
            {
                EndSong();
            }

            curTime += deltaTime;

            //Debug Stuff
            if (curTime - 1 > curSec)
            {
                //Debug.Log(++curSec);
            }
            if (curTime < 5.1f)
            {
                //Debug.Log(curTime);
            }

            iSAcc.text = "Accuracy:\n" + (((float)((int)(accuracy * 10000))) / 100) + "%";
            iSCombo.text = "Combo:\n" + curCombo;
            iSNotes.text = "Notes Hit:\n" + (hit2Dcount + hit3Dcount) + " / " + ((songMode != 2 ? note2Ds.Length : 0)
                                                                              + (songMode != 1 ? note3Ds.Length : 0));
        }
    }

    private Note2D FindHoldEnd(Note2D startNote)
    {
        foreach (Note2D note in note2Ds)
        {
            if (note.GetTrackNum() == startNote.GetTrackNum() && note.GetNoteType() == 2 && !note.WasFound())
            {
                note.Find();
                return note;
            }
        }
        throw new System.Exception("Could not find end note.");
    }

    public void LoadSong(int songIndex)
    {
        readyToPlay = false;
        curSong = new Song(songPaths[songDiff, songIndex]);
        curTime = 0;
        note2DSpawnIndex = 0;
        note3DSpawnIndex = 0;
        note2Ds = curSong.GetAll2DNotes();
        note3Ds = curSong.GetAll3DNotes();
        spawned2DNotes = new GameObject[curSong.GetAll2DNotes().Length];
        spawned3DNotes = new GameObject[curSong.GetAll3DNotes().Length];
        track2DHoldObjects = new List<GameObject>[4];
        for (int i = 0; i < 4; i++)
        {
            track2DHoldObjects[i] = new List<GameObject>();
        }
        songAudio = Resources.Load<AudioClip>(audioPaths[songIndex]);
        audioSource.clip = songAudio;
        float curVol = audioSource.volume;
        audioSource.volume = 0;
        audioSource.Play();
        audioSource.Stop();
        audioSource.volume = curVol;

        hit2Dcount = 0;
        hit3Dcount = 0;
        curCombo = 0;
        maxCombo = 0;
        numMisses = 0;
        accuracy = 0;
        accuracyWeight = 0;

        tutManager.ResetNextPauseTimeIndex();

        leftHapticCooldown = 0;
        rightHapticCooldown = 0;
        leftRapidHapticCount = 0;
        rightRapidHapticCount = 0;

        readyToPlay = true;
    }

    public void PlaySong()
    {
        if (!isSongPlaying && readyToPlay)
        {
            isSongPlaying = true;
            isSongPaused = false;
            tutPause = false;
            audioSource.Play();
        }
    }

    public void EndSong()
    {
        isSongPlaying = false;
        audioSource.Stop();
        menuManager.ActivateEndMenu();

        //Update End Menu Stats
        statsSummary.text = hit2Dcount + " / " + note2Ds.Length + "\n"
                          + hit3Dcount + " / " + note3Ds.Length + "\n"
                          + (hit2Dcount + hit3Dcount) + " / " + (note2Ds.Length + note3Ds.Length) + "\n"
                          + maxCombo + "\n"
                          + (numMisses == 0 ? "Full Clear!" : numMisses) + "\n"
                          + (((float)((int)(accuracy * 10000))) / 100) + "%";

        //Update Player Prefs - UNCOMMENT FOR FINAL BUILD
        /*
        if (PlayerPrefs.GetInt((("bestHit2Dcount" + curSongIndex) + songDiff) + songMode) < hit2Dcount)
        {
            PlayerPrefs.SetInt((("bestHit2Dcount" + curSongIndex) + songDiff) + songMode, hit2Dcount);
        }
        if (PlayerPrefs.GetInt((("bestHit3Dcount" + curSongIndex) + songDiff) + songMode) < hit3Dcount)
        {
            PlayerPrefs.SetInt((("bestHit3Dcount" + curSongIndex) + songDiff) + songMode, hit3Dcount);
        }
        if (PlayerPrefs.GetInt((("maxCombo" + curSongIndex) + songDiff) + songMode) < maxCombo)
        {
            PlayerPrefs.SetInt((("maxCombo" + curSongIndex) + songDiff) + songMode, maxCombo);
        }
        if (PlayerPrefs.GetFloat((("highestAccuracy" + curSongIndex) + songDiff) + songMode) < accuracy)
        {
            PlayerPrefs.SetFloat((("highestAccuracy" + curSongIndex) + songDiff) + songMode, accuracy);
        }
        if (PlayerPrefs.GetInt((("leastMisses" + curSongIndex) + songDiff) + songMode) > numMisses)
        {
            PlayerPrefs.SetInt((("leastMisses" + curSongIndex) + songDiff) + songMode, numMisses);
        }*/

        menuManager.EndSongMenu();
    }

    //Needed for when a song is ended mid-song
    private void DespawnAllNotes()
    {
        foreach (Note2D note in note2Ds)
        {
            if (!note.WasHit() && note.HasSpawned())
            {
                Destroy(spawned2DNotes[note.GetSpawnIndex()]);
                if (note.GetNoteType() == 1)
                {
                    if (track2DHoldObjects[note.GetTrackNum() - 1].Count > 0)
                    {
                        GameObject curHold = track2DHoldObjects[note.GetTrackNum() - 1][0];
                        track2DHoldObjects[note.GetTrackNum() - 1].RemoveAt(0);
                        Destroy(curHold);
                    }
                }
            }
        }

        for (int i = 0; i < 4; i++)
        {
            while (track2DHoldObjects[i].Count > 0)
            {
                GameObject curHold = track2DHoldObjects[i][0];
                track2DHoldObjects[i].RemoveAt(0);
                Destroy(curHold);
            }
        }

        foreach (Note3D note in note3Ds)
        {
            if (!note.WasHit() && note.HasSpawned())
            {
                Destroy(spawned3DNotes[note.GetSpawnIndex()]);
            }
        }
    }

    public void RestartSong()
    {
        isSongPlaying = false;
        audioSource.Stop();
        DespawnAllNotes();
        LoadSong(curSongIndex);
        menuManager.RestartSong();
        tutManager.EndTutPremature();
    }

    public void EndSongPremature()
    {
        isSongPlaying = false;
        audioSource.Stop();
        DespawnAllNotes();
        LoadSong(curSongIndex);
        menuManager.PauseToSong();
        tutManager.EndTutPremature();
    }

    public void RestartSongFromEnd()
    {
        LoadSong(curSongIndex);
        menuManager.RestartSongFromEnd();
    }

    public void ToSelectFromEnd()
    {
        LoadSong(curSongIndex);
        menuManager.EndToSong();
    }

    public void HitTrack(int trackNum)
    {
        foreach (Note2D note in note2Ds)
        {
            if (!note.WasHit() && note.GetTrackNum() == trackNum &&
                Mathf.Abs(curTime - note.GetTime()) < hitLeniency)
            {
                Destroy(spawned2DNotes[note.GetSpawnIndex()]);
                note.Hit();
                if (note.GetNoteType() == 0)
                {
                    all2DTracks[note.GetTrackNum() - 1].PlayHitAnim();
                }
                if (note.GetNoteType() == 1)
                {
                    all2DTracks[note.GetTrackNum() - 1].SetIsHolding(true);
                    all2DTracks[note.GetTrackNum() - 1].PlayHoldingAnim();
                }
                if (note.GetNoteType() == 2)
                {
                    if (track2DHoldObjects[note.GetTrackNum() - 1].Count > 0)
                    {
                        GameObject curHold = track2DHoldObjects[note.GetTrackNum() - 1][0];
                        track2DHoldObjects[note.GetTrackNum() - 1].RemoveAt(0);
                        Destroy(curHold);
                    }
                    all2DTracks[note.GetTrackNum() - 1].PlayHitAnim();
                }
                //Debug.Log("Note Hit!");
                HitNote(note.GetTime());
                hit2Dcount++;
                return;
            }
        }
        MissNote();
        //Debug.Log("Miss!");
    }

    public void ReleaseTrack(int trackNum)
    {
        foreach (Note2D note in note2Ds)
        {
            if (!note.WasHit() && note.GetTrackNum() == trackNum && note.HasSpawned() && note.GetNoteType() == 1)
            {
                return;
            }
            else if (!note.WasHit() && note.GetTrackNum() == trackNum && note.HasSpawned() &&
                     Mathf.Abs(curTime - note.GetTime()) < hitLeniency &&
                     note.GetNoteType() == 2)
            {
                Destroy(spawned2DNotes[note.GetSpawnIndex()]);
                note.Hit();
                all2DTracks[trackNum - 1].StopHoldingAnim();
                all2DTracks[trackNum - 1].PlayHitAnim();
                HitNote(note.GetTime());
                all2DTracks[trackNum - 1].SetIsHolding(false);
                hit2Dcount++;
                if (track2DHoldObjects[trackNum - 1].Count > 0)
                {
                    GameObject curHold = track2DHoldObjects[trackNum - 1][0];
                    track2DHoldObjects[trackNum - 1].RemoveAt(0);
                    Destroy(curHold);
                }
                //Debug.Log("Note Hit!");
                return;
            }
        }
        if (all2DTracks[trackNum - 1].IsHolding())
        {
            all2DTracks[trackNum - 1].SetIsHolding(false);
            all2DTracks[trackNum - 1].StopHoldingAnim();
            if (curCombo == maxCombo)
            {
                maxCombo--;
            }
            curCombo--;
            MissNote();
            if (track2DHoldObjects[trackNum - 1].Count > 0)
            {
                track2DHoldObjects[trackNum - 1][0].GetComponent<Image>().color = new Color(track2DHoldObjects[trackNum - 1][0].GetComponent<Image>().color.r / 2,
                                                                                            track2DHoldObjects[trackNum - 1][0].GetComponent<Image>().color.g / 2,
                                                                                            track2DHoldObjects[trackNum - 1][0].GetComponent<Image>().color.b / 2);
            }
            hit2Dcount--;
            //Debug.Log("Miss!");
        }
    }

    public void Hit3DNotes(int trackNum, Vector3 handPos)
    {
        foreach (Note3D note in note3Ds)
        {
            if (!note.WasHit() && note.GetTrackNum() == trackNum && note.HasSpawned() &&
                (handPos - spawned3DNotes[note.GetSpawnIndex()].transform.position).magnitude < distance3DLeniency)
            {
                Destroy(spawned3DNotes[note.GetSpawnIndex()]);
                note.Hit();
                if (Mathf.Abs(curTime - note.GetTime()) < hitLeniency)
                {
                    //Debug.Log("3d Note Hit!");
                    HitNote(note.GetTime());
                    hit3Dcount++;
                    if (usingHaptics)
                    {
                        if (trackNum == 5)
                        {
                            leftHaptics.SendHapticImpulse(leftRapidHapticCount < 10 ? .5f / Mathf.Pow(1.1f, leftRapidHapticCount) : .2f, .1f);
                            leftHapticCooldown = .5f;
                            leftRapidHapticCount++;
                        }
                        else if (trackNum == 6)
                        {
                            rightHaptics.SendHapticImpulse(leftRapidHapticCount < 10 ? .5f / Mathf.Pow(1.1f, leftRapidHapticCount) : .2f, .1f);
                            rightHapticCooldown = .5f;
                            rightRapidHapticCount++;
                        }
                    }
                }
                else
                {
                    //Debug.Log("Bad Timing!");
                    MissNote();
                }
                return;
            }
        }
    }

    private void HitNote(float noteHitTime)
    {
        curCombo++;
        if (curCombo > maxCombo)
        {
            maxCombo = curCombo;
        }
        //Note hit at the perfect time has an accuracy of 100%
        //Note hit at the worst possible time that still hits it has an accuracy of 60%
        accuracy = (accuracy * accuracyWeight +
                   .4f * (1 - Mathf.Abs(curTime - noteHitTime) / hitLeniency) + .6f)
                   / (accuracyWeight + 1);
        accuracyWeight++;
    }

    private void MissNote()
    {
        curCombo = 0;
        numMisses++;
        accuracy = (accuracy * accuracyWeight) / (accuracyWeight + 1);
        accuracyWeight++;
    }

    public void UnpauseSong()
    {
        if (!tutPause)
        {
            audioSource.Play();
        }
        isSongPaused = false;
    }

    public void SetDiffEasy()
    {
        if (!isSongPlaying)
        {
            songDiff = 0;
            menuManager.UpdateStatsText(songDurs[curSongIndex], notesInSong[curSongIndex, songDiff, songMode], curSongIndex, songDiff, songMode);
            LoadSong(curSongIndex);
        }
    }

    public void SetDiffMedium()
    {
        if (!isSongPlaying)
        {
            songDiff = 1;
            menuManager.UpdateStatsText(songDurs[curSongIndex], notesInSong[curSongIndex, songDiff, songMode], curSongIndex, songDiff, songMode);
            LoadSong(curSongIndex);
        }
    }

    public void SetDiffHard()
    {
        if (!isSongPlaying)
        {
            songDiff = 2;
            menuManager.UpdateStatsText(songDurs[curSongIndex], notesInSong[curSongIndex, songDiff, songMode], curSongIndex, songDiff, songMode);
            LoadSong(curSongIndex);
        }
    }

    public void SetModeBoth()
    {
        if (!isSongPlaying)
        {
            songMode = 0;
            menuManager.UpdateStatsText(songDurs[curSongIndex], notesInSong[curSongIndex, songDiff, songMode], curSongIndex, songDiff, songMode);
        }
    }

    public void SetMode2D()
    {
        if (!isSongPlaying)
        {
            songMode = 1;
            menuManager.UpdateStatsText(songDurs[curSongIndex], notesInSong[curSongIndex, songDiff, songMode], curSongIndex, songDiff, songMode);
        }
    }

    public void SetMode3D()
    {
        if (!isSongPlaying)
        {
            songMode = 2;
            menuManager.UpdateStatsText(songDurs[curSongIndex], notesInSong[curSongIndex, songDiff, songMode], curSongIndex, songDiff, songMode);
        }
    }

    public void SetSongTut()
    {
        curSongIndex = 2;
        menuManager.UpdateStatsText(songDurs[curSongIndex], notesInSong[curSongIndex, songDiff, songMode], curSongIndex, songDiff, songMode);
        LoadSong(curSongIndex);
    }

    public void SetSongLav()
    {
        curSongIndex = 1;
        menuManager.UpdateStatsText(songDurs[curSongIndex], notesInSong[curSongIndex, songDiff, songMode], curSongIndex, songDiff, -1);
        LoadSong(curSongIndex);
    }

    public void SetSongBDT()
    {
        curSongIndex = 0;
        menuManager.UpdateStatsText(songDurs[curSongIndex], notesInSong[curSongIndex, songDiff, songMode], curSongIndex, songDiff, -1);
        LoadSong(curSongIndex);
    }

    public void ChangeVolume(System.Single sIn)
    {
        audioSource.volume = sIn;
    }

    public void ContinueFromTut()
    {
        if (!isSongPaused)
        {
            audioSource.Play();
        }
        tutPause = false;
    }

    public void ToggleHaptics()
    {
        if (PlayerPrefs.GetInt("UsingHaptics") == 0)
        {
            PlayerPrefs.SetInt("UsingHaptics", 1);
            hapticsCheckmark.SetActive(true);
            usingHaptics = true;
        }
        else
        {
            PlayerPrefs.SetInt("UsingHaptics", 0);
            hapticsCheckmark.SetActive(false);
            usingHaptics = false;
        }
    }
}
