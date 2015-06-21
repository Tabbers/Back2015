using UnityEngine;
using System.Collections;

public class AI_Spawn : MonoBehaviour {

	private GameObject goAstar;
	private GameObject goRaystar;
	public GameObject goSpawning;

	public bool bA_star = false;
	public bool bMultiple = false;
	private bool bSpawning = true;
	public int iNumber;
	public int iSpawnd=0;
	public float SpawnInterval = 0;
	private float InstantiationTimer = 1f;
	private bool checktoQuit=false;

	// Use this for initialization
	void Start () 
	{
		goRaystar = Resources.Load("prefabs/AIAgent") as GameObject;
		goAstar = Resources.Load("prefabs/AstarIAgent") as GameObject;
		goAstar.GetComponent<AStarAI>().target = GameObject.FindGameObjectWithTag("Dest").transform;
		if(bA_star) goSpawning = goAstar;
		else goSpawning = goRaystar;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(bSpawning)
		{
			if(!bMultiple)
			{
				GameObject Agent = Instantiate(goSpawning,transform.position,transform.rotation) as GameObject;
				iSpawnd++;
				bSpawning = false;
				checktoQuit = true;
			}
			else
			{
				InstantiationTimer -= Time.deltaTime;
				if (InstantiationTimer <= 0)
				{
					GameObject Agent = Instantiate(goSpawning,transform.position,transform.rotation) as GameObject;
					InstantiationTimer = SpawnInterval;
					iSpawnd ++;
					checktoQuit = true;
				}
				if(iSpawnd == iNumber)bSpawning =false;
				else bSpawning = true;
			}
		}
		if(checktoQuit)
		{
			if(iSpawnd ==0) UnityEditor.EditorApplication.isPlaying = false;
		}
	}
}
