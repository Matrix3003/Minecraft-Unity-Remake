using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinitTerrainGenarator : MonoBehaviour
{
    [SerializeField] private Transform Player; 
    [SerializeField] private int rendererDistance = 0; 
    WorldGenarator GenaratorInstance; 
    private List<Vector2Int> CoordsToRemove; 

    public int RendererDistance
    {
        get{return rendererDistance;} 
        set{rendererDistance = value;}
    }

    private void Start(){
        GenaratorInstance = this.GetComponent<WorldGenarator>(); 
        CoordsToRemove = new List<Vector2Int>(); 

        if(PlayerSettings.PlayerRendererDistance != 0) rendererDistance = PlayerSettings.PlayerRendererDistance; 
    }

    private void Update() {
        int plrChunckX = (int)Player.position.x / WorldGenarator.ChunckSize.x; 
        int plrChunckY = (int)Player.position.z / WorldGenarator.ChunckSize.z; 
        CoordsToRemove.Clear(); 

        foreach(KeyValuePair<Vector2Int, GameObject> activeChunck in WorldGenarator.ActiveChuncks)
        {
            CoordsToRemove.Add(activeChunck.Key); 
        }

        for(int x = plrChunckX - rendererDistance; x <= plrChunckX + rendererDistance; x++)
        {
            for(int y = plrChunckY - rendererDistance; y <= plrChunckY + rendererDistance; y++)
            {
                Vector2Int chunckCoord = new Vector2Int(x, y); 

                if(!WorldGenarator.ActiveChuncks.ContainsKey(chunckCoord))
                {
                    StartCoroutine(GenaratorInstance.CreateChunck(chunckCoord)); 
                }

                CoordsToRemove.Remove(chunckCoord); 
            }
        }

        foreach(Vector2Int coord in CoordsToRemove)
        {
            GameObject chunckToDelete = WorldGenarator.ActiveChuncks[coord]; 
            WorldGenarator.ActiveChuncks.Remove(coord);
            Destroy(chunckToDelete);  
        }
    }

}
