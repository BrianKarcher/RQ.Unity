using ClipperLib;
using RQ;
using RQ2.Controller.tk2d_Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;


namespace RQ2.UI
{
    [AddComponentMenu("RQ/UI/Tilemap Setup")]
    public class TilemapSetup : MonoBehaviour
    {
        public tk2dTileMap TileMap;
        public tk2dTileMap TileMapTest;
        public tk2dSpriteCollectionData SpriteCollectionData;
        [SerializeField]
        private string _tileMapVersion;
        [SerializeField]
        private string _tilemapPath;
        [SerializeField]
        private string _tilemapFileName;
        [SerializeField]
        private TextAsset _tileMapTmx;
        [SerializeField]
        private CameraClass _camera;


        public void Awake()
        {
            if (TileMap != null)
            {
                //TileMap.Editor__SpriteCollection = SpriteCollectionData;
                //TileMap.SpriteCollectionInst = SpriteCollection;
                tk2dUtilities.TileMapDeleteAllLayers(TileMap);
                TileMap.data.tilePrefabs = new GameObject[0];
                TileMap.ForceBuild();
                //tk2dUtilities.TileMapDeleteAllLayers(TileMap);
                tk2dUtilities.TilemapLoad(TileMap, _tileMapTmx.text);
                tk2dUtilities.DeleteUnusedLayers(TileMap);
                TileMap.ForceBuild();
                var renderData = TileMap.renderData;
                SetTagRecursive(renderData, "Tilemap");
                ColliderConverter(TileMap);
            }
        }

        public void Start()
        {
            SetupCamera();
        }

        public void OnDestroy()
        {
            if (TileMap == null)
                return;
            tk2dUtilities.TileMapDeleteAllLayers(TileMap);
        }

        /// <summary>
        /// TODO Make this a GameObject extension method
        /// </summary>
        /// <param name="go"></param>
        /// <param name="tag"></param>
        private static void SetTagRecursive(GameObject go, string tag)
        {
            go.tag = tag;
            foreach (Transform child in go.transform)
            {
                SetTagRecursive(child.gameObject, tag);
            }
        }

        public void SetupCamera()
        {
            //CameraClass cameraClass = GameController.Instance.GetCamera();
            //var cameraClass = GameController.Instance.GetSceneSetup().camera
            if (_camera == null)
            {
                Debug.LogError("(TilemapSetup) No camera set, cannot call SetMaxBounds.");
            }
            if (_camera != null && TileMap != null)
            {
                //cameraClass.Init();
                _camera.SetMaxBounds(TileMap.width, TileMap.height);
            }


            //if (cameraClass.CameraMode == RQ.CameraMode.FollowTarget)
            //{
            //    if (EntityContainer._instance.GetMainCharacter() == null)
            //        throw new Exception("Main Character is null in BeginSceneState - Setup Camera");
            //    cameraClass.Target = EntityContainer._instance.GetMainCharacter().transform;
            //}
        }

        public string GetTileMapTmxData()
        {
            return _tileMapTmx.text;
        }

