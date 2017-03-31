using UnityEngine;

[ExecuteInEditMode] 
public class PolygonTester : MonoBehaviour
{
    public enum Direction { up, down, left, right };
    public Direction direction;
    public float downAmountX = 20f;
    public float downAmountY = 20f;

    public Material mat;
    Mesh msh;

    void Start()
    {
        InvokeRepeating("Thing", 0.1f, 0.1f);

    }
    void Thing()
    {
        
            msh = new Mesh();

            // Create Vector2 vertices

            Vector2[] vertices2D = new Vector2[GetComponent<BezierCollider2D>().pointsQuantity + 3];
            for (int i = 0; i < GetComponent<BezierCollider2D>().pointsQuantity + 3; i++)
            {
                if (i == GetComponent<BezierCollider2D>().pointsQuantity + 1)
                {
                    switch (direction)
                    {
                        case Direction.up:
                            vertices2D[GetComponent<BezierCollider2D>().pointsQuantity + 1] = new Vector2(GetComponent<BezierCollider2D>().points[i - 1].x, GetComponent<BezierCollider2D>().points[i - 1].y + downAmountX);
                            break;

                        case Direction.down:
                            vertices2D[GetComponent<BezierCollider2D>().pointsQuantity + 1] = new Vector2(GetComponent<BezierCollider2D>().points[i - 1].x, GetComponent<BezierCollider2D>().points[i - 1].y - downAmountX);
                            break;

                        case Direction.left:
                            vertices2D[GetComponent<BezierCollider2D>().pointsQuantity + 1] = new Vector2(GetComponent<BezierCollider2D>().points[i - 1].x - downAmountX, GetComponent<BezierCollider2D>().points[i - 1].y);
                            break;

                        case Direction.right:
                            vertices2D[GetComponent<BezierCollider2D>().pointsQuantity + 1] = new Vector2(GetComponent<BezierCollider2D>().points[i - 1].x + downAmountX, GetComponent<BezierCollider2D>().points[i - 1].y);
                            break;
                    }

                }
                else if (i == GetComponent<BezierCollider2D>().pointsQuantity + 2)
                {
                    switch (direction)
                    {
                        case Direction.up:
                            vertices2D[GetComponent<BezierCollider2D>().pointsQuantity + 2] = new Vector2(GetComponent<BezierCollider2D>().points[0].x, GetComponent<BezierCollider2D>().points[0].y + downAmountY);
                            break;

                        case Direction.down:
                            vertices2D[GetComponent<BezierCollider2D>().pointsQuantity + 2] = new Vector2(GetComponent<BezierCollider2D>().points[0].x, GetComponent<BezierCollider2D>().points[0].y - downAmountY);
                            break;

                        case Direction.left:
                            vertices2D[GetComponent<BezierCollider2D>().pointsQuantity + 2] = new Vector2(GetComponent<BezierCollider2D>().points[0].x - downAmountY, GetComponent<BezierCollider2D>().points[0].y);
                            break;

                        case Direction.right:
                            vertices2D[GetComponent<BezierCollider2D>().pointsQuantity + 2] = new Vector2(GetComponent<BezierCollider2D>().points[0].x + downAmountY, GetComponent<BezierCollider2D>().points[0].y);
                            break;
                    }

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


            TestUvRoofMap(msh);
            // Create the mesh
            msh.vertices = vertices;
            msh.triangles = indices;
            msh.RecalculateNormals();
            msh.RecalculateBounds();

            // Set up game object with mesh;
            GetComponent<MeshRenderer>().material = mat;
            MeshFilter filter = gameObject.GetComponent<MeshFilter>();
            filter.mesh = msh;
        
    }

    private void TestUvRoofMap(Mesh mesh)
    {
        var newUvs = new Vector2[mesh.vertices.Length];
        for (var v = 0; v < mesh.vertices.Length; v++)
        {
            var vertex = mesh.vertices[v];
            var normal = mesh.normals[v];
            // This gives a vector pointing up the roof:
            var vAxis = Vector3.Scale(normal, new Vector3(-1, 0, -1)).normalized;
            // This will make the u axis perpendicular to the v axis (ie. parallel to the roof edge)
            var uAxis = new Vector3(vAxis.y, 0, -vAxis.x);
            // I originally used vAxis here, but changed to position.y so you get more predticable alignment at edges.
            // Set eaveHeight to the y coordinate of the bottom edge of the roof.
            var uv = new Vector2(Vector3.Dot(vertex, uAxis), vertex.y - 0);

            newUvs[v] = uv;

            // You may need to scale the uv vector's x and y to get the aspect ratio you want.
            // The scale factor will vary with the roof's slope.
        }
        mesh.uv = newUvs;
    }
}