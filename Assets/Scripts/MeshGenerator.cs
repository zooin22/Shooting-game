////https://unity3d.com/es/learn/tutorials/s/procedural-cave-generation-tutorial 응용
//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class MeshGenerator : MonoBehaviour
//{
//    //List<Vector3> vertices;
//    //List<int> triangles;
//    public MeshFilter walls;
//    public MeshFilter floors;
//    public Texture2D floorTexture;
//    public Transform b;
//    HashSet<int> checkedVertices = new HashSet<int>();
//    List<List<int>> outlines = new List<List<int>>();
//    Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();

//    public void GenerateMesh(Node[,] map, float squareSize)
//    {
//        InitData();

//        SquareGrid squareGrid;

//        squareGrid = new SquareGrid(map, squareSize);

//        List<Vector3> vertices = new List<Vector3>();
//        List<int>  triangles = new List<int>();

//        for (int x = 0; x < squareGrid.wallSquares.GetLength(0); x++)
//        {
//            for (int y = 0; y < squareGrid.wallSquares.GetLength(1); y++)
//            {
//                TriangulateSquare(vertices, triangles, squareGrid.wallSquares[x, y]);
//            }
//        }
//        CreateTexture(vertices, triangles, walls);
//        Generate2DColliders(vertices);

//        vertices = new List<Vector3>();
//        triangles = new List<int>();

//        for (int x = 0; x < squareGrid.floorSquares.GetLength(0); x++)
//        {
//            for (int y = 0; y < squareGrid.floorSquares.GetLength(1); y++)
//            {
//                TriangulateSquare(vertices, triangles, squareGrid.floorSquares[x, y]);
//            }
//        }
//        CreateTexture(vertices, triangles, floors);
//    }
    
//#region mesh
//    void InitData()
//    {
//        triangleDictionary.Clear();
//        outlines.Clear();
//        checkedVertices.Clear();
//    }

//    void TriangulateSquare(List<Vector3> vertices,List<int> triangles, Square square)
//    {
//        switch (square.configuration)
//        {
//            case 0:
//                break;

//            // 1 points:
//            case 1:
//                MeshFromPoints(vertices, triangles, square.centreBottom, square.bottomLeft, square.centreLeft);
//                break;
//            case 2:
//                MeshFromPoints(vertices, triangles, square.centreRight, square.bottomRight, square.centreBottom);
//                break;
//            case 4:
//                MeshFromPoints(vertices, triangles, square.centreTop, square.topRight, square.centreRight);
//                break;
//            case 8:
//                MeshFromPoints(vertices, triangles, square.topLeft, square.centreTop, square.centreLeft);
//                break;

//            // 2 points:
//            case 3:
//                MeshFromPoints(vertices, triangles, square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
//                break;
//            case 6:
//                MeshFromPoints(vertices, triangles, square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
//                break;
//            case 9:
//                MeshFromPoints(vertices, triangles, square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
//                break;
//            case 12:
//                MeshFromPoints(vertices, triangles, square.topLeft, square.topRight, square.centreRight, square.centreLeft);
//                break;
//            case 5:
//                MeshFromPoints(vertices, triangles, square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
//                break;
//            case 10:
//                MeshFromPoints(vertices, triangles, square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
//                break;

//            // 3 point:
//            case 7:
//                MeshFromPoints(vertices, triangles, square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
//                break;
//            case 11:
//                MeshFromPoints(vertices, triangles, square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
//                break;
//            case 13:
//                MeshFromPoints(vertices, triangles, square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
//                break;
//            case 14:
//                MeshFromPoints(vertices, triangles, square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
//                break;

//            // 4 point:
//            case 15:
//                MeshFromPoints(vertices, triangles, square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
//                break;
//        }
//    }

//    void MeshFromPoints(List<Vector3> vertices,List<int> triangles,params MeshNode[] points)
//    {
//        AssignVertices(vertices, points);

//        if (points.Length >= 3)
//            CreateTriangle(triangles, points[0], points[1], points[2]);
//        if (points.Length >= 4) 
//            CreateTriangle(triangles, points[0], points[2], points[3]);
//        if (points.Length >= 5)
//            CreateTriangle(triangles, points[0], points[3], points[4]);
//        if (points.Length >= 6)
//            CreateTriangle(triangles, points[0], points[4], points[5]);
//    }