        public void ColliderConverter(tk2dTileMap _tileMap)
        {
            var meshCollider = _tileMap.renderData.GetComponentsInChildren<MeshCollider>();
            var polygonColliders = _tileMap.renderData.GetComponentsInChildren<PolygonCollider2D>();

            // Only destroy the polygon colliders if there are edge colliders to convert
            // This prevents the user from accidently removing all tile colliders from the map
            // ie, first run turns all edge colliders into polygon colliders.  Second run deletes
            // all polygon colliders.
            if (meshCollider.Length != 0)
            {
                for (int i = polygonColliders.Length - 1; i > 0; i--)
                {
                    GameObject.DestroyImmediate(polygonColliders[i]);
                }
            }

            int tileMapWidth = _tileMap.width;
            int tileMapHeight = _tileMap.height;


            for (int layerIndex = 0; layerIndex < _tileMap.data.Layers.Length; layerIndex++)
            {
                var layer = _tileMap.data.Layers[layerIndex];
                if (!layer.name.Contains("Water"))
                    continue;
                if (!layer.name.Contains("Collider"))
                continue;

                GameObject go = new GameObject("poly collider");
                go.layer = LayerMask.NameToLayer("Environment");
                //if (layer.name.Contains("Level 1"))
                //    go.layer = LayerMask.NameToLayer("Level 1 TC");
                //if (layer.name.Contains("Level 2"))
                //    go.layer = LayerMask.NameToLayer("Level 2 TC");
                //if (layer.name.Contains("Shared"))
                //    go.layer = LayerMask.NameToLayer("Shared TC");
                if (layer.name.Contains("Water"))
                    go.layer = LayerMask.NameToLayer("Water");

                foreach (Transform collidergo in _tileMap.renderData.transform)
                {
                    if (collidergo.name == layer.name)
                    {
                        go.transform.position = Vector3.zero;
                        go.transform.SetParent(collidergo);
                        break;
                    }
                }

                List<List<Vector2>> polygons = new List<List<Vector2>>();
                for (int y = 0; y < tileMapHeight; y++)
                {
                    for (int x = 0; x < tileMapWidth; x++)
                    {
                        var tileId = _tileMap.GetTile(x, y, layerIndex);
                        if (tileId == -1)
                            continue;
                        var tilePos = _tileMap.GetTilePosition(x, y);
                        // Create points representing a square polygon at this spot.
                        List<Vector2> polygon = new List<Vector2>()
                        {
                                new Vector2(tilePos.x, tilePos.y),
                                new Vector2(tilePos.x + 0.16f, tilePos.y),
                                new Vector2(tilePos.x + 0.16f, tilePos.y + 0.16f),
                                new Vector2(tilePos.x, tilePos.y + 0.16f)
                        };
                        polygons.Add(polygon);

                        // There is a collider here, create the PolygonCollider!
                        //PolygonCollider2D polygonCollider = go.AddComponent<PolygonCollider2D>();
                        //Vector2[] myPoints = new Vector2[]
                        //{
                        //    new Vector2(tilePos.x, tilePos.y),
                        //    new Vector2(tilePos.x + 0.16f, tilePos.y),
                        //    new Vector2(tilePos.x + 0.16f, tilePos.y + 0.16f),
                        //    new Vector2(tilePos.x, tilePos.y + 0.16f)
                        //};
                        //polygonCollider.points = myPoints;
                        //polygonCollider.pathCount = 1;
                        //polygonCollider.SetPath(0, myPoints);
                        //go.transform.SetParent(coll.gameObject.transform);
                    }
                }

                //polygons = UnitePolygons(polygons);
                //Create2DPolygonCollider(polygons, go);
                CreateMeshCollider(polygons, go);
                //sdf
                // Only do first layer for now
                //break;
            }
        }

