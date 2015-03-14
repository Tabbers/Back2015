using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Display_System : MonoBehaviour {
	

	Panel[] pDisplayPanels;
	// Use this for initialization
	void Start () {
		pDisplayPanels = GameObject.FindGameObjectsWithTag<Panel>("DisplayPanel");
	}
	
	// Update is called once per frame
	void Update () 
	{
		foreach (Panel pDisplay in pDisplayPanels)
		{	
			
		}
	}
}
