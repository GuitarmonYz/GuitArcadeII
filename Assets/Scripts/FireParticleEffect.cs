using UnityEngine;
using System.Collections;

public class FireParticleEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public GameObject ParticlePrefab;
    public float Rate = 500; // per second

    float timeSinceLastSpawn = 0;
	
	// Update is called once per frame
	void Update () {
	
        timeSinceLastSpawn += Time.deltaTime;
        float correctTimeBetweenSpawns = 1f/Rate;
        while( timeSinceLastSpawn > correctTimeBetweenSpawns )
        {
            // Time to spawn a particle
            SpawnFireAlongOutline();
            timeSinceLastSpawn -= correctTimeBetweenSpawns;
        }

	}

    void SpawnFireAlongOutline()
    {

        PolygonCollider2D col = GetComponent<PolygonCollider2D>();

        int pathIndex = Random.Range(0, col.pathCount);

        Vector2[] points = col.GetPath(pathIndex);

        int pointIndex = Random.Range(0, points.Length);

        Vector2 pointA = points[ pointIndex ];
        Vector2 pointB = points[ (pointIndex+1) % points.Length ];

        Vector2 spawnPoint = Vector2.Lerp(pointA, pointB, Random.Range(0f, 1f) );
        Vector2 scaledPoint = new Vector2(spawnPoint.x*4, spawnPoint.y*4-0.05f);
        SpawnFireAtPosition(scaledPoint + (Vector2)this.transform.position);
    }

    void SpawnFireAtPosition(Vector2 position)
    {
        SimplePool.Spawn(ParticlePrefab, position, Quaternion.identity);

    }
}
