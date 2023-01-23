using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunckInteractor : MonoBehaviour
{
    [SerializeField]private LayerMask ChunckInteractMask; 
    [SerializeField]private LayerMask BoundCheckMask; 
    [SerializeField]private Transform PlayerCamera; 
    [SerializeField]private float InteractRange = 8f;
    private WorldGenarator WorldGenInstance; 

    private void Start(){
        WorldGenInstance = FindObjectOfType<WorldGenarator>(); 
    } 

    private void Update(){
        if(Input.GetMouseButtonDown(0)){

            Ray camRay = new Ray(PlayerCamera.position, PlayerCamera.forward); 

            if(Physics.Raycast(camRay, out RaycastHit hitInfo, InteractRange, ChunckInteractMask)){
                
                Vector3 targetPoint = hitInfo.point - hitInfo.normal * .1f; 
                
                Vector3Int targetBlock = new Vector3Int{
                    x = Mathf.RoundToInt(targetPoint.x), 
                    y = Mathf.RoundToInt(targetPoint.y), 
                    z = Mathf.RoundToInt(targetPoint.z)
                }; 

                string chunckName = hitInfo.collider.gameObject.name; 
                if(chunckName.Contains("Chuck")){
                    WorldGenInstance.SetBlock(targetBlock, 0); 
                }
            }
        }

        if(Input.GetMouseButtonDown(1)){

            Ray camRay = new Ray(PlayerCamera.position, PlayerCamera.forward); 

            if(Physics.Raycast(camRay, out RaycastHit hitInfo, 4f, ChunckInteractMask)){
                
                Vector3 targetPoint = hitInfo.point + hitInfo.normal * .1f; 
                
                Vector3Int targetBlock = new Vector3Int{
                    x = Mathf.RoundToInt(targetPoint.x), 
                    y = Mathf.RoundToInt(targetPoint.y), 
                    z = Mathf.RoundToInt(targetPoint.z)
                }; 

                if(!Physics.CheckBox(targetBlock, Vector3.one * .5f, Quaternion.identity, BoundCheckMask)){

                    string chunckName = hitInfo.collider.gameObject.name; 

                    if(chunckName.Contains("Chuck")){
                        WorldGenInstance.SetBlock(targetBlock, 3); 
                    }
                }
            }
        }
    }
}
