using UnityEngine;

[ExecuteInEditMode] 
public class PolygonTester : MonoBehaviour
{
    public Material mat;
    Mesh msh;

    void Start()
    {
    }
    void Update()
    {
        msh = new Mesh();


        // Create Vector2 vertices

        Vector2[] vertices2D = new Vector2[GetComponent<BezierCollider2D>().pointsQuantity+3];
        for(int i = 0; i < GetComponent<BezierCollider2D>().pointsQuantity+3; i++)
        {
            if (i == GetComponent<BezierCollider2D>().pointsQuantity+1)
            {
                vertices2D[GetComponent<BezierCollider2D>().pointsQuantity+1] = new Vector2(GetComponent<BezierCollider2D>().points[i-1].x, GetComponent<BezierCollider2D>().points[i-1].y - 20);

            }
            else if (i == GetComponent<BezierCollider2D>().pointsQuantity + 2)
            {
                vertices2D[GetComponent<BezierCollider2D>().pointsQuantity + 2] = new Vector2(GetComponent<BezierCollider2D>().points[0].x, GetComponent<BezierCollider2D>().points[0].y - 20);

            }
            else
            vertices2D[i] = GetComponent<BezierCollider2D>().points[i];
        }

        

        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        // Create the mesh
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        //GetComponent<MeshRenderer>().material = mat;
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        filter.mesh = msh;
    }
}