        public void CreateGroundCollider(tk2dTileMap _tileMap)
        {
            var meshCollider = _tileMap.renderData.GetComponentsInChildren<MeshCollider>();
            var polygonColliders = _tileMap.renderData.GetComponentsInChildren<PolygonCollider2D>();

            // Only destroy the polygon colliders if there are edge colliders to convert
            // This prevents the user from accidently removing all tile colliders from the map
            // ie, first run turns all edge colliders into polygon colliders.  Second run deletes
            // all polygon colliders.
            if (meshCollider.Length != 0)
            {
                for (int i = polygonColliders.Length - 1; i > 0; i--)
                {
                    GameObject.DestroyImmediate(polygonColliders[i]);
                }
            }

            int tileMapWidth = _tileMap.width;
            int tileMapHeight = _tileMap.height;


            for (int layerIndex = 0; layerIndex < _tileMap.data.Layers.Length; layerIndex++)
            {
                var layer = _tileMap.data.Layers[layerIndex];
                if (!layer.name.Contains("Collider"))
                    continue;
                if (layer.name.Contains("Shared"))
                    continue;

                GameObject go = new GameObject("mesh collider");
                go.layer = LayerMask.NameToLayer("Environment");
                //if (layer.name.Contains("Level 1"))
                //    go.layer = LayerMask.NameToLayer("Level 1 TC");
                //if (layer.name.Contains("Level 2"))
                //    go.layer = LayerMask.NameToLayer("Level 2 TC");
                //if (layer.name.Contains("Shared"))
                //    go.layer = LayerMask.NameToLayer("Shared TC");
                if (layer.name.Contains("Water"))
                    go.layer = LayerMask.NameToLayer("Water");

                foreach (Transform collidergo in _tileMap.renderData.transform)
                {
                    if (collidergo.name == layer.name)
                    {
                        go.transform.SetParent(collidergo);
                        go.transform.localPosition = Vector3.zero;
                        //go.transform.position = collidergo.transform.position;
                        break;
                    }
                }

                List<List<Vector2>> polygons = new List<List<Vector2>>();
                for (int y = 0; y < tileMapHeight; y++)
                {
                    for (int x = 0; x < tileMapWidth; x++)
                    {
                        var tileId = _tileMap.GetTile(x, y, layerIndex);
                        // Colliders are not walkable, so skip those
                        if (tileId != -1)
                            continue;
                        var tilePos = _tileMap.GetTilePosition(x, y);
                        // Create points representing a square polygon at this spot.
                        //List<Vector2> polygon = new List<Vector2>()
                        //{
                        //        new Vector2(tilePos.x, tilePos.y),
                        //        new Vector2(tilePos.x, tilePos.y + 0.16f),
                        //        new Vector2(tilePos.x + 0.16f, tilePos.y),
                        //        new Vector2(tilePos.x + 0.16f, tilePos.y + 0.16f)                                
                        //};
                        // Square
                        List<Vector2> polygon = new List<Vector2>()
                        {
                                new Vector2(tilePos.x, tilePos.y),
                                new Vector2(tilePos.x, tilePos.y + 0.16f),
                                new Vector2(tilePos.x + 0.16f, tilePos.y + 0.16f),
                                new Vector2(tilePos.x + 0.16f, tilePos.y)                                
                        };
                        polygons.Add(polygon);

                        // There is a collider here, create the PolygonCollider!
                        //PolygonCollider2D polygonCollider = go.AddComponent<PolygonCollider2D>();
                        //Vector2[] myPoints = new Vector2[]
                        //{
                        //    new Vector2(tilePos.x, tilePos.y),
                        //    new Vector2(tilePos.x + 0.16f, tilePos.y),
                        //    new Vector2(tilePos.x + 0.16f, tilePos.y + 0.16f),
                        //    new Vector2(tilePos.x, tilePos.y + 0.16f)
                        //};
                        //polygonCollider.points = myPoints;
                        //polygonCollider.pathCount = 1;
                        //polygonCollider.SetPath(0, myPoints);
                        //go.transform.SetParent(coll.gameObject.transform);
                    }
                }

                //polygons = UnitePolygons(polygons);
                //CreateMeshCollider(polygons, go);
                CreateBoxCollider(polygons, go);
                //sdf
                // Only do first layer for now
                //break;
            }
        }

        //public void ColliderConverter(tk2dTileMap _tileMap)
        //{
        //    var meshCollider = _tileMap.renderData.GetComponentsInChildren<MeshCollider>();
        //    var polygonColliders = _tileMap.renderData.GetComponentsInChildren<PolygonCollider2D>();

        //    // Only destroy the polygon colliders if there are edge colliders to convert
        //    // This prevents the user from accidently removing all tile colliders from the map
        //    // ie, first run turns all edge colliders into polygon colliders.  Second run deletes
        //    // all polygon colliders.
        //    if (meshCollider.Any())
        //    {
        //        for (int i = polygonColliders.Count() - 1; i > 0; i--)
        //        {
        //            GameObject.DestroyImmediate(polygonColliders[i]);
        //        }
        //    }

        //    int tileMapWidth = _tileMap.width;
        //    int tileMapHeight = _tileMap.height;


        //    for (int layerIndex = 0; layerIndex < _tileMap.data.Layers.Count(); layerIndex++)
        //    {
        //        var layer = _tileMap.data.Layers[layerIndex];
        //        if (!layer.name.Contains("Collider"))
        //            continue;

