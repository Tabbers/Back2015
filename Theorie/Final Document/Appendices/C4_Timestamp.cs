using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class Timestamp{

	private string sData="";

	public Timestamp()
	{
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
		string first		= time.ToString();
		string newline 		= string.Format("{0},{1}", first,Environment.NewLine);
		sData += newline;
	}
	public void SavetoFile(string filePath)
	{
		if (!File.Exists("Data/"+filePath))
		{
			File.Create("Data/"+filePath);
		}
		File.AppendAllText("Data/"+filePath, sData);
	}
}
	