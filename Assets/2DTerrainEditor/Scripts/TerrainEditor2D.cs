using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class TerrainEditor2D : MonoBehaviour
{
    // --- Variables

    #region Main values
    public int InstanceId { get; private set; }

    /// <summary>
    /// 2D terrain width in units
    /// </summary>
    public int Width = 50;
    /// <summary>
    /// 2D terrain height in units
    /// </summary>
    public int Height = 50;
    /// <summary>
    /// 2D terrain segments per unit
    /// </summary>
    public int Resolution = 1;
    #endregion

    #region 2D Terrain settings values
    /// <summary>
    /// Main material of this 2D terrain with tiled texture
    /// </summary>
    public Material MainMaterial;
    /// <summary>
    /// Number of main texture tiles for whole 2D terrain 
    /// </summary>
    public int MainTextureSize = 15;
    /// <summary>
    /// Color of 2D terrain mesh. Works only if shader supports vertex colors.
    /// </summary>
    public Color MainColor = Color.white;
    public int SortingLayerId;
    public string SortingLayerName;
    public int OrderInLayer;
    #endregion

    #region Random values
    /// <summary>
    /// Average height of randomly generated 2D terrain. WARNING: Value can not be more than <see cref="Height"/>
    /// </summary>
    public float RndHeight = 25;
    /// <summary>
    /// Max hills number of randomly generated 2D terrain. WARNING: Value can not be more than (<see cref="Width"/> - 1) / 2
    /// </summary>
    public int RndHillsCount = 5;
    /// <summary>
    /// Average height of randomly generated 2D terrain. WARNING: Value can not be more than <see cref="Height"/> / 2
    /// </summary>
    public float RndAmplitude = 5;
    #endregion

    #region Cap settings values
    /// <summary>
    /// Is cap needs to be generated for this 2D terrain?
    /// </summary>
    public bool GenerateCap;
    /// <summary>
    /// Cap material of this 2D terrain with tiled texture
    /// </summary>
    public Material CapMaterial;
    /// <summary>
    /// Number of cap texture tiles for whole 2D terrain (does not affect to cap with <see cref="SmartCap"/> enabled)
    /// </summary>
    public int CapTextureTiling = 15;
    /// <summary>
    /// Color of cap mesh. Works only if shader supports vertex colors.
    /// </summary>
    public Color CapColor = Color.white;
    public string CapSortingLayerName;
    public int CapSortingLayerId;
    public int CapOrderInLayer;
    /// <summary>
    /// Cap height in units
    /// </summary>
    public float CapHeight = 1;
    /// <summary>
    /// Cap offset relative to 2D terrain tip
    /// </summary>
    public float CapOffset;
    /// <summary>
    /// Is Smart Cap mode enabled for 2D terrain cap?
    /// </summary>
    public bool SmartCap = true;
    /// <summary>
    /// Min height in units for splitting cap into different paths
    /// </summary>
    public float SmartCapCutHeight = 1;
    /// <summary>
    /// Width of side segments in units
    /// </summary>
    public float SmartCapSideSegmentsWidth = 1;
    /// <summary>
    /// Horizontal UV for smart cap texture. It's like separator which divides cap texture into 3 parts.
    /// </summary>
    public Vector2 SmartCapSegmentsUv = new Vector2(0.125f, 0.875f);
    #endregion

    #region Deform settings values
    /// <summary>
    /// Is this terrain 2D can deform in realtime?
    /// </summary>
    public bool RealtimeDeformEnabled = false;
    /// <summary>
    /// Is UV texture coordinates should be updated on each iteration of deformation?
    /// </summary>
    public bool RealtimeDeformUpdateUv = true;
    /// <summary>
    /// Is colliders should be updated on each iteration of deformation?
    /// </summary>
    public bool RealtimeDeformUpdateColliders = true;
    #endregion

    #region Additional settings values
    /// <summary>
    /// Is side end points of terrain 2D path needs to be fixed and protected from changes? 
    /// </summary>
    public bool FixSides;
    /// <summary>
    /// This value will NOT be changed after applying random terrain generation.WARNING: Value can not be more than <see cref="Height"/>
    /// </summary>
    public float LeftFixedPoint = 25;
    /// <summary>
    /// This value will be changed after applying random terrain generation. WARNING: Value can not be more than <see cref="Height"/>
    /// </summary>
    public float RightFixedPoint = 25;
    /// <summary>
    /// Is 3D collider mesh needs to be generated on top of this 2D terrain?
    /// </summary>
    public bool Generate3DCollider;
    /// <summary>
    /// Width of 3D collider by Z
    /// </summary>
    public float Collider3DWidth = 5;
    #endregion

    #region References

    public GameObject CapObj;
    public GameObject Collider3DObj;
    public List<SmartCapArea> SmartCapAreas; 
    #endregion

    // --- public methods

    /// <summary>
    /// Generates new flat terrain based on main values
    /// </summary>
    public void Create()
    {
        Mesh pathMesh;
        if (gameObject.GetComponent<MeshFilter>().sharedMesh == null)
        {
            pathMesh = new Mesh();
            pathMesh.name = "Terrain2D_mesh_" + InstanceId;
            gameObject.GetComponent<MeshFilter>().mesh = pathMesh;
        }
        else
        {
            pathMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            pathMesh.Clear();
        }

        #region Generate vertices
        Vector3[] vertices = new Vector3[((Width * 2) * Resolution) + 2];

        for (int i = 0; i < vertices.Length; i += 2)
        {
            float vertsInterval = (i * 0.5f) / Resolution;

            vertices[i] = new Vector3(vertsInterval, Height * 0.5f, 0);
            vertices[i + 1] = new Vector3(vertsInterval, 0, 0);
        }

        if (FixSides)
        {
            vertices[0].y = LeftFixedPoint;
            vertices[vertices.Length - 2].y = RightFixedPoint;
        }
        #endregion

        #region Generate triangles
        int[] tris = new int[(vertices.Length - 2) * 3];

        bool toSide = false;
        int curTrisIndex = 0;
        for (int i = 0; i < vertices.Length - 2; i++)
        {
            if (toSide)
            {
                tris[curTrisIndex] = i;
                tris[curTrisIndex + 1] = i + 1;
                tris[curTrisIndex + 2] = i + 2;
            }
            else
            {
                tris[curTrisIndex] = i + 2;
                tris[curTrisIndex + 1] = i + 1;
                tris[curTrisIndex + 2] = i;
            }
            toSide = !toSide;

            curTrisIndex += 3;
        }
        #endregion

        #region Generate UV
        Vector2[] uv = new Vector2[vertices.Length];

        for (int i = 0; i < uv.Length; i += 2)
        {
            uv[i] = new Vector2((float)i / (uv.Length - 2) * MainTextureSize, (vertices[i].y / Height) * ((float)Height / Width) * MainTextureSize);
            uv[i + 1] = new Vector2((float)i / (uv.Length - 2) * MainTextureSize, (vertices[i + 1].y / Height) * ((float)Height / Width) * MainTextureSize);
        }
        #endregion

        pathMesh.vertices = vertices;
        pathMesh.triangles = tris;
        pathMesh.normals = GetMeshNormals(vertices.Length);
        pathMesh.uv = uv;
        pathMesh.SetColors(GetMeshColors(MainColor, vertices.Length).ToList());
        pathMesh.RecalculateBounds();

        SetSortingLayerAndOrder();

        UpdateCollider2D();
        UpdateCap(true);
        UpdateCollider3D(true);
    }

    /// <summary>
    /// Generates new or updates 2D collider based on terrain path
    /// </summary>
    public void UpdateCollider2D()
    {
        #region Update Edge Collider 2D
        if (GetComponent<EdgeCollider2D>() != null)
        {
            Vector3[] path = GetPath(Space.Self);
            Vector2[] colliderPath = new Vector2[path.Length];

            for (int i = 0; i < path.Length; i++)
            {
                colliderPath[i] = path[i];
            }


            GetComponent<EdgeCollider2D>().points = colliderPath;
        }
        #endregion

        #region Update Polygon Collider 2D
        /*
        Vector3[] vertices = GetVertsPos();
        Vector2[] points = new Vector2[vertices.Length];

        int point = 0;
        for (int i = 0; i < vertices.Length; i += 2)
        {
            if (i >= vertices.Length - 1)
                break;

            points[point] = new Vector2(vertices[i].x, vertices[i].y);
            point++;
        }

        for (int i = vertices.Length - 1; i > 1; i -= 2)
        {
            if (i <= 0)
                break;

            points[point] = new Vector2(vertices[i].x, vertices[i].y);
            point++;
        }

        gameObject.GetComponent<PolygonCollider2D>().SetPath(0, points);
        */
        #endregion

    }

    /// <summary>
    /// Generates new or updates mesh for 3D collider based on terrain path
    /// </summary>
    /// <param name="initalGeneration">Is collider generates initially?</param>
    /// <param name="updateShared">Is shared mesh of collider should be updated too?</param>
    public void UpdateCollider3D(bool updateShared)
    {
        #region Check object & mesh
        if (!Generate3DCollider)
        {
            if (Collider3DObj != null)
                DestroyImmediate(Collider3DObj);

            return;
        }
            
        if (Collider3DObj == null)
        {
            Collider3DObj = new GameObject("Collider 3D");
            Collider3DObj.transform.parent = transform;
            Collider3DObj.transform.position = transform.position;
            Collider3DObj.AddComponent<MeshFilter>();
            Collider3DObj.AddComponent<MeshCollider>();
        }

        Mesh collider3DMesh;
        if (Collider3DObj.GetComponent<MeshFilter>().sharedMesh == null)
        {
            collider3DMesh = new Mesh();
            collider3DMesh.name = "Terrain2D_collider3D_mesh_" + InstanceId;
            Collider3DObj.GetComponent<MeshFilter>().mesh = collider3DMesh;
        }
        else
        {
            if (updateShared)
            {
                collider3DMesh = Collider3DObj.GetComponent<MeshFilter>().sharedMesh;
                collider3DMesh.Clear();
            }
            else collider3DMesh = Collider3DObj.GetComponent<MeshFilter>().mesh;
        }
        #endregion

        Vector3[] vertices = GetVertsPos();
        bool initGeneration = collider3DMesh.vertexCount != vertices.Length;

        #region Generate vertices
        
        Vector3[] vertsPoints = new Vector3[vertices.Length];

        

        int point = 0;
        for (int i = 0; i < vertices.Length; i += 2)
        {
            vertsPoints[point] = new Vector3(vertices[i].x, vertices[i].y, -Collider3DWidth * 0.5f);
            vertsPoints[point + 1] = new Vector3(vertices[i].x, vertices[i].y, Collider3DWidth * 0.5f);
            point += 2;
        }

        collider3DMesh.vertices = vertsPoints;
        #endregion

        if (initGeneration)
        {
            #region Generate triangles
            int[] tris = new int[(vertsPoints.Length - 2) * 3];

            bool toSide = false;
            int curTrisIndex = 0;
            for (int i = 0; i < vertsPoints.Length - 2; i++)
            {
                if (toSide)
                {
                    tris[curTrisIndex] = i + 2;
                    tris[curTrisIndex + 1] = i + 1;
                    tris[curTrisIndex + 2] = i;
                }
                else
                {
                    tris[curTrisIndex] = i;
                    tris[curTrisIndex + 1] = i + 1;
                    tris[curTrisIndex + 2] = i + 2;
                }
                toSide = !toSide;

                curTrisIndex += 3;
            }

            collider3DMesh.triangles = tris;
            #endregion

            collider3DMesh.normals = GetMeshNormals(vertsPoints.Length);
        }

        Collider3DObj.GetComponent<MeshCollider>().enabled = false;
        Collider3DObj.GetComponent<MeshCollider>().enabled = true;

        Collider3DObj.GetComponent<MeshCollider>().sharedMesh = collider3DMesh;
    }

    /// <summary>
    /// Changes terrain geometry in realtime based on new path. Path can be obtained from <see cref="GetPath()"/> method
    /// </summary>
    /// <param name="newPath">Array of new terrain path points</param>
    public void ApplyDeform(Vector3[] newPath)
    {
        if (!RealtimeDeformEnabled)
            return;

        Mesh pathMesh = gameObject.GetComponent<MeshFilter>().mesh;

        if (FixSides)
        {
            newPath[0].y = LeftFixedPoint;
            newPath[newPath.Length - 1].y = RightFixedPoint;
        }

        #region Generate vertices
        Vector3[] vertices = GetVertsPos();
        int pathIndex = 0;
        for (int i = 0; i < vertices.Length; i += 2)
        {
            if (newPath[pathIndex].y < 0)
                newPath[pathIndex].y = 0;

            if (newPath[pathIndex].y > Height)
                newPath[pathIndex].y = Height;

            vertices[i] = newPath[pathIndex];
            pathIndex++;
        }
        
        pathMesh.vertices = vertices;
        #endregion 

        if (RealtimeDeformUpdateUv)
        {
            #region Generate UV
            Vector2[] uv = new Vector2[vertices.Length];

            for (int i = 0; i < uv.Length; i += 2)
            {
                uv[i] = new Vector2((float)i / (uv.Length - 2) * MainTextureSize, (vertices[i].y / Height) * ((float)Height / Width) * MainTextureSize);
                uv[i + 1] = new Vector2((float)i / (uv.Length - 2) * MainTextureSize, (vertices[i + 1].y / Height) * ((float)Height / Width) * MainTextureSize);
            }

            pathMesh.uv = uv;
            #endregion
        }

        pathMesh.RecalculateBounds();

        UpdateCap(false);
        if (RealtimeDeformUpdateColliders)
        {
            UpdateCollider2D();
            UpdateCollider3D(false);
        }
    }

    /// <summary>
    /// Updates terrain path based on <see cref="RndAmplitude"/>, <see cref="RndHillsCount"/> and <see cref="RndHeight"/> values
    /// </summary>
    /// <param name="updateShared">Shared mesh of terrain should be updated too?</param>
    public void RandomizeTerrain(bool updateShared)
    {
        Mesh pathMesh;
        if (updateShared)
            pathMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        else pathMesh = gameObject.GetComponent<MeshFilter>().mesh;

        #region Generate vertices
        Vector3[] vertices = gameObject.GetComponent<MeshFilter>().sharedMesh.vertices;

        int v = (Width * Resolution) / RndHillsCount;
        int step = 0;

        float rndHeightPoint = GetRandomHeightPoint();

        if (FixSides)
            vertices[0].y = LeftFixedPoint;
        else
        {
            vertices[0].y = rndHeightPoint;
            rndHeightPoint = GetRandomHeightPoint();
        }

        for (int i = 2; i < vertices.Length; i += 2)
        {
            if (step >= v)
            {
                rndHeightPoint = GetRandomHeightPoint();
                step = 0;
            }

            vertices[i].y = Mathf.Lerp(vertices[i - 2].y, rndHeightPoint, ((float)step / v) / Resolution);
            step++;
        }

        if (FixSides)
            RightFixedPoint = vertices[vertices.Length - 2].y;
        #endregion

        pathMesh.vertices = vertices;
        pathMesh.uv = GetMeshUv(vertices);
        pathMesh.RecalculateBounds();
        
        UpdateCap(updateShared);
        UpdateCollider2D();
        UpdateCollider3D(updateShared);
    }
    
    /// <summary>
    /// Updates terrain path based on <see cref="RndAmplitude"/>, <see cref="RndHillsCount"/> and <see cref="RndHeight"/> values and given left initial point
    /// </summary>
    /// <param name="leftInitialVertexPos">Vertex point in local space from which generation will start for this terrain mesh. Can be obtained from <see cref="GetLastVertexPos()"/> method</param>
    /// <param name="updateShared">Shared mesh of terrain should be updated too?</param>
    public void RandomizeTerrain(Vector3 leftInitialVertexPos, bool updateShared)
    {
        Mesh pathMesh;
        if (updateShared)
            pathMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        else pathMesh = gameObject.GetComponent<MeshFilter>().mesh;

        #region Generate vertices
        Vector3[] vertices = gameObject.GetComponent<MeshFilter>().sharedMesh.vertices;

        int v = (Width * Resolution) / RndHillsCount;
        int step = 0;

        float a = GetRandomHeightPoint();
        vertices[0].y = leftInitialVertexPos.y;

        for (int i = 2; i < vertices.Length; i += 2)
        {
            if (step >= v)
            {
                a = GetRandomHeightPoint();
                step = 0;
            }

            if (!FixSides)
                vertices[i].y = Mathf.Lerp(vertices[i - 2].y, a, ((float)step / v) / Resolution);
            else
            {
                vertices[i].y = Mathf.Lerp(vertices[i - 2].y, a, ((float)step / v) / Resolution);
            }

            step++;
        }
        #endregion

        pathMesh.vertices = vertices;
        pathMesh.uv = GetMeshUv(vertices);
        pathMesh.RecalculateBounds();

        UpdateCap(updateShared);
        UpdateCollider2D();
        UpdateCollider3D(updateShared);
    }

    /// <summary>
    /// Sets sorting layer and order for terrain and cap
    /// </summary>
    public void SetSortingLayerAndOrder()
    {
        gameObject.GetComponent<MeshRenderer>().sortingLayerName = SortingLayerName;
        gameObject.GetComponent<MeshRenderer>().sortingOrder = OrderInLayer;

        if (CapObj != null)
        {
            CapObj.GetComponent<MeshRenderer>().sortingLayerName = CapSortingLayerName;
            CapObj.GetComponent<MeshRenderer>().sortingOrder = CapOrderInLayer;
        }
    }

    /// <summary>
    /// Returns last (upper right) vertex point position of terrain mesh. This value can be used as initial point in <see cref="RandomizeTerrain()"/> method for left-to-right random generation
    /// </summary>
    /// <returns>Last (upper right) vertex point position in local space</returns>
    public Vector3 GetLastVertexPos()
    {
        int vertsCount = gameObject.GetComponent<MeshFilter>().sharedMesh.vertices.Length;

        return gameObject.GetComponent<MeshFilter>().sharedMesh.vertices[vertsCount - 2];
    }

    /// <summary>
    /// Returns all vertices points of this terrain mesh
    /// </summary>
    /// <returns>Array of all vertices points</returns>
    public Vector3[] GetVertsPos()
    {
        if (gameObject.GetComponent<MeshFilter>().sharedMesh == null)
            return null;
        
        return gameObject.GetComponent<MeshFilter>().sharedMesh.vertices;
    }

    /// <summary>
    /// Return path of this terrain
    /// </summary>
    /// <param name="relativeSpace">Relative to local or world space</param>
    /// <returns>Array of path points</returns>
    public Vector3[] GetPath(Space relativeSpace)
    {
        Mesh terrainMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

        if (terrainMesh == null)
            return null;

        Vector3[] path = new Vector3[terrainMesh.vertexCount / 2];
        int vertIndex = 0;
        if (relativeSpace == Space.Self)
        {
            for (int i = 0; i < terrainMesh.vertexCount; i += 2)
            {
                path[vertIndex] = terrainMesh.vertices[i];
                vertIndex++;
            }
        }
        else
        {
            for (int i = 0; i < terrainMesh.vertexCount; i += 2)
            {
                path[vertIndex] = terrainMesh.vertices[i] + transform.position;
                vertIndex++;
            }
        }

        return path;
    }

    /// <summary>
    /// Returns all vertices points of cap mesh
    /// </summary>
    /// <returns>Array of all vertices points</returns>
    public Vector3[] GetVertsPosCap()
    {
        if (!GenerateCap || CapObj == null)
            return null;

        if (CapObj.GetComponent<MeshFilter>() == null)
            return null;

        if (CapObj.GetComponent<MeshFilter>().sharedMesh == null)
            return null;

        return CapObj.GetComponent<MeshFilter>().sharedMesh.vertices;
    }


    // --- Unity Editor methods

    /// <summary>
    /// Sets mesh ID for this terrain (used only in Unity Editor by custom inspector)
    /// </summary>
    /// <param name="instanceId">Component instance id</param>
    public void SetInstanceId(int instanceId)
    {
        InstanceId = instanceId;
    }

    /// <summary>
    /// Fully updates terrain based on given vertices array (used only in Unity Editor by custom inspector)
    /// </summary>
    /// <param name="newVertsPos">New vertices array</param>
    /// <param name="updateShared">Shared mesh of terrain should be updated too?</param>
    public void EditTerrain(Vector3[] newVertsPos, bool updateShared)
    {
        Mesh pathMesh;
        if (updateShared)
            pathMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        else pathMesh = gameObject.GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = newVertsPos;

        if (FixSides)
        {
            vertices[0].y = LeftFixedPoint;
            vertices[vertices.Length - 2].y = RightFixedPoint;
        }

        #region Generate UV
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0; i < uv.Length; i += 2)
        {
            uv[i] = new Vector2((float)i / (uv.Length - 2) * MainTextureSize, (vertices[i].y / Height) * ((float)Height / Width) * MainTextureSize);
            uv[i + 1] = new Vector2((float)i / (uv.Length - 2) * MainTextureSize, (vertices[i + 1].y / Height) * ((float)Height / Width) * MainTextureSize);
        }
        #endregion

        pathMesh.vertices = vertices;
        pathMesh.uv = uv;
        pathMesh.SetColors(GetMeshColors(MainColor, vertices.Length).ToList());
        pathMesh.RecalculateBounds();

        SetSortingLayerAndOrder();
        
        UpdateCap(updateShared);
        UpdateCollider3D(updateShared);
    }

    
    // --- private methods

    /// <summary>
    /// Creates new or updates cap
    /// </summary>
    /// <param name="initialGeneration">Is cap generates initially?</param>
    /// <param name="updateShared">Shared mesh of cap should be updated too?</param>
    void UpdateCap(bool updateShared)
    {
        #region Check object & mesh
        if (!GenerateCap)
        {
            if (CapObj != null)
                DestroyImmediate(CapObj);

            return;
        }

        if (CapObj == null)
        {
            CapObj = new GameObject("Cap");
            CapObj.transform.position = transform.position;
            CapObj.transform.parent = transform;
            CapObj.AddComponent<MeshFilter>();
            CapObj.AddComponent<MeshRenderer>();
        }

        if (SmartCap)
        {
            UpdateCapSmart(updateShared);
            return;
        }

        Mesh capMesh;
        if (CapObj.GetComponent<MeshFilter>().sharedMesh == null)
        {
            capMesh = new Mesh();
            capMesh.name = "Terrain2D_cap_mesh_" + InstanceId;
            CapObj.GetComponent<MeshFilter>().mesh = capMesh;
        }
        else
        {
            if (updateShared)
            {
                capMesh = CapObj.GetComponent<MeshFilter>().sharedMesh;
                capMesh.Clear();
            }
            else capMesh = CapObj.GetComponent<MeshFilter>().mesh;
        }
        #endregion

        Vector3[] vertices = GetVertsPos();
        bool initGeneration = vertices.Length != capMesh.vertexCount;

        #region Generate vertices
        

        for (int i = 0; i < vertices.Length; i += 2)
        {
            vertices[i] = new Vector3(vertices[i].x, vertices[i].y, -0.01f);
            vertices[i + 1] = new Vector3(vertices[i + 1].x, vertices[i].y - CapHeight, -0.01f);
            vertices[i].y += CapOffset;
            vertices[i + 1].y += CapOffset;
        }

        capMesh.vertices = vertices;
        #endregion

        if (initGeneration)
        {
            #region Generate triangles
            int[] tris = new int[(vertices.Length - 2) * 3];

            bool toSide = false;
            int curTrisIndex = 0;
            for (int i = 0; i < vertices.Length - 2; i++)
            {
                if (toSide)
                {
                    tris[curTrisIndex] = i;
                    tris[curTrisIndex + 1] = i + 1;
                    tris[curTrisIndex + 2] = i + 2;
                }
                else
                {
                    tris[curTrisIndex] = i + 2;
                    tris[curTrisIndex + 1] = i + 1;
                    tris[curTrisIndex + 2] = i;
                }
                toSide = !toSide;

                curTrisIndex += 3;
            }

            capMesh.triangles = tris;
            #endregion

            capMesh.normals = GetMeshNormals(vertices.Length);
        }

        #region Generate UV
        Vector2[] uv = new Vector2[vertices.Length];

        for (int i = 0; i < uv.Length; i += 2)
        {
            uv[i] = new Vector2((float)i / (uv.Length - 2) * CapTextureTiling, 1);
            uv[i + 1] = new Vector2((float)i / (uv.Length - 2) * CapTextureTiling, 0);
        }

        capMesh.uv = uv;
        #endregion

        capMesh.SetColors(GetMeshColors(CapColor, vertices.Length).ToList());
        capMesh.RecalculateBounds();

        CapObj.GetComponent<Renderer>().material = CapMaterial;
    }

    /// <summary>
    /// Updates smart cap
    /// </summary>
    /// <param name="updateShared">Shared mesh of cap should be updated too?</param>
    void UpdateCapSmart(bool updateShared)
    {
        #region Check mesh
        Mesh capMesh;
        if (CapObj.GetComponent<MeshFilter>().sharedMesh == null)
        {
            capMesh = new Mesh();
            capMesh.name = "Terrain2D_cap_mesh_" + InstanceId;
            CapObj.GetComponent<MeshFilter>().mesh = capMesh;
        }
        else
        {
            if (updateShared)
            {
                capMesh = CapObj.GetComponent<MeshFilter>().sharedMesh;
                capMesh.Clear();
            }
            else capMesh = CapObj.GetComponent<MeshFilter>().mesh;

        }
        #endregion

        Vector3[] vertices = GetVertsPos();
        SmartCapAreas = new List<SmartCapArea>();

        #region Create smart cap areas
        SmartCapArea curSmartCapArea = new SmartCapArea();
        bool nextSmartCapArea = false;
        int counterSign = 1;
        for (int i = 0; i < vertices.Length; i += 2)
        {
            if (i == vertices.Length - 2)
                counterSign = -1;

            if (Mathf.Abs(vertices[i].y - vertices[i + (2 * counterSign)].y) < SmartCapCutHeight)
            {
                if (!nextSmartCapArea)
                {
                    SmartCapAreas.Add(new SmartCapArea());
                    curSmartCapArea = SmartCapAreas[SmartCapAreas.Count - 1];
                    nextSmartCapArea = true;
                }

                curSmartCapArea.VertsPoints.Add(new Vector3(vertices[i].x, vertices[i].y + CapOffset, -0.01f));
                curSmartCapArea.VertsPoints.Add(new Vector3(vertices[i + 1].x, vertices[i].y - CapHeight + CapOffset, -0.01f));
            }
            else
            {
                if (nextSmartCapArea)
                {
                    curSmartCapArea.VertsPoints.Add(new Vector3(vertices[i].x, vertices[i].y + CapOffset, -0.01f));
                    curSmartCapArea.VertsPoints.Add(new Vector3(vertices[i + 1].x, vertices[i].y - CapHeight + CapOffset, -0.01f));
                }
                nextSmartCapArea = false;
            }
        }

        #endregion

        #region Remove too small areas
        for (int i = SmartCapAreas.Count - 1; i >= 0; i--)
        {
            SmartCapAreas[i].MainSegmentsCount = (SmartCapAreas[i].VertsPoints.Count - 2) / 2;
            if (SmartCapAreas[i].MainSegmentsCount < Resolution)
                SmartCapAreas.RemoveAt(i);
        }
        #endregion

        #region Combine all vertices
        List<Vector3> totalCapVertices = new List<Vector3>();
        Vector3[] sideVertices = new Vector3[4];
        for (int i = 0; i < SmartCapAreas.Count; i++)
        {
            curSmartCapArea = SmartCapAreas[i];

            //Add side segments vertices
            sideVertices[0] = curSmartCapArea.VertsPoints[0] - new Vector3(SmartCapSideSegmentsWidth, 0, 0);
            sideVertices[1] = curSmartCapArea.VertsPoints[1] - new Vector3(SmartCapSideSegmentsWidth, 0, 0);
            sideVertices[2] = curSmartCapArea.VertsPoints[curSmartCapArea.VertsPoints.Count - 1] + new Vector3(SmartCapSideSegmentsWidth, 0, 0);
            sideVertices[3] = curSmartCapArea.VertsPoints[curSmartCapArea.VertsPoints.Count - 2] + new Vector3(SmartCapSideSegmentsWidth, 0, 0);

            sideVertices[0].y += (sideVertices[0].y - curSmartCapArea.VertsPoints[2].y);
            sideVertices[1].y += (sideVertices[1].y - curSmartCapArea.VertsPoints[3].y);
            sideVertices[2].y += (sideVertices[2].y - curSmartCapArea.VertsPoints[curSmartCapArea.VertsPoints.Count - 3].y);
            sideVertices[3].y += (sideVertices[3].y - curSmartCapArea.VertsPoints[curSmartCapArea.VertsPoints.Count - 4].y);

            curSmartCapArea.VertsPoints.Insert(0, sideVertices[1]);
            curSmartCapArea.VertsPoints.Insert(0, sideVertices[0]);
            curSmartCapArea.VertsPoints.Add(sideVertices[3]);
            curSmartCapArea.VertsPoints.Add(sideVertices[2]);

            for (int j = 0; j < curSmartCapArea.VertsPoints.Count; j += 2)
            {
                totalCapVertices.Add(curSmartCapArea.VertsPoints[j]);
                totalCapVertices.Add(curSmartCapArea.VertsPoints[j + 1]);

                if (j >= 2 && j <= curSmartCapArea.VertsPoints.Count - 3)
                {
                    totalCapVertices.Add(curSmartCapArea.VertsPoints[j]);
                    totalCapVertices.Add(curSmartCapArea.VertsPoints[j + 1]);
                }

            }
        }
        #endregion

        #region Generate UV
        Vector2[] uv = new Vector2[totalCapVertices.Count];
        float uvPerUnit = (SmartCapSegmentsUv.y - SmartCapSegmentsUv.x) / Resolution;
        int uvIndex = 0;
        for (int i = 0; i < SmartCapAreas.Count; i++)
        {
            curSmartCapArea = SmartCapAreas[i];

            uv[uvIndex] = new Vector2(0, 1);
            uv[uvIndex + 1] = new Vector2(0, 0);
            uv[uvIndex + 2] = new Vector2(SmartCapSegmentsUv.x, 1);
            uv[uvIndex + 3] = new Vector2(SmartCapSegmentsUv.x, 0);

            uvIndex += 4;

            int unitCounter = 0;
            int uvTotalVerts = curSmartCapArea.VertsPoints.Count + (curSmartCapArea.VertsPoints.Count - 14);
            for (int j = 0; j < uvTotalVerts; j += 4)
            {
                uv[uvIndex] = new Vector2(SmartCapSegmentsUv.x + (uvPerUnit * unitCounter), 1);
                uv[uvIndex + 1] = new Vector2(SmartCapSegmentsUv.x + (uvPerUnit * unitCounter), 0);
                uv[uvIndex + 2] = new Vector2(SmartCapSegmentsUv.x + (uvPerUnit * (unitCounter + 1)), 1);
                uv[uvIndex + 3] = new Vector2(SmartCapSegmentsUv.x + (uvPerUnit * (unitCounter + 1)), 0);

                uvIndex += 4;

                unitCounter++;
                if (unitCounter >= Resolution)
                    unitCounter = 0;
            }

            if (uv[uvIndex - 1].x < SmartCapSegmentsUv.y)
            {
                SmartCapAreas[i].CorruptedTilesVertsPoints.Add(totalCapVertices[uvIndex - 1]);
                SmartCapAreas[i].CorruptedTilesVertsPoints.Add(totalCapVertices[uvIndex - 2]);
            }

            uv[uvIndex] = new Vector2(SmartCapSegmentsUv.y, 1);
            uv[uvIndex + 1] = new Vector2(SmartCapSegmentsUv.y, 0);
            uv[uvIndex + 2] = new Vector2(1, 1);
            uv[uvIndex + 3] = new Vector2(1, 0);

            uvIndex += 4;
        }

        #endregion

        #region Generate triangles

        int unusedVertsCount = 0;
        foreach (var smartCapArea in SmartCapAreas)
        {
            unusedVertsCount += smartCapArea.VertsPoints.Count - 4;
        }

        int[] totalCapTris = new int[((totalCapVertices.Count - unusedVertsCount) - 2) * 3];
        int curTrisIndex = 0;
        int vertCounter = 0;
        bool toSide = false;
        for (int i = 0; i < SmartCapAreas.Count; i++)
        {
            SmartCapArea curArea = SmartCapAreas[i];

            int skipVertCounter = 0;
            for (int j = 2; j < curArea.VertsPoints.Count + (curArea.VertsPoints.Count - 4); j++)
            {
                if (skipVertCounter > 1)
                {
                    vertCounter += 2;
                    j += 2;
                    skipVertCounter = 0;
                }

                if (toSide)
                {
                    totalCapTris[curTrisIndex] = vertCounter;
                    totalCapTris[curTrisIndex + 1] = vertCounter + 1;
                    totalCapTris[curTrisIndex + 2] = vertCounter + 2;
                }
                else
                {
                    totalCapTris[curTrisIndex] = vertCounter + 2;
                    totalCapTris[curTrisIndex + 1] = vertCounter + 1;
                    totalCapTris[curTrisIndex + 2] = vertCounter;
                }
                toSide = !toSide;

                vertCounter++;
                curTrisIndex += 3;
                skipVertCounter++;
            }

            vertCounter += 2;

            toSide = false;
        }
        #endregion

        capMesh.SetVertices(totalCapVertices);
        capMesh.triangles = totalCapTris;
        capMesh.normals = GetMeshNormals(totalCapVertices.Count);
        capMesh.uv = uv;
        capMesh.SetColors(GetMeshColors(CapColor, totalCapVertices.Count).ToList());
        capMesh.RecalculateBounds();
    }
    
    /// <summary>
    /// Returns height point based on Rnd values
    /// </summary>
    /// <returns>Random height point</returns>
    float GetRandomHeightPoint() 
    {
        float a = RndHeight - RndAmplitude;
        float b = RndHeight + RndAmplitude;

        if (a < 0.1f)
            a = 0.1f;
        if (b > Height)
            b = Height;

        return Random.Range(a, b);
    }

    /// <summary>
    /// Returns normals of terrain mesh
    /// </summary>
    Vector3[] GetMeshNormals(int vertsCount)
    {
        Vector3[] normals = new Vector3[vertsCount];

        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -Vector3.forward;
        }

        return normals;
    }

    /// <summary>
    /// Returns UV coordinates of terrain mesh 
    /// </summary>
    /// <param name="vertices">Array of vertices</param>
    /// <returns>Array of UV coordinates</returns>
    Vector2[] GetMeshUv(Vector3[] vertices)
    {
        Vector2[] uv = new Vector2[vertices.Length];

        for (int i = 0; i < uv.Length; i += 2)
        {
            uv[i] = new Vector2((float)i / (uv.Length - 2) * MainTextureSize, (vertices[i].y / Height) * ((float)Height / Width) * MainTextureSize);
            uv[i + 1] = new Vector2((float)i / (uv.Length - 2) * MainTextureSize, (vertices[i + 1].y / Height) * ((float)Height / Width) * MainTextureSize);
        }

        return uv;
    }

    /// <summary>
    /// Returns array of colors for mesh
    /// </summary>
    /// <param name="col">General color</param>
    /// <param name="vertsCount">vertices count</param>
    /// <returns></returns>
    Color[] GetMeshColors(Color col, int vertsCount)
    {
        Color[] colors = new Color[vertsCount];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = col;
        }

        return colors;
    }


    // --- static methods
    
    /// <summary>
    /// Instantiates new 2D terrain
    /// </summary>
    /// <param name="position">Position in world space</param>
    /// <returns>2D terrain game object</returns>
    public static GameObject InstantiateTerrain2D(Vector3 position)
    {
        GameObject newTerrain = new GameObject("New Terrain 2D");
        newTerrain.transform.position = position;
        newTerrain.AddComponent<TerrainEditor2D>();

        newTerrain.GetComponent<TerrainEditor2D>().Create();

        return newTerrain;
    }
    
    /// <summary>
    /// Instantiates new 2D terrain
    /// </summary>
    /// <param name="position">Position in world space</param>
    /// <param name="width">Width in units</param>
    /// <param name="height">Height in units</param>
    /// <param name="resolution">Mesh esolution</param>
    /// <returns>2D terrain game object</returns>
    public static GameObject InstantiateTerrain2D(Vector3 position, int width, int height, int resolution)
    {
        GameObject newTerrain = new GameObject("New Terrain 2D");
        newTerrain.transform.position = position;
        newTerrain.AddComponent<TerrainEditor2D>();

        newTerrain.GetComponent<TerrainEditor2D>().Width = width;
        newTerrain.GetComponent<TerrainEditor2D>().Height = height;
        newTerrain.GetComponent<TerrainEditor2D>().Resolution = resolution;

        newTerrain.GetComponent<TerrainEditor2D>().Create();

        return newTerrain;
    }


    // --- internal classes

    [Serializable]
    public class SmartCapArea
    {
        public List<Vector3> VertsPoints = new List<Vector3>();
        public int MainSegmentsCount = 0;
        public List<Vector3> CorruptedTilesVertsPoints = new List<Vector3>(); 

    }
}


