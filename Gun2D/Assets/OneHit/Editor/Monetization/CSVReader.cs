using UnityEngine;
using System.IO;
using System.Globalization;
using Sirenix.OdinInspector;
using UnityEditor;
using System;

public class CSVReader
{
    public static string csvFilePath = "Assets/IdAds.csv";
    [Button("ReadData")]
    public string ReadData(int row, int column)
    {
        return ReadCSVData(row, column);
    }
    public static string ReadCSVData(int row, int column)
    {
        if (File.Exists(csvFilePath))
        {
            using StreamReader reader = new StreamReader(csvFilePath);
            int lineNumber = 1;
            string line;
            string data;
            while ((line = reader.ReadLine()) != null)
            {
                if (lineNumber == row)
                {
                    string[] columns = line.Split(',');
                    if (columns.Length >= column)
                    {
                        data = columns[column - 1];
                        Debug.LogFormat(data);
                        return data.Trim();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", "Không đủ số cột trong dòng thứ 18 của tệp CSV.", "OK");
                    }
                    break;
                }
                lineNumber++;
            }
        }
        else
        {
            throw new Exception();
        }
        return "";
    }
}
