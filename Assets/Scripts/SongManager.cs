using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    public static readonly string[] songPaths = {"Assets/Songs/TestSong.txt"}; 
    private Song curSong;
    private bool isSongPlaying = false;
    private float curTime;
    private int curSec = 0;
    private float hitLeniency = .15f;
    private Track2D[] all2DTracks;
    public GameObject note2DObject;
    private GameObject[] spawned2DNotes;
    private int noteSpawnIndex;
    private float spawnBeatsInAdvance = 2;
    Note2D[] notes;

    private KeyCode track1Hit = KeyCode.D;
    private KeyCode track2Hit = KeyCode.F;
    private KeyCode track3Hit = KeyCode.H;
    private KeyCode track4Hit = KeyCode.J;

    // Start is called before the first frame update
    void Start()
    {
        LoadSong(0);
        all2DTracks = new Track2D[4] { GameObject.Find("Track2D1").GetComponent<Track2D>(),
                                       GameObject.Find("Track2D2").GetComponent<Track2D>(),
                                       GameObject.Find("Track2D3").GetComponent<Track2D>(),
                                       GameObject.Find("Track2D4").GetComponent<Track2D>()};
        spawned2DNotes = new GameObject[curSong.GetAll2DNotes().Length];
        PlaySong();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSongPlaying)
        {
            //Store the time that has advanced to keep it consistent throughout Update
            float deltaTime = Time.deltaTime;

            //Move all spawned notes down
            foreach (Track2D track in all2DTracks)
            {
                track.MoveNotesDown(deltaTime / (curSong.GetBeatLength() * spawnBeatsInAdvance)
                                    * (track.gameObject.GetComponent<RectTransform>().sizeDelta.y -
                                    note2DObject.GetComponent<RectTransform>().sizeDelta.y));
            }

            //Check if user made any inputs for 2D Notes
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

            //Spawn and despawn notes based on the time that has elapsed
            foreach (Note2D note in notes)
            {
                if (!note.WasHit() && (curTime - hitLeniency) > note.GetTime())
                {
                    Destroy(spawned2DNotes[note.GetSpawnIndex()]);
                    note.Hit();
                    Debug.Log("Despawned");
                }

                if (!note.HasSpawned() && (curTime + spawnBeatsInAdvance * curSong.GetBeatLength()) > note.GetTime())
                {
                    spawned2DNotes[noteSpawnIndex] = Instantiate(note2DObject, all2DTracks[note.GetTrackNum() - 1].transform);
                    note.Spawn();
                    note.SetSpawnIndex(noteSpawnIndex);
                    noteSpawnIndex++;
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
        noteSpawnIndex = 0;
        notes = curSong.GetAll2DNotes();
    }

    public void PlaySong()
    {
        isSongPlaying = true;
    }

    public void HitTrack(int trackNum)
    {
        foreach (Note2D note in notes)
        {
            if (!note.WasHit() && note.GetTrackNum() == trackNum && Mathf.Abs(curTime - note.GetTime()) < hitLeniency)
            {
                Destroy(spawned2DNotes[note.GetSpawnIndex()]);
                note.Hit();
                Debug.Log("Note Hit!");
                return;
            }
        }
        Debug.Log("Miss!");
    }
}