//    void AssignVertices(List<Vector3> vertices, MeshNode[] points)
//    {
//        for (int i = 0; i < points.Length; i++)
//        {
//            if (points[i].vertexIndex == -1)
//            {
//                points[i].vertexIndex = vertices.Count;
//                vertices.Add(points[i].position);
//            }
//        }
//    }

//    void CreateTriangle(List<int> triangles, MeshNode a, MeshNode b, MeshNode c)
//    {
//        triangles.Add(a.vertexIndex);
//        triangles.Add(b.vertexIndex);
//        triangles.Add(c.vertexIndex);

//        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
//        AddTriangleToDictionary(triangle.vertexIndexA, triangle);
//        AddTriangleToDictionary(triangle.vertexIndexB, triangle);
//        AddTriangleToDictionary(triangle.vertexIndexC, triangle);
//    }
//    #endregion
//#region texture
//    void CreateTexture(List<Vector3> vertices, List<int> triangles, MeshFilter meshFilter)
//    {
//        Mesh mesh = new Mesh();

//        mesh.vertices = vertices.ToArray();
//        mesh.triangles = triangles.ToArray();
//        mesh.RecalculateNormals();

//        var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);

//        // set the pixel values
//        texture.SetPixel(0, 0, new Color(1.0f, 1.0f, 1.0f, 0.5f));
//        texture.SetPixel(1, 0, Color.clear);
//        texture.SetPixel(0, 1, Color.white);
//        texture.SetPixel(1, 1, Color.black);

//        meshFilter.mesh = mesh;
//    }
//#endregion
//#region collider
//    public void Generate2DColliders(List<Vector3> vertices) // edge collider 만들기
//    {
//        EdgeCollider2D[] currentColliders = gameObject.GetComponents<EdgeCollider2D>();
//        for (int i = 0; i < currentColliders.Length; i++)
//        {
//            Destroy(currentColliders[i]);
//        }

//        CalculateMeshOutlines(vertices.Count);

//        foreach (List<int> outline in outlines)
//        {
//            EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
//            Vector2[] edgePoints = new Vector2[outline.Count];

//            for (int i = 0; i < outline.Count; i++)
//            {
//                edgePoints[i] = new Vector2(vertices[outline[i]].x, vertices[outline[i]].y);
//            }
//            edgeCollider.points = edgePoints;
//        }

//    }

//    void CalculateMeshOutlines(int verticesCount)
//    {

//        for (int vertexIndex = 0; vertexIndex < verticesCount; vertexIndex++)
//        {
//            if (!checkedVertices.Contains(vertexIndex))
//            {
//                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
//                if (newOutlineVertex != -1)
//                {
//                    checkedVertices.Add(vertexIndex);

//                    List<int> newOutline = new List<int>();
//                    newOutline.Add(vertexIndex);
//                    outlines.Add(newOutline);
//                    FollowOutline(newOutlineVertex, outlines.Count - 1);
//                    outlines[outlines.Count - 1].Add(vertexIndex);
//                }
//            }
//        }
//    }

//    int GetConnectedOutlineVertex(int vertexIndex)
//    {
//        List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

//        for (int i = 0; i < trianglesContainingVertex.Count; i++)
//        {
//            Triangle triangle = trianglesContainingVertex[i];

//            for (int j = 0; j < 3; j++)
//            {
//                int vertexB = triangle[j];
//                if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB))
//                {
//                    if (IsOutlineEdge(vertexIndex, vertexB))
//                    {
//                        return vertexB;
//                    }
//                }
//            }
//        }

//        return -1;
//    }

//    void FollowOutline(int vertexIndex, int outlineIndex)
//    {
//        outlines[outlineIndex].Add(vertexIndex);
//        checkedVertices.Add(vertexIndex);
//        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

//        if (nextVertexIndex != -1)
//        {
//            FollowOutline(nextVertexIndex, outlineIndex);
//        }
//    }

//    bool IsOutlineEdge(int vertexA, int vertexB)
//    {
//        List<Triangle> trianglesContainingVertexA = triangleDictionary[vertexA];
//        int sharedTriangleCount = 0;

//        for (int i = 0; i < trianglesContainingVertexA.Count; i++)
//        {
//            if (trianglesContainingVertexA[i].Contains(vertexB))
//            {
//                sharedTriangleCount++;
//                if (sharedTriangleCount > 1)
//                {
//                    break;
//                }
//            }
//        }
//        return sharedTriangleCount == 1;
//    }