        //        GameObject go = new GameObject("poly collider");
        //        if (layer.name.Contains("Level 1"))
        //            go.layer = LayerMask.NameToLayer("Level 1 TC");
        //        if (layer.name.Contains("Level 2"))
        //            go.layer = LayerMask.NameToLayer("Level 2 TC");
        //        if (layer.name.Contains("Shared"))
        //            go.layer = LayerMask.NameToLayer("Shared TC");
        //        if (layer.name.Contains("Water"))
        //            go.layer = LayerMask.NameToLayer("Water");

        //        foreach (Transform collidergo in _tileMap.renderData.transform)
        //        {
        //            if (collidergo.name == layer.name)
        //            {
        //                go.transform.SetParent(collidergo);
        //                break;
        //            }
        //        }

        //        for (int y = 0; y < tileMapHeight; y++)
        //        {
        //            for (int x = 0; x < tileMapWidth; x++)
        //            {
        //                var tileId = _tileMap.GetTile(x, y, layerIndex);
        //                if (tileId == -1)
        //                    continue;
        //                var tilePos = _tileMap.GetTilePosition(x, y);
        //                // There is a collider here, create the PolygonCollider!
        //                PolygonCollider2D polygonCollider = go.AddComponent<PolygonCollider2D>();
        //                Vector2[] myPoints = new Vector2[]
        //                {
        //                    new Vector2(tilePos.x, tilePos.y),
        //                    new Vector2(tilePos.x + 0.16f, tilePos.y),
        //                    new Vector2(tilePos.x + 0.16f, tilePos.y + 0.16f),
        //                    new Vector2(tilePos.x, tilePos.y + 0.16f)
        //                };
        //                polygonCollider.points = myPoints;
        //                polygonCollider.pathCount = 1;
        //                polygonCollider.SetPath(0, myPoints);
        //                //go.transform.SetParent(coll.gameObject.transform);
        //            }
        //        }
        //        // Only do first layer for now
        //        //break;
        //    }
        //}

        //var colliderLayers = _tileMap.data.Layers.Where(i => i.name.Contains("Collider"));
        //colliderLayers.First().

        //foreach (var coll in meshCollider)
        //{
        //    // Get triangles and vertices from mesh
        //    var triangles = coll.sharedMesh.triangles;
        //    var vertices = coll.sharedMesh.vertices;
        //    //PolygonCollider2D polygonCollider = _tileMap.renderData.gameObject.AddComponent<PolygonCollider2D>();
        //    var go = new GameObject("poly collider");
        //    go.transform.SetParent(coll.gameObject.transform);

        //    PolygonCollider2D polygonCollider = go.AddComponent<PolygonCollider2D>();
        //    List<Vector2> myPoints = new List<Vector2>();
        //    int pathCount = 0;
        //    for (int i = 0; i < triangles.Count(); i++)
        //    {
        //        myPoints.Add(vertices[triangles[i]]);
        //        if (i % 3 == 0)
        //        {
        //            polygonCollider.pathCount++;
        //            polygonCollider.SetPath(polygonCollider.pathCount - 1, myPoints.ToArray());
        //            myPoints.Clear();
        //        }
        //    }

        //    //polygonCollider.points = myPoints.ToArray();
        //    //polygonCollider.pathCount = 1;
        //    //polygonCollider.SetPath(0, myPoints.ToArray());


        //    // Get just the outer edges from the mesh's triangles (ignore or remove any shared edges)
        //    //var edges = new Dictionary<string, KeyValuePair<int, int>>();
        //    //for (int i = 0; i < triangles.Length; i += 3)
        //    //{
        //    //    for (int e = 0; e < 3; e++)
        //    //    {
        //    //        int vert1 = triangles[i + e];
        //    //        int vert2 = triangles[i + e + 1 > i + 2 ? i : i + e + 1];
        //    //        string edge = Mathf.Min(vert1, vert2) + ":" + Mathf.Max(vert1, vert2);
        //    //        if (edges.ContainsKey(edge))
        //    //        {
        //    //            edges.Remove(edge);
        //    //        }
        //    //        else
        //    //        {
        //    //            edges.Add(edge, new KeyValuePair<int, int>(vert1, vert2));
        //    //        }
        //    //    }
        //    //}

