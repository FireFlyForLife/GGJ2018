using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedText : MonoBehaviour
{
    private Text text;
    private float timeShown = 0;
    public float ShowDuration = 0.7f;

	void Start ()
	{
	    text = GetComponent<Text>();
	    text.enabled = false;
	    ShowDuration = 2;
	}

    public void AddKill(FPSPlayer killer)
    {
        text.text = "Player" + ( killer.PlayerNumber+1 )+ " Killed You!";
        timeShown = Time.time;
    }
	
	void Update ()
	{
	    text.enabled = timeShown + ShowDuration > Time.time;
	}
}
