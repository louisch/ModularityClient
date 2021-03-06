using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.Events;

public class MapManager : MonoBehaviour {

	public int mapDifference = 50; // 
	public int mapSize = 100;
	// public int partsMin = 2;
	// public int partsMax = 8;
	public float scale = 5f; // used for perlin noise
	public float spawnThreshold = 0.5f; // also used for perlin noise - if no. is greater than this.
	public GameObject[] parts; // holds list of all parts that can be spawned


	private Transform boardHolder;
	private List <Vector3> gridPositions = new List<Vector3> ();


	void Start()
	{
		SetupScene();
	}

	void InitList()
	{
		gridPositions.Clear ();

		for (int x = 1; x < mapSize - 1; ++x) 
		{
			for (int y = 1; y < mapSize -1; ++y)
			{
				gridPositions.Add (new Vector3(x,y,0));
			}
		}
	}

	void BoardSetup ()
	{
		boardHolder = new GameObject ("Board").transform; // not actually a GameObject but a transform

		for (int x = - 1; x < mapSize + 1; ++x) 
		{
			for (int y = -1; y < mapSize + 1; ++y)
			{
				GameObject toInstanciate = parts[Random.Range (0, parts.Length)];
				GameObject instance = Instantiate(toInstanciate, new Vector3(x,y,0), Quaternion.identity) as GameObject;
				instance.transform.SetParent(boardHolder);
			}
		}
	}

	Vector3 RandomPosition ()
	{
		int randomIndex = Random.Range(0, gridPositions.Count);
		Vector3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt(randomIndex);

		return randomPosition;
	}

	// void LayoutObjectRandom(GameObject[] tileArray, int min, int max)
	// {
	// 	int objectCount = Random.Range(min, max);

	// 	for (int i = 0 ; i < objectCount; ++i)
	// 	{
	// 		GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
	// 		Instantiate (tileChoice, RandomPosition(), Quaternion.identity);
	// 	}
	// }

	void LayoutObjectPerlin(GameObject[] tileArray)
	{

		for (int x = 0 ; x < mapSize ; ++x)
		{
			for (int y = 0 ; y < mapSize ; ++y)
			{
				float xCoord = x/scale;
				float yCoord = y/scale;
				float perlinThreshold = Mathf.PerlinNoise(xCoord,yCoord);
				if (perlinThreshold > spawnThreshold)
				{
					GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
					Vector3 position = normaliseToMapSize(RandomPosition());
					Instantiate (tileChoice, position, Quaternion.identity);
				}

			}
		}
	}

	Vector3 normaliseToMapSize(Vector3 position)
	{
		return position - new Vector3(mapDifference,mapDifference,0);
	}

	void SetupScene ()
	{
		// BoardSetup();
		InitList();
		LayoutObjectPerlin (parts);
	}


}