        //    //// Create edge lookup (Key is first vertex, Value is second vertex, of each edge)
        //    //var lookup = new Dictionary<int, int>();
        //    //foreach (KeyValuePair<int, int> edge in edges.Values)
        //    //{
        //    //    if (lookup.ContainsKey(edge.Key) == false)
        //    //        lookup.Add(edge.Key, edge.Value);
        //    //}
        //    //// Create empty polygon collider
        //    ////GameObject.Add
        //    //PolygonCollider2D polygonCollider = _tileMap.renderData.gameObject.AddComponent<PolygonCollider2D>();
        //    //polygonCollider.pathCount = 0;
        //    //// Loop through edge vertices in order
        //    //int startVert = 0;
        //    //int nextVert = startVert;
        //    //int highestVert = startVert;
        //    //var colliderPath = new List<Vector2>();
        //    //while (true)
        //    //{
        //    //    // Add vertex to collider path
        //    //    colliderPath.Add(vertices[nextVert]);

        //    //    // Get next vertex
        //    //    nextVert = lookup[nextVert];

        //    //    // Store highest vertex (to know what shape to move to next)
        //    //    if (nextVert > highestVert)
        //    //    {
        //    //        highestVert = nextVert;
        //    //    }

        //    //    // Shape complete
        //    //    if (nextVert == startVert)
        //    //    {
        //    //        // Add path to polygon collider
        //    //        polygonCollider.pathCount++;
        //    //        polygonCollider.SetPath(polygonCollider.pathCount - 1, colliderPath.ToArray());
        //    //        colliderPath.Clear();

        //    //        // Go to next shape if one exists
        //    //        if (lookup.ContainsKey(highestVert + 1))
        //    //        {
        //    //            // Set starting and next vertices
        //    //            startVert = highestVert + 1;
        //    //            nextVert = startVert;

        //    //            // Continue to next loop
        //    //            continue;
        //    //        }

        //    //        // No more verts
        //    //        break;
        //    //    }
        //    //}

        //    // myPoints.Clear();
        //    //Vector2[] myPoints = coll.points;
        //    //int myEdges = coll.edgeCount;

        //    //myPoly.points = myPoints;
        //    //myPoly.pathCount = 1;
        //    //myPoly.SetPath(0, myPoints);
        //    ////if (convertToTriggers) myPoly.isTrigger = true;
        //    //GameObject.DestroyImmediate(coll);
        //}

        //this function takes a list of polygons as a parameter, this list of polygons represent all the polygons that constitute collision in your level.
        public List<List<Vector2>> UnitePolygons(List<List<Vector2>> polygons)
        {
            //this is going to be the result of the method
            List<List<Vector2>> unitedPolygons = new List<List<Vector2>>();
            Clipper clipper = new Clipper();

            //clipper only works with ints, so if we're working with floats, we need to multiply all our floats by
            //a scaling factor, and when we're done, divide by the same scaling factor again
            int scalingFactor = 10000;

            //this loop will convert our List<List<Vector2>> to what Clipper works with, which is "Path" and "IntPoint"
            //and then add all the Paths to the clipper object so we can process them
            for (int i = 0; i < polygons.Count; i++)
            {
                Path allPolygonsPath = new Path(polygons[i].Count);

                for (int j = 0; j < polygons[i].Count; j++)
                {
                    allPolygonsPath.Add(new IntPoint(Mathf.Floor(polygons[i][j].x * scalingFactor), Mathf.Floor(polygons[i][j].y * scalingFactor)));
                }
                clipper.AddPath(allPolygonsPath, PolyType.ptSubject, true);

            }

            //this will be the result
            Paths solution = new Paths();

            //having added all the Paths added to the clipper object, we tell clipper to execute an union
            clipper.Execute(ClipType.ctUnion, solution);

            //the union may not end perfectly, so we're gonna do an offset in our polygons, that is, expand them outside a little bit
            ClipperOffset offset = new ClipperOffset();
            offset.AddPaths(solution, JoinType.jtMiter, EndType.etClosedPolygon);
            //5 is the ammount of offset
            offset.Execute(ref solution, 5f);

            //now we just need to conver it into a List<List<Vector2>> while removing the scaling
            foreach (Path path in solution)
            {
                List<Vector2> unitedPolygon = new List<Vector2>();
                foreach (IntPoint point in path)
                {
                    unitedPolygon.Add(new Vector2(point.X / (float)scalingFactor, point.Y / (float)scalingFactor));
                }
                unitedPolygons.Add(unitedPolygon);
            }

            //this removes some redundant vertices in the polygons when they are too close from each other
            //may be useful to clean things up a little if your initial collisions don't match perfectly from tile to tile
            unitedPolygons = RemoveClosePointsInPolygons(unitedPolygons);

            //everything done
            return unitedPolygons;
        }

