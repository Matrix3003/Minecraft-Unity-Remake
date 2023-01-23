using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsController : MonoBehaviour
{
    [SerializeField] private GameObject levelLoader; 
    private LevelLoader lvlLoader; 
    
    private void Start() {
        lvlLoader = levelLoader.GetComponent<LevelLoader>(); 
    }
    
    public void Settings(){
        lvlLoader.LoadNewScene("Options");
    }
    
    public void Play(){
        lvlLoader.LoadNewScene("GameScnene"); 
    }
    
    public void QuitGame(){
        Application.Quit(); 
    }
}
