using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDesable : MonoBehaviour
{
    [SerializeField] private GameObject player, cam, loodingUi, gameUi; 
    private float timeToEnablePlayer; 

    private int rendererDistance; 

    private void Start() {
        player.SetActive(false); 
        loodingUi.SetActive(true); 
        gameUi.SetActive(false); 

        rendererDistance = this.GetComponent<InfinitTerrainGenarator>().RendererDistance; 
        StartCoroutine(EnablePlayer()); 
    }

    private IEnumerator EnablePlayer(){

        if(rendererDistance <= 4) timeToEnablePlayer = 10; 
        if(rendererDistance > 4 && rendererDistance <= 6) timeToEnablePlayer = 20; 
        if(rendererDistance > 6 && rendererDistance <= 10) timeToEnablePlayer = 40; 
        
        yield return new WaitForSeconds(timeToEnablePlayer); 

        player.SetActive(true); 
        loodingUi.SetActive(false); 
        gameUi.SetActive(true); 

        cam.transform.SetParent(player.transform);
        cam.transform.localPosition = Vector3.zero; 
    }
}
