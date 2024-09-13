using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Reactional.Core;
public class FloatingObjects : MonoBehaviour
{
    public List<GameObject> prefab; // The prefab to spawn
    public float spawnRate = 1f; // The rate at which to spawn the prefabs
    public float speed = 1f; // The speed at which the prefabs move upwards
    public float tumble = 1f; // The speed at which the prefabs tumble
    private float realTumble = 1f; // The real tumble speed
    public float xRange = 10f; // The range for the X position
    public float zRange = 10f; // The range for the Z position
    public float yPosition = -10f; // The Y position below the camera
    public float xOffSet = 0f; // The X offset for the spawn position
    public float destroyHeight = 20f; // The height above the camera to destroy the prefab
    public Material material; // The material to apply to the prefab
    public Material flashMaterial; // The material to apply to the prefab when flashing
    private int lastbar;

    private List<GameObject> spawnedObjects = new List<GameObject>(); // The list of spawned prefabs

    void Start()
    {
        StartCoroutine(SpawnPrefabs());
        ReactionalEngine.Instance.onBarBeat += OnBarBeat;
    }

    IEnumerator SpawnPrefabs()
    {
        while (true)
        {
            float xPosition = Random.Range(-xRange, xRange);
            float zPosition = Random.Range(-zRange, zRange);
            GameObject obj = Instantiate(prefab[Random.Range(0, prefab.Count)], new Vector3(xPosition + xOffSet, yPosition, zPosition), Quaternion.identity);
            obj.transform.parent = transform;
            Destroy(obj.GetComponent<Piece>());
            Destroy(obj.GetComponent<GhostPiece>());
            
            // set all scale to 1
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.AddComponent<ReactionalSizePulse>();
            foreach (Transform child in obj.transform)
            {
                MeshRenderer renderer = child.GetComponent<MeshRenderer>();
                renderer.material = material;
            }
            spawnedObjects.Add(obj);
            yield return new WaitForSeconds(1f / spawnRate);
        }
    }

    void Update()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                obj.transform.position += Vector3.up * speed * Time.deltaTime;
                obj.transform.Rotate(new Vector3(Random.value, Random.value, Random.value) * realTumble);

                // if higher than Y position above camera, destroy
                if (obj.transform.position.y > destroyHeight)
                {
                    Destroy(obj);
                }
            }
        }
    }

    private void OnBarBeat(double offset, int bar, int beat)
    {
        // Change the speed of the prefabs
        speed = beat;

        if (bar % 2 == 0)
        {
            // Change the tumble of the prefabs
            realTumble = tumble * 2;
        }
        else
        {
            // Change the tumble of the prefabs
            realTumble = tumble * 0.5f;
        }

        if (bar != lastbar)
        {
            // Change the spawn rate of the prefabs
            StartCoroutine(Flash());
            lastbar = bar;
        }
    }

    IEnumerator Flash()
    {
        // Flash the prefabs
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            foreach (GameObject obj in spawnedObjects)
            {
                if (obj != null)
                {
                    foreach (Transform child in obj.transform)
                    {
                        MeshRenderer renderer = child.GetComponent<MeshRenderer>();
                        // interpolate materials
                        renderer.material.Lerp(flashMaterial, material, t);
                    }
                }
            }
            yield return null;
        }
    }
}