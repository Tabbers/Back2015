using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class Timestamp{

	private List<long> Data;

	public Timestamp()
	{
		Data = new List<long>();
	}

	public void EmptyFile(string filePath)
	{
		if (!File.Exists("Data/"+filePath))
		{
			File.WriteAllText("Data/"+filePath,string.Empty);
		}
	}
	public void saveData(long time)
	{
		Data.Add(time);
	}
	public void SavetoFile(string filePath)
	{
		long sum = 0;
		string sData="";

		foreach(long Datum in Data)
		{
			sum+= Datum;
		}
		UnityEngine.Debug.Log(sum);
		float average = (float)sum/Data.Count;
		UnityEngine.Debug.Log(average);
		string first		= average.ToString();
		string newline 		= string.Format("{0},{1}", first,Environment.NewLine);
		sData += newline;
		if (!File.Exists("Data/"+filePath))
		{
			File.Create("Data/"+filePath);
		}
		File.AppendAllText("Data/"+filePath, sData);
	}
}
	