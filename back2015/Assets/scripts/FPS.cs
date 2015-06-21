using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class FPS  : MonoBehaviour
{
	public string file;
	public Timestamp ts;
	public Timestamp ts2;
	public int iNumber;
	float deltaTime = 0.0f;

	void Start()
	{
		ts = new Timestamp();
		ts2 = new Timestamp();

	}
	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		float fps = 1.0f /deltaTime;
		ts.saveData((long)fps);
	}
	void OnApplicationQuit() {
		ts.EmptyFile(file);
		ts.SavetoFile(file);

		ts2.EmptyFile("collissions.csv");
		GameObject[] om = GameObject.FindGameObjectsWithTag("dynamic");
		long sum =0;
		foreach(GameObject go in om)
		{
			sum+= go.GetComponent<Obstacle_movement>().Collisions;
		}
		ts2.saveData(sum);
		ts2.SavetoFile("collissions.csv");
	}
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
		
		GUIStyle style = new GUIStyle();
		
		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}
}
