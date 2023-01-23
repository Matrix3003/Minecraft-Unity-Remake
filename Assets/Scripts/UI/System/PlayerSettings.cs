using UnityEngine;
using UnityEngine.UI; 

public class PlayerSettings : MonoBehaviour
{
    public Slider sensibilidadeSlider; 
    public Slider rendererDistanceSlider; 
    
    public static float PlayerSensibilidade; 
    public static int PlayerRendererDistance; 

    public void Aplay(){
        PlayerSensibilidade = sensibilidadeSlider.value; 
        PlayerRendererDistance = (int)rendererDistanceSlider.value; 

        GameObject lvlLoader = GameObject.FindGameObjectWithTag("LevelLoader"); 
        lvlLoader.GetComponent<LevelLoader>().LoadNewScene("Menu"); 
    }
}