//    void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
//    {
//        if (triangleDictionary.ContainsKey(vertexIndexKey))
//        {
//            triangleDictionary[vertexIndexKey].Add(triangle);
//        }
//        else
//        {
//            List<Triangle> triangleList = new List<Triangle>();
//            triangleList.Add(triangle);
//            triangleDictionary.Add(vertexIndexKey, triangleList);
//        }
//    }
//#endregion
//#region struct&class
//    public class SquareGrid
//    {
//        public Square[,] wallSquares;
//        public Square[,] floorSquares;

//        public SquareGrid(Node[,] map, float squareSize)
//        {
//            int nodeCountX = map.GetLength(0);
//            int nodeCountY = map.GetLength(1);

//            float mapWidth = 0;
//            float mapHeight = 0;
//            ControlNode[,] wallControlNodes = new ControlNode[nodeCountX, nodeCountY];
//            ControlNode[,] floorControlNodes = new ControlNode[nodeCountX, nodeCountY];
//            wallSquares = new Square[nodeCountX - 1, nodeCountY - 1];
//            floorSquares = new Square[nodeCountX - 1, nodeCountY - 1];

//            for (int x = 0; x < nodeCountX; x++)
//            {
//                for (int y = 0; y < nodeCountY; y++)
//                {
//                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, -mapHeight / 2 + y * squareSize + squareSize / 2, 0);
//                    wallControlNodes[x, y] = new ControlNode(pos, map[x, y].tileType == TileType.EDGE, squareSize);
//                }
//            }

//            for (int x = 0; x < nodeCountX; x++)
//            {
//                for (int y = 0; y < nodeCountY; y++)
//                {
//                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, -mapHeight / 2 + y * squareSize + squareSize / 2, 0);
//                    floorControlNodes[x, y] = new ControlNode(pos, map[x, y].tileType == TileType.FLOOR, squareSize);
//                }
//            }

//            for (int x = 0; x < nodeCountX - 1; x++)
//            {
//                for (int y = 0; y < nodeCountY - 1; y++)
//                {
//                    wallSquares[x, y] = new Square(wallControlNodes[x, y + 1], wallControlNodes[x + 1, y + 1], wallControlNodes[x + 1, y], wallControlNodes[x, y]);
//                    floorSquares[x, y] = new Square(floorControlNodes[x, y + 1], floorControlNodes[x + 1, y + 1], floorControlNodes[x + 1, y], floorControlNodes[x, y]);
//                }
//            }

//        }
//    }

//    struct Triangle
//    {
//        public int vertexIndexA;
//        public int vertexIndexB;
//        public int vertexIndexC;
//        int[] vertices;

//        public Triangle(int a, int b, int c)
//        {
//            vertexIndexA = a;
//            vertexIndexB = b;
//            vertexIndexC = c;

//            vertices = new int[3];
//            vertices[0] = a;
//            vertices[1] = b;
//            vertices[2] = c;
//        }

//        public int this[int i]
//        {
//            get
//            {
//                return vertices[i];
//            }
//        }


//        public bool Contains(int vertexIndex)
//        {
//            return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
//        }
//    }

//    public class Square
//    {

//        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
//        public MeshNode centreTop, centreRight, centreBottom, centreLeft;
//        public int configuration;

//        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft)
//        {
//            topLeft = _topLeft;
//            topRight = _topRight;
//            bottomRight = _bottomRight;
//            bottomLeft = _bottomLeft;

//            centreTop = topLeft.right;
//            centreRight = bottomRight.above;
//            centreBottom = bottomLeft.right;
//            centreLeft = bottomLeft.above;

//            if (topLeft.active)
//                configuration += 8;
//            if (topRight.active)
//                configuration += 4;
//            if (bottomRight.active)
//                configuration += 2;
//            if (bottomLeft.active)
//                configuration += 1;
//        }
//    }

//    public class MeshNode
//    {
//        public Vector3 position;
//        public int vertexIndex = -1;

//        public MeshNode(Vector3 _pos)
//        {
//            position = _pos;
//        }
//    }

//    public class ControlNode : MeshNode
//    {

//        public bool active;
//        public MeshNode above, right;

//        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos)
//        {
//            active = _active;
//            above = new MeshNode(position + Vector3.up * squareSize / 2f);
//            right = new MeshNode(position + Vector3.right * squareSize / 2f);
//        }
//    }
//#endregion
//}