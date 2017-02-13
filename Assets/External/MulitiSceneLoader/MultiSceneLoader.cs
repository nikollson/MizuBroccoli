using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


[ExecuteInEditMode]
public class MultiSceneLoader : MonoBehaviour
{

    public string[] scenes;


    void Start()
    {
        if (Application.isPlaying) LoadAdditiveScenesApplication();
        if (!Application.isPlaying) LoadAdditiveScenesEditor();
    }

    
    void LoadAdditiveScenesApplication()
    {
        foreach (var a in scenes)
        {
            bool hit = false;
            for (int i = 0; i < SceneManager.sceneCount; i++) hit |= SceneManager.GetSceneAt(i).name == a;
            if (!hit) SceneManager.LoadScene(a, LoadSceneMode.Additive);
        }
    }


    void LoadAdditiveScenesEditor()
    {
#if UNITY_EDITOR
        var t = UnityEditor.EditorBuildSettings.scenes;
        List<string> sceneNames = new List<string>();
        List<string> scenePathes = new List<string>();
        foreach (var y in t)
        {
            string str = y.path;
            int p = str.Length - 1;
            for (int i = 0; i < 1000; i++)
            {
                if (p == -1 || str[p] == '/')
                {
                    string cut = str.Substring(p + 1, (str.Length - p - 1) - (".unity".Length)); ;
                    sceneNames.Add(cut);
                    scenePathes.Add(str);
                    break;
                }
                p--;
            }
        }


        if (scenes != null)
        {
            foreach (var a in scenes)
            {
                for (int i = 0; i < sceneNames.Count; i++)
                {
                    if (sceneNames[i] == a)
                    {
                        EditorSceneManager.OpenScene(scenePathes[i], OpenSceneMode.Additive);
                    }
                }
            }
        }
#endif
    }
}
