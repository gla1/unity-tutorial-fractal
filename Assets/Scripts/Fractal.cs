using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour
{
   
    public Material material;

    public int maxDepth;
    private int depth;

    private float childScale;
    public float maxRotationSpeed;
    private float rotationSpeed;

    private Material[,] materials;
    public Mesh[] meshes;

    public float maxTwist;

    public float maxScale;
    private static Vector3[] childDirections = {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };

    public float spawnProbability;

    private static Quaternion[] childOrientations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f)
    };
    private void InitializeMaterials(){
        materials = new Material[maxDepth + 1, 2];
        for (int i = 0; i <= maxDepth; i++){
            float t = i / (maxDepth - 1f);
            t *= t;
            materials[i, 0] = new Material(material);
            materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, (float)i / maxDepth);
            materials[i, 1] = new Material(material);
            materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, (float)i / maxDepth);
        }
        materials[maxDepth, 0].color = Color.magenta;        
        materials[maxDepth, 1].color = Color.red;

    }

    private void Start() {
        if (materials == null) {
            InitializeMaterials();
        }
        rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        transform.Rotate(Random.Range(-maxTwist, maxTwist), 0f, 0f);
        childScale = Random.Range(maxScale/4, maxScale);
        gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
        gameObject.AddComponent<MeshRenderer>().material = materials[depth, Random.Range(0, 2)];
        if (depth < maxDepth) {
            StartCoroutine(CreateChild());
        }
    }    

    private IEnumerator CreateChild() {
        for (int i = 0; i < childDirections.Length; i++) {
            if (Random.value < spawnProbability) {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, childDirections[i], childOrientations[i]);
            }
        }
    }


    private void Initialize(Fractal parent, Vector3 direction, Quaternion orientation){
        meshes = parent.meshes;
        materials = parent.materials;
        maxDepth = parent.maxDepth;
        depth = parent.depth +1;
        childScale = parent.childScale;
        transform.parent = parent.transform;
        spawnProbability = parent.spawnProbability;
        maxRotationSpeed = parent.maxRotationSpeed;
        maxTwist = parent.maxTwist;
        maxScale = parent.maxScale;

        transform.localScale = Vector3.one * childScale;
        transform.localPosition = direction * (0.5f + 0.5f * childScale);
        transform.localRotation = orientation;
    }


    private void Update() {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 02f);
    }
} 
