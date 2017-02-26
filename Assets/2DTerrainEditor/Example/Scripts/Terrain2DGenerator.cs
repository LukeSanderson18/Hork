/* This script shows how realtime generation works. Read the comments in code to understand the process */

using System.Collections.Generic;
using UnityEngine;

public class Terrain2DGenerator : MonoBehaviour
{
    public Transform Player;
    public GameObject TerrainTemplate;

    //Position for next generated terrain
    private Vector3 _nextTerrainPos;
    //List of terrains that have been already generated and placed on the scene
    private List<GameObject> _createdTerrains = new List<GameObject>(); 

	void Start () 
    {
        //Generate first terrain in the middle of the scene
        GenerateNextTerrain2D(Vector2.zero);
	}

    void Update()
    {
        //Check if player already cross the next line and want to see new terrain
        if (Player.position.x > _nextTerrainPos.x) 
        {
            //Increment next terrainPos that player must cross for generating new terrain
            _nextTerrainPos.x += TerrainTemplate.GetComponent<TerrainEditor2D>().Width; 
            //Generate new terrain in next position
            GenerateNextTerrain2D(new Vector2(_nextTerrainPos.x, 0));
        }

        //We assume that the player always move forward and can not see terrains that stayed behind him
        //Check if scene has more that 5 terrains
        if (_createdTerrains.Count > 5)
        {
            //Destroy terrain and remove it from the list
            Destroy(_createdTerrains[0]);
            _createdTerrains.RemoveAt(0);
        }
    }

    void FixedUpdate()
    {
        //Move player horizontally forward
        Player.transform.Translate(Vector3.right * (Time.deltaTime * 15));
        //Follow the player
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(Player.position.x, Player.position.y, Camera.main.transform.position.z), Time.deltaTime * 25f);
    }

    void GenerateNextTerrain2D(Vector2 terrainPos)
    {
        //Instantiate terrain template object
        GameObject newTerrainObj = (GameObject) Instantiate(TerrainTemplate, _nextTerrainPos, Quaternion.identity);

        TerrainEditor2D terrain = newTerrainObj.GetComponent<TerrainEditor2D>();

        //Set random values before generating if needed
        terrain.RndAmplitude = Random.Range(3, 6);
        terrain.RndHillsCount = Random.Range(2, 8);
        terrain.RndHeight = Random.Range(20, 30);

        //Check if any terrain has been already generated
        if (_createdTerrains.Count > 0)
            //Specify the left initial path point from last terrain to new terrain to obtain smooth transition
            //We don't want to update shared mesh of our terrain template
            terrain.RandomizeTerrain(_createdTerrains[_createdTerrains.Count - 1].GetComponent<TerrainEditor2D>().GetLastVertexPos(), false);
        else terrain.RandomizeTerrain(false);

        //Add new terrain to the list
        _createdTerrains.Add(newTerrainObj);
    }
}
