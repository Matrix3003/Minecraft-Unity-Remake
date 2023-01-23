using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks; 
using UnityEngine;

public class DataGenarator
{
    public class GenData
    {
        public System.Action<int[,,]> OnComplete; 
        public Vector3Int GenarationPoint; 
    }
    
    private WorldGenarator GenaratorInstance; 
    private Queue<GenData> DataToGenerate; 
    public bool Terminate; 
    
    public DataGenarator(WorldGenarator worldGen){
        GenaratorInstance = worldGen; 
        DataToGenerate = new Queue<GenData>(); 
        worldGen.StartCoroutine(DataGenLoop()); 
    }

    public void QueueDataToGenarate(GenData data){
        DataToGenerate.Enqueue(data); 
    }

    public IEnumerator DataGenLoop(){
        while(Terminate == false){
            if(DataToGenerate.Count > 0){
                GenData gen = DataToGenerate.Dequeue(); 
                yield return GenaratorInstance.StartCoroutine(GenerateData(gen.GenarationPoint, gen.OnComplete)); 
            }

            yield return null; 
        }
    }
    
    public IEnumerator GenerateData(Vector3Int offset, System.Action<int[,,]> callback){
        
        Vector3Int ChunckSize = WorldGenarator.ChunckSize; 

        Vector2 NoiseOffset = GenaratorInstance.NoiseOffset; 
        Vector2 NoiseScale = GenaratorInstance.NoiseScale;
        
        float HeightIntencity = GenaratorInstance.HeightIntencity; 
        float HeightOffset = GenaratorInstance.HeightOffset; 
        
        int[,,] TempData = new int[ChunckSize.x, ChunckSize.y, ChunckSize.z]; 

        Task t = Task.Factory.StartNew(delegate // onde gera os numeros da matriz 
        {
            //Loop para gerar os "Cubos" do mundo de forma uniforme
            for(int x = 0; x < ChunckSize.x; x++){
                for(int z = 0; z < ChunckSize.z; z++){
                    //Criar numero que variam suavimente entre seus valoresm
                    float PerlinCoordX = NoiseScale.x + (x + (offset.x * 16f)) / ChunckSize.x * NoiseScale.x; 
                    float PerlinCoordY = NoiseScale.y + (z + (offset.z * 16f)) / ChunckSize.z * NoiseScale.y; 
                    int HeightGen = Mathf.RoundToInt(Mathf.PerlinNoise(PerlinCoordX, PerlinCoordY) * HeightIntencity + HeightOffset); 
                    

                    for(int y = HeightGen; y >= 0; y--)
                    {
                        int BlockTypeToAssign = 0; 

                        //colocar grama nas camadas superiores
                        if(y == HeightGen) BlockTypeToAssign = 1; 

                        //colocar 3 camadas de terra apos a grama
                        if(y < HeightGen && y > HeightGen - 4) BlockTypeToAssign = 2; 

                        //colocar terra em todas as outras camadas exeto onde tem terre e na camada 0
                        if(y <= HeightGen - 4 && y > 0) BlockTypeToAssign = 3; 

                        if(y == 0) BlockTypeToAssign = 4; 

                        TempData[x, y, z] = BlockTypeToAssign; // coloca na matriz o tipo de bloco em seu indexador especifico 
                    }
                }
            }            
        }); 

        yield return new WaitUntil(() => {
            return t.IsCompleted;  
        }); 

        if(t.Exception != null) Debug.LogError(t.Exception); 

        WorldGenarator.WorldData.Add(offset, TempData); 
        callback(TempData); 
    }
}
