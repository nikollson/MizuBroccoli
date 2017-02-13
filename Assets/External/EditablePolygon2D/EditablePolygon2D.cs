using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Library.EditablePolygon2D
{

    [ExecuteInEditMode()]
    public class EditablePolygon2D : MonoBehaviour
    {
        MeshFilter meshFilter;
        PolygonCollider2D polygonCollider2D;
        BoxCollider2D boxCollider2D;
        UpdateDetecter updateDetecter;
        MeshRenderer meshRenderer;

        void Start()
        {
            meshFilter = this.GetComponent<MeshFilter>();
            polygonCollider2D = this.GetComponent<PolygonCollider2D>();
            boxCollider2D = this.GetComponent<BoxCollider2D>();
            meshRenderer = this.GetComponent<MeshRenderer>();
            updateDetecter = new UpdateDetecter(this.gameObject);
            updateDetecter.Reset();
            meshFilter.mesh = null;
            MakePolygonMesh();
        }

        void Update()
        {
            MakePolygonMesh();
        }

        void OnDisable()
        {
            ClearMeshFilter();
        }

        
        void MakePolygonMesh()
        {
            if (updateDetecter == null) return;
            if (!updateDetecter.IsUpdated()) return;

            updateDetecter.Update();

            var points = updateDetecter.GetPointsFromGameCollider();
            var meshData = GetMeshData(points, 0, 0.0f, 1, 1);
            ClearMeshFilter();
            meshFilter.sharedMesh = meshData.GetMesh(meshRenderer.sharedMaterials.Length);
        }

        public static MeshData GetMeshData(Vector2[] points, int materialNum, float zValue, float width, float height)
        {
            MeshData ret = new MeshData();

            int N = points.Length;

            var vertices = new Vector3[N];
            var uv = new Vector2[N];
            for (int i = 0; i < points.Length; i++) vertices[i] = new Vector3(points[i].x, points[i].y, zValue);
            for (int i = 0; i < points.Length; i++) uv[i] = new Vector2(points[i].x / width, points[i].y / height);

            DelaunyTrianglation delaunyTrianglation = new DelaunyTrianglation();
            var triangles = delaunyTrianglation.TriangulatePolygon_Track(points);


            ret.SetVertices(new List<Vector3>(vertices));
            ret.SetUV(new List<Vector2>(uv));
            ret.SetTriangle(new List<int>(triangles), materialNum);

            return ret;
        }

        void ClearMeshFilter()
        {
            if (meshFilter.sharedMesh != null) DestroyImmediate(meshFilter.sharedMesh);
        }

        public class UpdateDetecter
        {
            bool atfirst = true;
            Vector2[] _mesh;
            public void Reset()
            {
                _mesh = null;
                atfirst = true;
            }

            Collider2D collider2D;
            public UpdateDetecter(GameObject gameObject)
            {
                collider2D = gameObject.GetComponent<Collider2D>();
            }

            public Vector2[] GetPointsFromGameCollider()
            {
                if (collider2D == null) return null;
                if (collider2D is PolygonCollider2D) return ((PolygonCollider2D)collider2D).points;
                if (collider2D is BoxCollider2D)
                {
                    var boxCollider2D = (BoxCollider2D)collider2D;
                    Vector2[] ret = new Vector2[4];
                    Vector2 leftBottom = boxCollider2D.offset - boxCollider2D.size / 2; ;
                    Vector2 rightTop = boxCollider2D.offset + boxCollider2D.size / 2;
                    ret[0] = leftBottom;
                    ret[1] = new Vector2(leftBottom.x, rightTop.y);
                    ret[2] = rightTop;
                    ret[3] = new Vector2(rightTop.x, leftBottom.y);
                    return ret;
                }
                Xorshift xorShift = new Xorshift(1234);
                if (collider2D is CircleCollider2D)
                {
                    var circleCollider2D = (CircleCollider2D)collider2D;
                    int split = 30;
                    Vector2[] ret = new Vector2[split];
                    for (int i = 0; i < split; i++)
                    {
                        float angle = - Mathf.PI * 2 * i / split;
                        float radius = circleCollider2D.radius;
                        
                        ret[i] = (new Vector2(Mathf.Cos(angle), Mathf.Sin(angle))) * (radius * (xorShift.Range(995,1005)/1000.0f)) + circleCollider2D.offset;
                    }
                    return ret;
                }
                return null;
            }


            public bool IsUpdated()
            {
                var mesh = GetPointsFromGameCollider();
                bool result = GetResult(mesh);
                if (atfirst) _mesh = mesh;
                atfirst = false;
                return result;
            }

            public void Update()
            {
                if (IsUpdated())
                {
                    var mesh = GetPointsFromGameCollider();
                    _mesh = mesh;
                }
            }

            bool GetResult(Vector2[] mesh)
            {
                if (atfirst) return false;
                if (mesh == null) return true;
                if (_mesh.Length != mesh.Length) return true;

                bool result = false;
                for (int i = 0; i < _mesh.Length; i++)
                {
                    if (_mesh[i] != mesh[i]) result = true;
                }
                return result;
            }
        }
    }
}