        //create the collider in unity from the list of polygons
        public void Create2DPolygonCollider(List<List<Vector2>> polygons, GameObject colliderObj)
        {
            //GameObject colliderObj = new GameObject("LevelCollision");
            //colliderObj.layer = GR.inst.GetLayerID(Layer.PLATFORM);
            //colliderObj.transform.SetParent(level.levelObj.transform);

            PolygonCollider2D collider = colliderObj.AddComponent<PolygonCollider2D>();

            collider.pathCount = polygons.Count;

            for (int i = 0; i < polygons.Count; i++)
            {
                Vector2[] points = polygons[i].ToArray();

                collider.SetPath(i, points);
            }
        }

        //create the collider in unity from the list of polygons
        public void CreateMeshCollider(List<List<Vector2>> polygons, GameObject colliderObj)
        {
            //GameObject colliderObj = new GameObject("LevelCollision");
            //colliderObj.layer = GR.inst.GetLayerID(Layer.PLATFORM);
            //colliderObj.transform.SetParent(level.levelObj.transform);

            //PolygonCollider2D collider = colliderObj.AddComponent<PolygonCollider2D>();

            //collider.pathCount = polygons.Count;

            //for (int i = 0; i < polygons.Count; i++)
            //{
            //    Vector2[] points = polygons[i].ToArray();

            //    collider.SetPath(i, points);
            //}

            //var vertices2D = polygons.SelectMany(i => i).ToArray();

            // Use the triangulator to get indices for creating triangles
            //Triangulator tr = new Triangulator(vertices2D);
            //int[] indices = tr.Triangulate();

            List<int> indices = new List<int>();
            List<Vector3> vertices = new List<Vector3>();
            int vertexCount = 0;
            foreach (var polygon in polygons)
            {
                //int vertexInPolygonCount = 0;
                for (int i = 0; i < polygon.Count; i++)
                {
                    vertices.Add(polygon[i]);
                    if (i >= 2)
                    {
                        // Create triangle
                        indices.Add(vertexCount - 2);
                        indices.Add(vertexCount - 1);
                        indices.Add(vertexCount);
                    }
                    vertexCount++;
                }
            }

            // Create the Vector3 vertices
            //Vector3[] vertices = new Vector3[vertices2D.Length];
            //for (int i = 0; i < vertices.Length; i++)
            //{
            //    vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
            //}

            // Create the mesh
            Mesh msh = new Mesh();
            msh.vertices = vertices.ToArray();
            msh.triangles = indices.ToArray();
            //msh.vertices = new Vector3[] { new Vector3(0, 0), new Vector3(0, 5), new Vector3(5, 0), new Vector3(5, 5) };
            //msh.triangles = new int[] { 0, 1, 2, 1, 3, 2 };
            msh.RecalculateNormals();
            msh.RecalculateBounds();

            // Set up game object with mesh;
            //gameObject.colliderObj(typeof(MeshRenderer));
            MeshCollider filter = colliderObj.AddComponent(typeof(MeshCollider)) as MeshCollider;
            filter.sharedMesh = msh;
        }

