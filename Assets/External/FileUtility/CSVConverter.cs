using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileUtility
{
    public static class CSVConverter
    {
        public static List<List<string>> GetArray(string text)
        {
            List<List<string>> data = new List<List<string>>();
            
            StringReader sr = new StringReader(text);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] sp = line.Split(',');
                List<string> tmp = new List<string>();
                foreach (var tx in sp) tmp.Add(tx);
                data.Add(tmp);
            }
            return data;
        }

        public static Dictionary<string, Dictionary<string, string>> GetDictionary(string text)
        {
            var data = GetArray(text);
            if (data.Count == 0) return null;

            Dictionary<string, Dictionary<string, string>> ret = new Dictionary<string, Dictionary<string, string>>();

            for (int i = 1; i < data.Count; i++)
            {
                ret[data[i][0]] = new Dictionary<string, string>();
                for (int j = 1; j < data[i].Count; j++)
                {
                    ret[data[i][0]][data[0][j]] = data[i][j];
                }
            }
            return ret;
        }
    }
}
