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
	public int iNumber;

	void Start()
	{
		ts = new Timestamp();
	}
	void FixedUpdate()
	{
		float fps = 1.0f / Time.deltaTime;
		ts.saveData((long)fps);
	}
	void OnDestroy() {
		ts.EmptyFile(file.Replace(".csv",iNumber.ToString()+".csv"));
		ts.SavetoFile(file.Replace(".csv",iNumber.ToString()+".csv"));
	}
}
