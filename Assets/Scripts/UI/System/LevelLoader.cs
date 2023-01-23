using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class LevelLoader : MonoBehaviour
{
    [SerializeField]private Animator anm; 
    private string nextScene; 
    private string newScene;
    
    public void LoadNewScene(string newScene)
    {
        nextScene = newScene; 
        StartCoroutine(LoadScene()); 
    }

    IEnumerator LoadScene()
    {
        anm.SetTrigger("Load"); 

        yield return new WaitForSeconds(1f); 

        SceneManager.LoadScene(nextScene); 
    }
}
