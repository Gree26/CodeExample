using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{

    //The size of each cube
    private float width = 1;
    private float height = 1;
    private float depth = 1;

    [SerializeField]
    private Material mat;

    //The mesh that we will build upon
    Mesh mesh;

    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector3> normals = new List<Vector3>();
    
    private List<int> triangles = new List<int>();
    //UVs
    private List<Vector2> uv = new List<Vector2>();
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private void Start()
    {
        //Get(Create) mesh renderer and set the shader
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        //Create the mesh filter
        meshFilter = gameObject.AddComponent<MeshFilter>();

        mesh = new Mesh();
        //Call on create incase a level was loaded 
        UpdateMesh();
       
    }

    /**
     * Update each element of the array based on the data saved in each list 
     *  - Vertices
     *  - Triangles
     *  - Normals
     *  - UV
     */
    private void UpdateMesh()
    {
        //If the number of vertices decreased, clear the entire mesh before building 
        if (mesh.vertices.Length > vertices.Count)
        {
            mesh.Clear();
        }
        //Is the mesh empty? if so, dont bother building the mesh 
        if (vertices.Count == 0)
        {
            return;
        }

        //Create an array based on the traingle list
        int[] tris = new int[triangles.Count];

        for (int i = 0; i < triangles.Count; i++)
        {
            tris[i] = triangles[i];
        }

        //Create an array based on the traingle list
        Vector3[] norm = new Vector3[normals.Count];

        for (int i = 0; i < normals.Count; i++)
        {
            norm[i] = normals[i];
        }

        //Create an uv array based on the traingle list to create the UV mesh
        Vector2[] tempUv = new Vector2[uv.Count];

        for (int i = 0; i < uv.Count; i++)
        {
            tempUv[i] = uv[i];
        }

        //Update each part of the mesh !ORDER CANNOT BE CHANGED!
        mesh.SetVertices(vertices);
        mesh.triangles = tris;
        mesh.normals = norm;
        mesh.uv = tempUv;

        mesh.Optimize();

        meshRenderer.material = mat;

        meshFilter.mesh = mesh;

    }

    //Create a cube if the block is not already placed
    //
    public void newCube(Vector3 pos)
    { 
        createCubeMesh(new Vector3(pos.x, pos.y, pos.z));
    }
    //Remove a cube if the block is not already placed
    public void removeCube(Vector3 pos)
    {
        removeCubeMesh(new Vector3(pos.x, pos.y, pos.z));
    }

    //Generate the cube by calling the generate face 6 times
    //Feeding it proper positions
    private void createCubeMesh(Vector3 location)
    {
        //Side 1(-z)
        createCubeFaceMesh(
            new Vector3(location.x - (width / 2), location.y - (height / 2), location.z - (depth / 2)),
            new Vector3(location.x + (width / 2), location.y - (height / 2), location.z - (depth / 2)),
            new Vector3(location.x - (width / 2), location.y + (height / 2), location.z - (depth / 2)),
            new Vector3(location.x + (width / 2), location.y + (height / 2), location.z - (depth / 2)),
            -Vector3.forward
        );

        //Side 2(+z)
        createCubeFaceMesh(
            new Vector3(location.x + (width / 2), location.y - (height / 2), location.z + (depth / 2)),
            new Vector3(location.x - (width / 2), location.y - (height / 2), location.z + (depth / 2)),
            new Vector3(location.x + (width / 2), location.y + (height / 2), location.z + (depth / 2)),
            new Vector3(location.x - (width / 2), location.y + (height / 2), location.z + (depth / 2)),
            Vector3.forward
        );

        //Side 3(-x)
        createCubeFaceMesh(
            new Vector3(location.x - (width / 2), location.y - (height / 2), location.z + (depth / 2)),
            new Vector3(location.x - (width / 2), location.y - (height / 2), location.z - (depth / 2)),
            new Vector3(location.x - (width / 2), location.y + (height / 2), location.z + (depth / 2)),
            new Vector3(location.x - (width / 2), location.y + (height / 2), location.z - (depth / 2)),
            -Vector3.right
        );

        //Side 4(+x)
        createCubeFaceMesh(
            new Vector3(location.x + (width / 2), location.y - (height / 2), location.z - (depth / 2)),
            new Vector3(location.x + (width / 2), location.y - (height / 2), location.z + (depth / 2)),
            new Vector3(location.x + (width / 2), location.y + (height / 2), location.z - (depth / 2)),
            new Vector3(location.x + (width / 2), location.y + (height / 2), location.z + (depth / 2)),
            Vector3.right
        );

        //Side 5(+y)
        createCubeFaceMesh(
            new Vector3(location.x + (width / 2), location.y + (height / 2), location.z - (depth / 2)),
            new Vector3(location.x + (width / 2), location.y + (height / 2), location.z + (depth / 2)),
            new Vector3(location.x - (width / 2), location.y + (height / 2), location.z - (depth / 2)),
            new Vector3(location.x - (width / 2), location.y + (height / 2), location.z + (depth / 2)),
            Vector3.up
        );

        //Side 6(-y)
        createCubeFaceMesh(
            new Vector3(location.x - (width / 2), location.y - (height / 2), location.z - (depth / 2)),
            new Vector3(location.x - (width / 2), location.y - (height / 2), location.z + (depth / 2)),
            new Vector3(location.x + (width / 2), location.y - (height / 2), location.z - (depth / 2)),
            new Vector3(location.x + (width / 2), location.y - (height / 2), location.z + (depth / 2)),
            -Vector3.up
        );

        UpdateMesh();
        mesh.Optimize();
    }

    //Create 1 face(Each Cube Consisting of 6)
    private void createCubeFaceMesh(Vector3 pointOne, Vector3 pointTwo, Vector3 pointThree, Vector3 pointFour, Vector3 direction)
    {
        //Create Vertices
        vertices.Add(pointOne);
        vertices.Add(pointTwo);
        vertices.Add(pointThree);
        vertices.Add(pointFour);

        int startPoint = vertices.Count;

        //Connect Verticies
        // lower left triangle
        triangles.Add(startPoint - 4 + 0);
        triangles.Add(startPoint - 4 + 2);
        triangles.Add(startPoint - 4 + 1);

        // upper right triangle
        triangles.Add(startPoint - 4 + 2);
        triangles.Add(startPoint - 4 + 3);
        triangles.Add(startPoint - 4 + 1);

        //Normals
        normals.Add(direction);
        normals.Add(direction);
        normals.Add(direction);
        normals.Add(direction);

        //Uv
        uv.Add(new Vector2(0, 0));
        uv.Add(new Vector2(1, 0));
        uv.Add(new Vector2(0, 1));
        uv.Add(new Vector2(1, 1));
    }

    //Generate the cube by calling the generate face 6 times
    //Feeding it proper positions
    private void removeCubeMesh(Vector3 location)
    {
        int v = 0;

        foreach (Vector3 pos in vertices)
        {
            //Check only the first vert created
            if(v % 24 == 0 && pos == location - new Vector3(0.5f,0.5f,0.5f))
            {
                
                break;
            }

            v++;
        }

        //If v reached the end of the vertices list, 
        //The position doesnt exist
        if (v == vertices.Count)
        {
            Debug.Log("Block Doesnt Exist at: " + location);
            return;
        }
        //Get the coefficient 
        v = v / 24;

        // Starting at the current vertice's location
        vertices.RemoveRange(v * 24, 24);
        
        triangles.RemoveRange(v * 36, 36);
        

        for (int i = v * 36; i < triangles.Count; i++)
        {
            triangles[i] -= 24;
        }

        normals.RemoveRange(v * 24, 24);
        
        uv.RemoveRange(v * 24, 24);

        

        UpdateMesh();
        mesh.Optimize();
    }
}
