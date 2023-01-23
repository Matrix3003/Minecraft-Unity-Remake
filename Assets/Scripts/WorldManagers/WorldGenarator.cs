using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks; 
using UnityEngine;

//cretits to https://www.youtube.com/c/RytechWasTaken
public class WorldGenarator : MonoBehaviour
{
    [Header("ChunckSettings")]
    public static Dictionary<Vector3Int, int[,,]> WorldData; 
    public static Dictionary<Vector2Int, GameObject> ActiveChuncks;  
    public static readonly Vector3Int ChunckSize =  new Vector3Int(16, 256, 16); 

    [SerializeField] private TextureLoader TextureLoaderInstance; 
    [SerializeField] private Material MainMaterial; 
    [Space]
    public Vector2 NoiseScale = Vector2.one; // deixa o terreno mais pontudo
    public Vector2 NoiseOffset = Vector2.zero;  
    [Space]
    public int HeightOffset = 60; // define a altura do terreno
    public int HeightIntencity = 5; // define a variacao do terreno

    private ChunckMeshCreator meshCreator; 
    private DataGenarator dataCreator; 

    private void Start(){
        WorldData = new Dictionary<Vector3Int, int[,,]>(); 
        ActiveChuncks = new Dictionary<Vector2Int, GameObject>(); 
        meshCreator = new ChunckMeshCreator(TextureLoaderInstance, this); 
        dataCreator = new DataGenarator(this); 
    }

    public IEnumerator CreateChunck(Vector2Int ChunckCoord){
        Vector3Int pos = new Vector3Int (ChunckCoord.x, 0 , ChunckCoord.y); 
        //cria o objeto do chunck na unity e adiciona os componentes 
        string name = ($"Chuck {ChunckCoord.x} {ChunckCoord.y}"); 
        GameObject newChunck = new GameObject(name, new System.Type[]{
            typeof(MeshRenderer),
            typeof(MeshFilter), 
            typeof(MeshCollider)
        }); 

        newChunck.transform.position = new Vector3(ChunckCoord.x * 16, 0f, ChunckCoord.y * 16);
        ActiveChuncks.Add(ChunckCoord, newChunck);  

        int[,,] dataToApplay = WorldData.ContainsKey(pos) ? WorldData[pos] : null; 
        Mesh meshToUse = null;

        if(dataToApplay == null)
        {
            dataCreator.QueueDataToGenarate(new DataGenarator.GenData{
                GenarationPoint = pos, 
                OnComplete = x => dataToApplay = x
            });
            yield return new WaitUntil(() => dataToApplay != null); 
        }

        meshCreator.QueueDataToDraw(new ChunckMeshCreator.CreateMesh{
            DataToDraw = dataToApplay, 
            OnComplete = x => meshToUse = x 
        });
        yield return new WaitUntil(() => meshToUse != null); 

        if(meshToUse != null)
        {
            MeshRenderer newMeshRenderer = newChunck.GetComponent<MeshRenderer>(); 
            MeshFilter newMeshFilter = newChunck.GetComponent<MeshFilter>(); 
            MeshCollider collider = newChunck.GetComponent<MeshCollider>();

            newMeshFilter.mesh = meshToUse; 
        
            newMeshRenderer.material = MainMaterial;
            collider.sharedMesh = newMeshFilter.mesh;
        } 
    }

    public void UpdateChunck(Vector2Int ChunckCoord){

        if(ActiveChuncks.ContainsKey(ChunckCoord)){

            Vector3Int DataCoords = new Vector3Int(ChunckCoord.x, 0, ChunckCoord.y); 

            GameObject TargetChunck = ActiveChuncks[ChunckCoord]; 
            MeshFilter TargetChunckMeshFilter = TargetChunck.GetComponent<MeshFilter>(); 
            MeshCollider TargetChunckCollider = TargetChunck.GetComponent<MeshCollider>(); 

            StartCoroutine(meshCreator.CreateMeshFromData(WorldData[DataCoords], x => {
                TargetChunckMeshFilter.mesh = x; 
                TargetChunckCollider.sharedMesh = x; 
            })); 
        }
    }

    public void SetBlock(Vector3Int WorldPosition, int BlockType = 0){

        Vector2Int coords = GetChunckCoordsFromPosition(WorldPosition); 
        Vector3Int DataPosition = new Vector3Int(coords.x, 0, coords.y); 

        if(WorldData.ContainsKey(DataPosition)){
            Vector3Int coordstoChange = WorldToLocalCoords(WorldPosition, coords); 
            WorldData[DataPosition][coordstoChange.x,coordstoChange.y,coordstoChange.z] = BlockType; 
            UpdateChunck(coords); 
        }
    }

    private Vector2Int GetChunckCoordsFromPosition(Vector3 WorldPosition){
        return new Vector2Int(
            Mathf.FloorToInt(WorldPosition.x / ChunckSize.x), 
            Mathf.FloorToInt(WorldPosition.z / ChunckSize.z)
        ); 
    }

    private Vector3Int WorldToLocalCoords(Vector3Int WorldPosition, Vector2Int Coords){
        return new Vector3Int{
            x = WorldPosition.x - Coords.x * ChunckSize.x, 
            y = WorldPosition.y, 
            z = WorldPosition.z - Coords.y * ChunckSize.z
        }; 
    }
}
