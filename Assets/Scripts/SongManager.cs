using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;

public class SongManager : MonoBehaviour
{
    public static readonly string[] songPaths = { "Assets/Songs/TestSong.txt" };
    public static readonly string[] audioPaths = { "Audio/TestSong" };
    private Song curSong;
    private bool readyToPlay = false, isSongPlaying = false;
    private float curTime;
    private int curSec = 0;
    private float hitLeniency = .15f, hit3DLeniency = .25f, distance3DLeniency = .381f;
    private Track2D[] all2DTracks;
    private Track3D[] all3DTracks;
    public GameObject[] note2DObjects, note3DObjects;
    private GameObject[] spawned2DNotes, spawned3DNotes;
    private int note2DSpawnIndex, note3DSpawnIndex;
    private float spawn2DBeatsInAdvance = 3, spawn3DBeatsInAdvance = 3;
    private Note2D[] note2Ds;
    private Note3D[] note3Ds;
    private AudioClip songAudio;
    private AudioSource audioSource;

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
    //public GameObject leftHandSphere, rightHandSphere;
    public TMPro.TextMeshProUGUI hits2D, hits3D;
    public int hit2Dcount, hit3Dcount;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        LoadSong(0);
        all2DTracks = new Track2D[4] { GameObject.Find("Track2D1").GetComponent<Track2D>(),
                                       GameObject.Find("Track2D2").GetComponent<Track2D>(),
                                       GameObject.Find("Track2D3").GetComponent<Track2D>(),
                                       GameObject.Find("Track2D4").GetComponent<Track2D>()};
        all3DTracks = new Track3D[2] { GameObject.Find("Track3D5").GetComponent<Track3D>(),
                                       GameObject.Find("Track3D6").GetComponent<Track3D>()};
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

            foreach (Track3D track in all3DTracks)
            {
                track.MoveNotesForwards(deltaTime / (curSong.GetBeatLength() * spawn3DBeatsInAdvance), note3Ds);
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
                    //Debug.Log("Despawned");
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

            //Spawn and despawn 3D notes based on the time that has elapsed
            foreach (Note3D note in note3Ds)
            {
                //Despawning Notes
                if (!note.WasHit() && (curTime - hit3DLeniency) > note.GetTime())
                {
                    Destroy(spawned3DNotes[note.GetSpawnIndex()]);
                    note.Hit();
                    Debug.Log("Despawned");
                }

                //Spawning Notes
                if (!note.HasSpawned() && (curTime + spawn3DBeatsInAdvance * curSong.GetBeatLength()) > note.GetTime())
                {
                    spawned3DNotes[note3DSpawnIndex] = Instantiate(note3DObjects[note.GetNoteType()],
                                                                   note.GetStartingPos(), Quaternion.identity,
                                                                   all3DTracks[note.GetTrackNum() - 5].transform);
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

            hits2D.text = hit2Dcount + " / " + note2Ds.Length;
            hits3D.text = hit3Dcount + " / " + note3Ds.Length;
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
        songAudio = Resources.Load<AudioClip>(audioPaths[songIndex]);
        audioSource.clip = songAudio;
        float curVol = audioSource.volume;
        audioSource.volume = 0;
        audioSource.Play();
        audioSource.Stop();
        audioSource.volume = curVol;

        hit2Dcount = 0;
        hit3Dcount = 0;

        readyToPlay = true;
    }

    public void PlaySong()
    {
        if (!isSongPlaying && readyToPlay)
        {
            isSongPlaying = true;
            audioSource.Play();
        }
    }

    public void EndSong()
    {
        isSongPlaying = false;
        LoadSong(0);
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
                hit2Dcount++;
                return;
            }
        }
        Debug.Log("Miss!");
    }

    public void ReleaseTrack(int trackNum)
    {
        foreach (Note2D note in note2Ds)
        {
            if (!note.WasHit() && note.GetTrackNum() == trackNum && note.HasSpawned() &&
                Mathf.Abs(curTime - note.GetTime()) < hitLeniency &&
                note.GetNoteType() == 2)
            {
                Destroy(spawned2DNotes[note.GetSpawnIndex()]);
                note.Hit();
                all2DTracks[trackNum - 1].SetIsHolding(false);
                hit2Dcount++;
                //Debug.Log("Note Hit!");
                return;
            }
        }
        if (all2DTracks[trackNum - 1].IsHolding())
        {
            all2DTracks[trackNum - 1].SetIsHolding(false);
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
                    Debug.Log("3d Note Hit!");
                    hit3Dcount++;
                }
                else
                {
                    Debug.Log("Bad Timing!");
                }
                return;
            }
        }
    }
}
