using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Track2D : MonoBehaviour
{
    public int trackNum;
    private bool isHolding = false;
    private Image animRing;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAnimRing(Image ringIn)
    {
        animRing = ringIn;
        animRing.color = new Color(1, 1, 1, 0);
    }

    public bool IsHolding()
    {
        return isHolding;
    }

    public void SetIsHolding(bool holdIn)
    {
         isHolding = holdIn;
    }

    public void MoveNotesDown(float distance)
    {
        RectTransform[] allChildren = GetComponentsInChildren<RectTransform>();
        foreach (RectTransform child in allChildren)
        {
            if (child.gameObject.CompareTag("Note"))
            {
                child.localPosition = new Vector3(child.localPosition.x,
                                                  child.localPosition.y - distance,
                                                  child.localPosition.z);
            }
        }
    }

    public void PlayHitAnim()
    {
        StopAllCoroutines();
        animRing.color = new Color(1, 1, 1, 1);
        StartCoroutine(HitAnim());
    }

    IEnumerator HitAnim()
    {
        for (int frameNum = 0; frameNum < 20; frameNum++)
        {
            yield return new WaitForSeconds(.01f);
            animRing.color = new Color(1, 1, 1, 1 - frameNum / 20f);
        }
        animRing.color = new Color(1, 1, 1, 0);
    }

    public void PlayHoldingAnim()
    {
        StopAllCoroutines();
        animRing.color = new Color(1, 1, 1, 1);
        StartCoroutine(HoldingAnim());
    }

    public void StopHoldingAnim()
    {
        StopAllCoroutines();
        animRing.color = new Color(1, 1, 1, 0);
    }

    IEnumerator HoldingAnim()
    {
        animRing.color = new Color(1, 1, 1, 1);
        for (int frameNum = 0; frameNum < 50; frameNum++)
        {
            yield return new WaitForSeconds(.01f);
            animRing.color = new Color(1, 1, 1, Mathf.Abs(frameNum - 25)/25f);
        }
        StartCoroutine(HoldingAnim());
    }
}
