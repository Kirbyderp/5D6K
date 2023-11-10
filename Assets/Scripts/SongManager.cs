using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;

public class SongManager : MonoBehaviour
{
    public static readonly string[] songPaths = {"Assets/Songs/TestSong.txt"}; 
    private Song curSong;
    private bool isSongPlaying = false;
    private float curTime;
    private int curSec = 0;
    private float hitLeniency = .15f;
    private Track2D[] all2DTracks;
    public GameObject[] note2DObjects, note3DObjects;
    private GameObject[] spawned2DNotes, spawned3DNotes;
    private int note2DSpawnIndex, note3DSpawnIndex;
    private float spawn2DBeatsInAdvance = 2, spawn3DBeatsInAdvance = 4;
    private Note2D[] note2Ds;
    private Note3D[] note3Ds;

    private bool pressingLGrip = false;
    private bool pressingLTrigger = false;
    private bool pressingRTrigger = false;
    private bool pressingRGrip = false;

    private KeyCode track1Hit = KeyCode.D;
    private KeyCode track2Hit = KeyCode.F;
    private KeyCode track3Hit = KeyCode.H;
    private KeyCode track4Hit = KeyCode.J;

    public InputDevice leftHand, rightHand;

    //DEBUG VARS
    public GameObject[] outlines;

    // Start is called before the first frame update
    void Start()
    {
        LoadSong(0);
        all2DTracks = new Track2D[4] { GameObject.Find("Track2D1").GetComponent<Track2D>(),
                                       GameObject.Find("Track2D2").GetComponent<Track2D>(),
                                       GameObject.Find("Track2D3").GetComponent<Track2D>(),
                                       GameObject.Find("Track2D4").GetComponent<Track2D>()};
        outlines = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            outlines[i] = GameObject.Find("Outline" + (i + 1));
            outlines[i].GetComponent<Image>().color = Color.red;
        }
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
            //Store the time that has advanced to keep it consistent throughout Update
            float deltaTime = Time.deltaTime;

            //Move all spawned notes down
            foreach (Track2D track in all2DTracks)
            {
                track.MoveNotesDown(deltaTime / (curSong.GetBeatLength() * spawn2DBeatsInAdvance)
                                    * (track.gameObject.GetComponent<RectTransform>().sizeDelta.y -
                                    note2DObjects[0].GetComponent<RectTransform>().sizeDelta.y));
            }

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
                outlines[0].GetComponent<Image>().color = Color.blue;
            }
            else if (curLGrip == false && pressingLGrip == true)
            {
                ReleaseTrack(1);
                pressingLGrip = false;
                outlines[0].GetComponent<Image>().color = Color.red;
            }

            //Track 2
            if (curLTrigger == true && pressingLTrigger == false)
            {
                HitTrack(2);
                pressingLTrigger = true;
                outlines[1].GetComponent<Image>().color = Color.blue;
            }
            else if (curLTrigger == false && pressingLTrigger == true)
            {
                ReleaseTrack(2);
                pressingLTrigger = false;
                outlines[1].GetComponent<Image>().color = Color.red;
            }

            //Track 3
            if (curRTrigger == true && pressingRTrigger == false)
            {
                HitTrack(3);
                pressingRTrigger = true;
                outlines[2].GetComponent<Image>().color = Color.blue;
            }
            else if (curRTrigger == false && pressingRTrigger == true)
            {
                ReleaseTrack(3);
                pressingRTrigger = false;
                outlines[2].GetComponent<Image>().color = Color.red;
            }

            //Track 4
            if (curRGrip == true && pressingRGrip == false)
            {
                HitTrack(4);
                pressingRGrip = true;
                outlines[3].GetComponent<Image>().color = Color.blue;
            }
            else if (curRGrip == false && pressingRGrip == true)
            {
                ReleaseTrack(4);
                pressingRGrip = false;
                outlines[3].GetComponent<Image>().color = Color.red;
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

            //Spawn and despawn 2D notes based on the time that has elapsed
            foreach (Note2D note in note2Ds)
            {
                //Despawning Notes
                if (!note.WasHit() && (curTime - hitLeniency) > note.GetTime())
                {
                    Destroy(spawned2DNotes[note.GetSpawnIndex()]);
                    note.Hit();
                    Debug.Log("Despawned");
                }

                //Spawning Notes
                if (!note.HasSpawned() && (curTime + spawn2DBeatsInAdvance * curSong.GetBeatLength()) > note.GetTime())
                {
                    spawned2DNotes[note2DSpawnIndex] = Instantiate(note2DObjects[note.GetNoteType()],
                                                                   all2DTracks[note.GetTrackNum() - 1].transform);
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

            foreach (Note3D note in note3Ds)
            {
                //Despawning Notes
                if (!note.WasHit() && (curTime - hitLeniency) > note.GetTime())
                {
                    Destroy(spawned3DNotes[note.GetSpawnIndex()]);
                    note.Hit();
                    Debug.Log("Despawned");
                }

                //Spawning Notes
                if (!note.HasSpawned() && (curTime + spawn3DBeatsInAdvance * curSong.GetBeatLength()) > note.GetTime())
                {
                    spawned3DNotes[note3DSpawnIndex] = Instantiate(note3DObjects[note.GetNoteType()],
                                                                   note.GetStartingPos(), Quaternion.identity);
                    note.Spawn();
                    note.SetSpawnIndex(note3DSpawnIndex);
                    note3DSpawnIndex++;
                    //Debug.Log("Spawned");
                }
                else if (!note.HasSpawned())
                {
                    break;
                }
            }

            //Debug Stuff
            curTime += deltaTime;
            if (curTime - 1 > curSec)
            {
                //Debug.Log(++curSec);
            }
            if (curTime < 5.1f)
            {
                //Debug.Log(curTime);
            }
        }
    }

    public void LoadSong(int songIndex)
    {
        curSong = new Song(songPaths[songIndex]);
        curTime = 0;
        note2DSpawnIndex = 0;
        note3DSpawnIndex = 0;
        note2Ds = curSong.GetAll2DNotes();
        note3Ds = curSong.GetAll3DNotes();
        spawned2DNotes = new GameObject[curSong.GetAll2DNotes().Length];
        spawned3DNotes = new GameObject[curSong.GetAll3DNotes().Length];
    }

    public void PlaySong()
    {
        isSongPlaying = true;
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
                if (note.GetNoteType() == 1)
                {
                    all2DTracks[note.GetTrackNum() - 1].SetIsHolding(true);
                }
                Debug.Log("Note Hit!");
                return;
            }
        }
        Debug.Log("Miss!");
    }

    public void ReleaseTrack(int trackNum)
    {
        foreach (Note2D note in note2Ds)
        {
            if (!note.WasHit() && note.GetTrackNum() == trackNum &&
                Mathf.Abs(curTime - note.GetTime()) < hitLeniency &&
                note.GetNoteType() == 2)
            {
                Destroy(spawned2DNotes[note.GetSpawnIndex()]);
                note.Hit();
                all2DTracks[trackNum - 1].SetIsHolding(false);
                Debug.Log("Note Hit!");
                return;
            }
        }
        if (all2DTracks[trackNum - 1].IsHolding())
        {
            all2DTracks[trackNum - 1].SetIsHolding(false);
            Debug.Log("Miss!");
        }
    }
}
