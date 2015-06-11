using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class Timestamp{
	
	public static void SavetoFile(string filePath,long time)
	{
		if (!File.Exists(filePath))
		{
			File.Create(filePath);
		}
		File.WriteAllText(filePath,string.Empty);
		StringBuilder csv = new StringBuilder();
		string first = time.ToString();
		string newline =  string.Format("{0},{1}", first, Environment.NewLine);
		csv.Append(newline);
		File.AppendAllText(filePath, csv.ToString());
	}
}
	