        //create the collider in unity from the list of polygons
        public void CreateBoxCollider(List<List<Vector2>> polygons, GameObject colliderObj)
        {
            //GameObject colliderObj = new GameObject("LevelCollision");
            //colliderObj.layer = GR.inst.GetLayerID(Layer.PLATFORM);
            //colliderObj.transform.SetParent(level.levelObj.transform);

            //PolygonCollider2D collider = colliderObj.AddComponent<PolygonCollider2D>();

            //collider.pathCount = polygons.Count;

            //for (int i = 0; i < polygons.Count; i++)
            //{
            //    Vector2[] points = polygons[i].ToArray();

            //    collider.SetPath(i, points);
            //}

            //var vertices2D = polygons.SelectMany(i => i).ToArray();

            // Use the triangulator to get indices for creating triangles
            //Triangulator tr = new Triangulator(vertices2D);
            //int[] indices = tr.Triangulate();

            //List<int> indices = new List<int>();
            //List<Vector3> vertices = new List<Vector3>();
            //int vertexCount = 0;
            foreach (var polygon in polygons)
            {
                BoxCollider boxCollider = colliderObj.AddComponent(typeof(BoxCollider)) as BoxCollider;
                var bottomLeft = polygon[0];
                var topLeft = polygon[1];
                var topRight = polygon[2];
                var bottomRight = polygon[3];
                boxCollider.center = new Vector3((bottomLeft.x + bottomRight.x) / 2f, (topLeft.y + bottomLeft.y) / 2f);
                boxCollider.size = new Vector3((bottomRight.x - bottomLeft.x) / 2f, (bottomLeft.y - topLeft.y) / 2f, .1f);
                //int vertexInPolygonCount = 0;
                //for (int i = 0; i < polygon.Count; i++)
                //{
                //    vertices.Add(polygon[i]);
                //    if (i >= 2)
                //    {
                //        // Create triangle
                //        indices.Add(vertexCount - 2);
                //        indices.Add(vertexCount - 1);
                //        indices.Add(vertexCount);
                //    }
                //    vertexCount++;
                //}
            }

            // Create the Vector3 vertices
            //Vector3[] vertices = new Vector3[vertices2D.Length];
            //for (int i = 0; i < vertices.Length; i++)
            //{
            //    vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
            //}

            // Create the mesh
            //Mesh msh = new Mesh();
            //msh.vertices = vertices.ToArray();
            //msh.triangles = indices.ToArray();
            ////msh.vertices = new Vector3[] { new Vector3(0, 0), new Vector3(0, 5), new Vector3(5, 0), new Vector3(5, 5) };
            ////msh.triangles = new int[] { 0, 1, 2, 1, 3, 2 };
            //msh.RecalculateNormals();
            //msh.RecalculateBounds();

            //// Set up game object with mesh;
            ////gameObject.colliderObj(typeof(MeshRenderer));
            //MeshCollider filter = colliderObj.AddComponent(typeof(MeshCollider)) as MeshCollider;
            //filter.sharedMesh = msh;
        }

        public List<List<Vector2>> RemoveClosePointsInPolygons(List<List<Vector2>> polygons)
        {
            float proximityLimit = 0.1f;

            List<List<Vector2>> resultPolygons = new List<List<Vector2>>();

            foreach (List<Vector2> polygon in polygons)
            {
                List<Vector2> pointsToTest = polygon;
                List<Vector2> pointsToRemove = new List<Vector2>();

                foreach (Vector2 pointToTest in pointsToTest)
                {
                    foreach (Vector2 point in polygon)
                    {
                        if (point == pointToTest || pointsToRemove.Contains(point)) continue;

                        bool closeInX = Math.Abs(point.x - pointToTest.x) < proximityLimit;
                        bool closeInY = Math.Abs(point.y - pointToTest.y) < proximityLimit;

                        if (closeInX && closeInY)
                        {
                            pointsToRemove.Add(pointToTest);
                            break;
                        }
                    }
                }
                polygon.RemoveAll(x => pointsToRemove.Contains(x));

                if (polygon.Count > 0)
                {
                    resultPolygons.Add(polygon);
                }
            }

            return resultPolygons;
        }
    }
}
