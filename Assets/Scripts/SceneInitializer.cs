using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
    private void Start()
    {
        string hashKey = "currentLevel";
        if (PlayerPrefs.HasKey(hashKey))
        {
            // we have already made some progress and can load the data
            int indexOfSceneToLoad = PlayerPrefs.GetInt(hashKey);
            SceneManager.LoadScene(sceneBuildIndex: indexOfSceneToLoad);
        }
        else
        {
            // need to start from the 1st scene
            SceneManager.LoadScene(1);
        }

    }
}
