using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Library.EditablePolygon2D
{
    [ExecuteInEditMode]
    [System.Serializable]
    public class EditablePolygonMk2 : MonoBehaviour
    {
        public MeshDrawer[] meshDrawer;
        EditablePolygon2D.UpdateDetecter updateDetecter;
        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        UpdateChecker updateChekcer;

        EditablePolygonMk2 myScript;
        string myName;
        GameObject myObject;
        int myCode;
        [HideInInspector] public int myId = 0;
        Vector3 myPosition;

        void Start()
        {
            updateDetecter = new EditablePolygon2D.UpdateDetecter(this.gameObject);
            updateDetecter.Reset();
            meshFilter = this.GetComponent<MeshFilter>();
            meshRenderer = this.GetComponent<MeshRenderer>();

            updateChekcer = new UpdateChecker();

            if (!Application.isPlaying)
            {
                if (myId != 0 && myId != gameObject. GetInstanceID())
                {
                    meshFilter.sharedMesh = null;
                }
                myId = this.gameObject.GetInstanceID();
                InitMeshUpdate_Editor();

                //Debug.Log("start " + this.gameObject.name + " "+Time.frameCount);
            }
            else
            {
                if (meshFilter.sharedMesh == null)
                {
                    InitMeshUpdate_Editor();
                }
                this.enabled = false;
            }
        }


        void Update()
        {
            if (!Application.isPlaying)
            {
                UpdateMesh_Editor();
            }
        }

        void InitMeshUpdate_Editor()
        {
            if (meshFilter.sharedMesh == null)
            {
                UpdateMesh_Editor();
            }
            else
            {
                if (updateDetecter == null) updateDetecter = new EditablePolygon2D.UpdateDetecter(this.gameObject);
                if (updateChekcer == null) updateChekcer = new UpdateChecker();
                updateDetecter.Update();
                updateChekcer.Update(meshDrawer);
            }
        }

        void UpdateMesh_Editor()
        {
            bool updated = false;

            if (updateDetecter == null) updateDetecter = new EditablePolygon2D.UpdateDetecter(this.gameObject);
            if (updateChekcer == null) updateChekcer = new UpdateChecker();

            updated |= meshFilter.sharedMesh == null;
            updated |= updateDetecter.IsUpdated();
            updated |= updateChekcer.IsUpdated(meshDrawer);

            updateDetecter.Update();
            updateChekcer.Update(meshDrawer);

            if (updated) MakeMesh(updateDetecter.GetPointsFromGameCollider());
        }

        void MakeMesh(Vector2[] points)
        {
            MeshData meshData = new MeshData();
            foreach (var a in meshDrawer)
            {
                meshData = MeshData.Marge(meshData, a.GetMeshData(points));
            }

            /*
            if (meshFilter.sharedMesh != null) DestroyImmediate(meshFilter.sharedMesh);
            meshFilter.sharedMesh = meshData.GetMesh(meshRenderer.sharedMaterials.Length);
             * */

            if (meshFilter.sharedMesh == null)
            {
                Mesh mesh = new Mesh();
                //UnityEditor.AssetDatabase.CreateAsset(mesh, "Assets/Resources/EditablePolygon/" + Random.Range(0, 10000.0f).ToString("0000.0000000") + "Mesh.asset");
                meshFilter.sharedMesh = mesh;
            }
            meshData.SetMesh(meshFilter.sharedMesh, meshRenderer.sharedMaterials.Length);
        }

        /*
        void OnDisable()
        {
            if (meshFilter!=null &&  meshFilter.sharedMesh != null)
            {
                if (Application.isPlaying) Destroy(meshFilter.sharedMesh);
                else DestroyImmediate(meshFilter.sharedMesh);
            }
        }*/

        [System.Serializable]
        public struct MeshDrawer
        {
            public enum Mode { Polygon, Line, Around };

            public Mode mode;
            public PolygonGetter polygonGetter;
            public LineGetter lineGetter;
            public AroundGetter aroundGetter;

            public MeshData GetMeshData(Vector2[] points)
            {
                if (mode == Mode.Polygon) return polygonGetter.GetMeshData(points);
                if (mode == Mode.Line) return lineGetter.GetMeshData(points);
                if (mode == Mode.Around) return aroundGetter.GetMeshData(points);
                return new MeshData();
            }

            new public bool Equals(object obj)
            {
                if (!(obj is MeshDrawer)) return false;
                MeshDrawer data = (MeshDrawer)obj;
                if (mode != data.mode) return false;
                if (!polygonGetter.Equals(data.polygonGetter)) return false;
                if (!lineGetter.Equals(data.lineGetter)) return false;
                if (!aroundGetter.Equals(data.aroundGetter)) return false;
                return true;
            }
        }

        // data class

        [System.Serializable]
        public struct PolygonGetter
        {
            public int material;
            public float zValue;
            public Additional additional;
            public Make3DSetting make3DSetting;

            [System.Serializable]
            public struct Make3DSetting
            {
                public bool is3DMode;
                public float front;
                public float back;
                public bool ignoreFront;
                public bool ignoreBack;
            }


            [System.Serializable]
            public struct Additional
            {
                public bool isAdditional;
                public float width;
            }

            public MeshData GetMeshData(Vector2[] points)
            {
                float width = (additional.isAdditional ? additional.width : 1.0f);

                MeshData meshData = EditablePolygon2D.GetMeshData(points, material, zValue, width, width);
                if (make3DSetting.is3DMode)
                {
                    MeshData meshNext = new MeshData();

                    if (!make3DSetting.ignoreFront)
                    {
                        // front mesh
                        meshData.Move(new Vector3(0, 0, make3DSetting.front - zValue));
                        meshNext = MeshData.Marge(meshNext, meshData);
                    }



                    for (int i = 0; i < points.Length; i++)
                    {
                        /*
                        Vector3 u = meshData.vertices[i];
                        Vector3 v = meshData.vertices[(i + 1) % meshData.vertices.Count];
                        */
                        Vector3 u = points[i];
                        Vector3 v = points[(i + 1) % points.Length];
                        MeshData tmpMeshData = GetSidePolygon(u, v, width, make3DSetting.front, make3DSetting.back, material);
                        meshNext = MeshData.Marge(tmpMeshData, meshNext);
                    }

                    if (!make3DSetting.ignoreBack)
                    {
                        // back mesh
                        MeshData meshBack = MeshData.Marge(new MeshData(), meshData);
                        meshBack.Move(new Vector3(0, 0, make3DSetting.back - make3DSetting.front));
                        meshNext = MeshData.Marge(meshBack, meshNext);
                    }

                    meshData = meshNext;
                }
                return meshData;
            }

            public static MeshData GetSidePolygon(Vector3 u, Vector3 v, float width, float front, float back, int material)
            {
                u.z = front;
                v.z = front;
                Vector3 distUV = (v - u);
                float height = 1.0f;
                float shift = 0.0f;
                if (Mathf.Abs(distUV.x) > Mathf.Abs(distUV.y))
                {
                    height = width * (distUV.magnitude) / distUV.x;
                    shift = (float)(-(double)v.x / width) * Mathf.Sign(distUV.x);
                }
                else
                {
                    height = width * (distUV.magnitude) / distUV.y;
                    shift = v.y / width * Mathf.Sign(-distUV.y);
                }

                shift = shift - (int)(shift / width) * width;

                Vector2[] data = new Vector2[4];
                float xmin = back, xmax = front;
                float ymin = shift, ymax = distUV.magnitude + shift;

                data[0] = new Vector2(xmin, ymax);
                data[1] = new Vector2(xmax, ymax);
                data[2] = new Vector2(xmax, ymin);
                data[3] = new Vector2(xmin, ymin);
                /*
                data[0] = new Vector2(0, 1);
                data[1] = new Vector2(1, 1);
                data[2] = new Vector2(1, 0);
                data[3] = new Vector2(0, 0);
                */
                MeshData tmpMeshData = EditablePolygon2D.GetMeshData(data, material, 0, width, height);

                float rotX = Mathf.Atan2(distUV.y, distUV.x) / Mathf.PI * 180;
                tmpMeshData.Move(new Vector3(0, -shift - distUV.magnitude / 2, 0));
                tmpMeshData.Rotate(new Vector3(rotX, 0, 0));
                tmpMeshData.Rotate(new Vector3(90, 0, 90));
                tmpMeshData.Move((u + v) / 2 + new Vector3(0, 0, -front));
                return tmpMeshData;
            }

            public new bool Equals(object obj)
            {
                if (!(obj is PolygonGetter)) return false;
                var data = (PolygonGetter)obj;

                if (material != data.material) return false;
                if (zValue != data.zValue) return false;
                if (!additional.Equals(data.additional)) return false;
                if (!make3DSetting.Equals(data.make3DSetting)) return false;
                return true;
            }
        }

        [System.Serializable]
        public struct LineGetter
        {
            public int material;
            public float zValue;
            public float width;
            public float startAngle;
            public float endAngle;
            public float nankaAngle;

            public AdditionalSetting additional;
            public Make3DSetting make3D;

            new public bool Equals(object obj)
            {
                if (!(obj is LineGetter)) return false;
                var data = (LineGetter)obj;

                if (material != data.material) return false;
                if (zValue != data.zValue) return false;
                if (width != data.width) return false;
                if (startAngle != data.startAngle) return false;
                if (endAngle != data.endAngle) return false;
                if (nankaAngle != data.nankaAngle) return false;
                if (!additional.Equals(data.additional)) return false;
                if (!make3D.Equals(data.make3D)) return false;
                return true;
            }

            [System.Serializable]
            public struct Make3DSetting
            {
                public bool is3DMode;
                public int topMaterial;
                public float front;
                public float back;
                public float padding;

                new public bool Equals(object obj)
                {
                    if (!(obj is Make3DSetting)) return false;
                    var data = (Make3DSetting)obj;
                    if (is3DMode != data.is3DMode) return false;
                    if (topMaterial != data.topMaterial) return false;
                    if (front != data.front) return false;
                    if (back != data.back) return false;
                    if (padding != data.padding) return false;
                    return true;
                }
            }

            [System.Serializable]
            public struct AdditionalSetting
            {
                public bool isAdditionalMode;
                public int materialLeft;
                public int materialRight;

                new public bool Equals(object obj)
                {
                    if (!(obj is AdditionalSetting)) return false;
                    var data = (AdditionalSetting)obj;
                    if (isAdditionalMode != data.isAdditionalMode) return false;
                    if (materialLeft != data.materialLeft) return false;
                    if (materialRight != data.materialRight) return false;
                    return true;
                }
            }

            public MeshData GetMeshData(Vector2[] points)
            {
                int N = points.Length;
                if (N == 0) return null;
                float startRad = startAngle / 180 * Mathf.PI;
                float endRad = endAngle / 180 * Mathf.PI;

                int padding = 0;
                for (int i = 0; i < N; i++)
                {
                    Vector2 dist = points[(i + 1) % N] - points[i];
                    float angle = Mathf.Atan2(dist.y, dist.x);

                    if (!IsAngleIn(startRad, endRad, angle)) { padding = i; break; }
                }

                MeshData meshData = new MeshData();
                List<Vector2> line = new List<Vector2>();

                for (int i = 0; i < N; i++)
                {
                    Vector2 pointA = points[(i + padding) % N];
                    Vector2 pointB = points[(i + 1 + padding) % N];
                    Vector2 dist = pointB - pointA;
                    float angle = Mathf.Atan2(dist.y, dist.x);

                    bool doMake = false;

                    if (!IsAngleIn(startRad, endRad, angle)) doMake = true;
                    else
                    {
                        if (line.Count == 0) line.Add(pointA);
                        line.Add(pointB);
                    }
                    doMake |= (i == N - 1);


                    if (doMake && line.Count != 0)
                    {
                        Vector2[] arg = ConvArray<Vector2>(line);
                        line.Clear();

                        meshData = MeshData.Marge(meshData, GetLineMeshData(arg));
                    }
                }

                return meshData;
            }

            MeshData GetLineMeshData(Vector2[] line)
            {
                MeshData meshData = new MeshData();
                if (additional.isAdditionalMode)
                {
                    MeshData lineMesh = LineDrawer.GetMeshDataMk2(line, width, additional.materialLeft, material, additional.materialRight, zValue);
                    meshData = MeshData.Marge(meshData, lineMesh);
                }
                else
                {
                    MeshData lineMesh = LineDrawer.GetMeshData(line, width, material, zValue);
                    meshData = MeshData.Marge(meshData, lineMesh);
                }

                if (make3D.is3DMode)
                {
                    List<Vector2> linepadData = LineDrawer.GetMainLineData(line, Mathf.Max(0.001f, make3D.padding))[0];

                    for (int i = linepadData.Count - 1; i >= 0; i--) if (i >= 2 && i % 2 == 1) linepadData.RemoveAt(i);
                    for (int i = 0; i < linepadData.Count - 1; i++)
                    {
                        var topMesh = PolygonGetter.GetSidePolygon(linepadData[i], linepadData[i + 1], 1.0f, make3D.front, make3D.back, make3D.topMaterial);
                        meshData = MeshData.Marge(meshData, topMesh);
                    }
                }
                return meshData;
            }

            bool IsAngleIn(float start, float end, float angle)
            {
                while (end < start) end += Mathf.PI * 2;
                while (angle > end) angle -= Mathf.PI * 2;
                while (start > angle) angle += Mathf.PI * 2;

                return start <= angle && angle <= end;
            }

            T[] ConvArray<T>(List<T> data)
            {
                var ret = new T[data.Count];
                for (int i = 0; i < data.Count; i++) ret[i] = data[i];
                return ret;
            }
        }

        [System.Serializable]
        public struct AroundGetter
        {
            public int material;
            public float zValue;
            public float width;

            new public bool Equals(object obj)
            {
                if (!(obj is AroundGetter)) return false;
                var data = (AroundGetter)obj;
                if (material != data.material) return false;
                if (zValue != data.zValue) return false;
                if (width != data.width) return false;
                return true;
            }

            public MeshData GetMeshData(Vector2[] p)
            {
                Vector2[] nextPoints = new Vector2[p.Length + 2];

                int maxp = 0;
                for (int i = 0; i < p.Length - 1; i++)
                {
                    if ((p[maxp + 1] - p[maxp]).magnitude <= (p[i + 1] - p[i]).magnitude) maxp = i;
                }

                Vector2 st = (p[maxp + 1] + p[maxp]) / 2;
                nextPoints[0] = st;
                nextPoints[nextPoints.Length - 1] = st;

                if (p.Length == 0) return null;
                for (int i = 0; i < p.Length; i++)
                {
                    nextPoints[i + 1] = p[(i + maxp + 1) % p.Length];
                }
                return LineDrawer.GetMeshData(nextPoints, width, material, zValue);
            }
        }

        class UpdateChecker
        {
            MeshDrawer[] bef = new MeshDrawer[0];
            public bool IsUpdated(MeshDrawer[] script)
            {
                if (bef.Length != script.Length) return true;
                for (int i = 0; i < script.Length; i++) if (!bef[i].Equals(script[i])) return true;
                return false;
            }
            public void Update(MeshDrawer[] script)
            {
                bef = new MeshDrawer[script.Length];
                for (int i = 0; i < script.Length; i++) bef[i] = script[i];
            }
        }
    }
}