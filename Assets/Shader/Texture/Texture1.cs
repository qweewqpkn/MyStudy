using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Texture1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        List<Vector3> planeVerticesList = new List<Vector3>();
        planeVerticesList.Add(new Vector3(-0.5f, 0.0f, -0.5f));
        planeVerticesList.Add(new Vector3(0.5f, 0.0f, -0.5f));
        planeVerticesList.Add(new Vector3(0.5f, 0.0f, 0.5f));
        planeVerticesList.Add(new Vector3(-0.5f, 0.0f, 0.5f));
        mesh.SetVertices(planeVerticesList);


        mesh.SetIndices(new int[] {0, 2, 1, 0, 3, 2 }, MeshTopology.Triangles, 0);

        List<Vector2> planeUVList = new List<Vector2>();
        planeUVList.Add(new Vector2(0.0f, 0.0f));
        planeUVList.Add(new Vector2(1.0f, 0.0f));
        planeUVList.Add(new Vector2(0.0f, 1.0f));
        planeUVList.Add(new Vector2(1.0f, 1.0f));
        mesh.SetUVs(0, planeUVList);

        mf.mesh = mesh;

        mr.material = new Material(Shader.Find("LH/Texture"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
