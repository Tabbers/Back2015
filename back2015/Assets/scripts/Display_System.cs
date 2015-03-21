using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Display_System : MonoBehaviour {
	
	Projection_System psScript;
	GameObject[] pDisplayPanels;
	// Use this for initialization
	void Start () {
		psScript = gameObject.GetComponent<Projection_System>();
		pDisplayPanels = FindObsWithTag("DisplayPanel");
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{	
		psScript = gameObject.GetComponent<Projection_System>();
		int iIterator=0;
		foreach (GameObject pDisplay in pDisplayPanels)
		{	
			if(psScript.goVisibleGO[iIterator]!=null)
			{
				if(psScript.goVisibleGO[iIterator].tag =="terrain")
				{
					pDisplay.GetComponentInChildren<Text>().text = psScript.goVisibleGO[iIterator].name;
					pDisplay.GetComponent<Image>().color = Color.red;
				}
				else if(psScript.goVisibleGO[iIterator].name =="none")
				{
					pDisplay.GetComponent<Image>().color = Color.grey;
				}
				
			}
			else
			{
				pDisplay.GetComponentInChildren<Text>().text = "null";
				pDisplay.GetComponent<Image>().color = Color.grey;
			} 	
			iIterator++;
		}
	}
	GameObject[] FindObsWithTag( string tag ) 
	{ 
		GameObject[] foundObs = GameObject.FindGameObjectsWithTag(tag); 
		Array.Sort( foundObs, CompareObNames ); 
		return foundObs; 
	}
	
	int CompareObNames( GameObject x, GameObject y ) 
	{ 
		return x.name.CompareTo( y.name ); 
	}
}
