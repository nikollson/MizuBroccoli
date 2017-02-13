using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Library;
using System;

namespace Library.EditablePolygon2D
{

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [ExecuteInEditMode()]
    public class LineDrawer : MonoBehaviour
    {

        const float INF = 1000000000;

        public float width = 1.0f;
        public Vector2[] vertex;
        [HideInInspector]
        public int materialNum = 0;

        float beforeWidth = -INF;
        int beforeMaterialNum = -1;

        void Update()
        {
            if (beforeWidth != width || beforeMaterialNum != materialNum)
            {
                beforeWidth = width;
                beforeMaterialNum = materialNum;
                UpdateMesh();
            }
        }


        public void SetVertexCount(int count)
        {
            if (vertex.Length == count) return;
            Vector2[] next = new Vector2[count];
            for (int i = 0; i < next.Length; i++) next[i] = (i < vertex.Length) ? vertex[i] : Vector2.zero;
            vertex = next;
            UpdateMesh();
        }

        public void SetPosition(int index, Vector2 position)
        {
            vertex[index] = position;
            UpdateMesh();
        }

        void UpdateMesh()
        {
            if (vertex.Length >= 2)
            {
                MeshData meshData = GetMeshData(vertex, width, materialNum, 0.0f);
                MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
                Mesh mesh = meshData.GetMesh(meshRenderer.sharedMaterials.Length);

                var meshFilter = this.GetComponent<MeshFilter>();
                if (meshFilter.sharedMesh != null)
                {
                    meshFilter.sharedMesh.Clear();
                    DestroyImmediate(meshFilter.sharedMesh);
                }
                meshFilter.sharedMesh = mesh;
            }
        }

        public static MeshData GetMeshData(Vector2[] line, float width, int materialNum, float zValue)
        {
            return MakeMesh.MakeMain(line, width, materialNum, zValue);
        }

        public static MeshData GetMeshDataMk2(Vector2[] line, float width, int materialLeft, int materialMain, int materialRight, float zValue)
        {
            return MakeMesh.MakeMainMk2(line, width, materialLeft, materialMain, materialRight, zValue);
        }

        public static List<List<Vector2>> GetMainLineData(Vector2[] line, float width)
        {
            return MakeMesh.GetMainLineData(line, width);
        }

        // implement class

        class MakeMesh
        {
            public static MeshData MakeMain(Vector2[] line, float width, int materialNum, float zValue)
            {
                var lineData = GetMainLineData(line, width);
                var left = lineData[0];
                var right = lineData[1];

                return MakeMeshData(left, right, width, materialNum, zValue, SetUV_Main);
            }

            public static MeshData MakeMainMk2(Vector2[] line, float width, int materialLeft, int materialMain, int materialRight, float zValue)
            {
                var lineData = GetMainLineData(line, width);
                var left = lineData[0];

                int N = line.Length;
                Vector2[] lineLeft = new Vector2[2];
                Vector2[] lineRight = new Vector2[2];

                Vector2 est = (line[1] - line[0]).normalized;
                Vector2 een = (line[N - 1] - line[N - 2]).normalized;

                lineLeft[0] = line[0] - est * (width / 2);
                lineLeft[1] = line[0] + est * Math.Min(width / 2, (left[0] - left[1]).magnitude / 2);

                lineRight[0] = line[N - 1] - een * Math.Min(width / 2, (left[N - 1] - left[N - 2]).magnitude / 2);
                lineRight[1] = line[N - 1] + een * width / 2;

                Vector2[] lineMain = new Vector2[N];
                for (int i = 0; i < line.Length; i++) lineMain[i] = line[i];
                lineMain[0] = lineLeft[1]; lineMain[N - 1] = lineRight[0];

                var lineDataMain = GetMainLineData(lineMain, width);
                var lineDataLeft = GetMainLineData(lineLeft, width);
                var lineDataRight = GetMainLineData(lineRight, width);

                MeshData meshData = new MeshData();
                meshData = MeshData.Marge(meshData, MakeMeshData(lineDataMain[0], lineDataMain[1], width, materialMain, zValue, SetUV_Main));
                meshData = MeshData.Marge(meshData, MakeMeshData(lineDataLeft[0], lineDataLeft[1], width, materialLeft, zValue, SetUV_Left));
                meshData = MeshData.Marge(meshData, MakeMeshData(lineDataRight[0], lineDataRight[1], width, materialRight, zValue, SetUV_Right));

                return meshData;
            }

            public static List<List<Vector2>> GetMainLineData(Vector2[] line, float width)
            {
                var ret = new List<List<Vector2>>();

                List<Vector2> left = new List<Vector2>();
                List<Vector2> right = new List<Vector2>();

                ConnectTop(left, right, line[0], line[1], width);
                for (int i = 0; i < line.Length - 2; i++)
                {
                    Connect(left, right, line[i], line[i + 1], line[i + 2], width);
                }
                ConnectTop(right, left, line[line.Length - 1], line[line.Length - 2], width);

                ret.Add(left);
                ret.Add(right);
                return ret;
            }

            delegate void SetUVFunc(List<Vector2> a, List<Vector2> b, List<Vector2> c, float d);

            private static MeshData MakeMeshData(List<Vector2> left, List<Vector2> right, float width, int materialNum, float zValue, SetUVFunc setUVFunc)
            {
                var vertex = new List<Vector3>();
                var uv = new List<Vector2>();
                var triangle = new List<int>();

                int N = left.Count;
                int L = 0;
                int R = left.Count;

                foreach (var a in left) vertex.Add(new Vector3(a.x, a.y, zValue));
                foreach (var a in right) vertex.Add(new Vector3(a.x, a.y, zValue));
                foreach (var a in vertex) uv.Add(Vector2.zero);

                for (int i = 0; i < N - 1; i++)
                {
                    var triA = new List<int> { (R + i), (L + i), (L + i + 1) };
                    var triB = new List<int> { (L + i + 1), (R + i + 1), (R + i) };

                    bool rightOut = i - 1 >= 0 && i + 1 < N && TriEqual(right[i - 1], right[i], right[i + 1]);
                    bool leftIn = i + 2 < N && TriEqual(left[i], left[i + 1], left[i + 2]);
                    if (rightOut || leftIn)
                    {
                        triA[2] = (R + i + 1);
                        triB[2] = (L + i);
                    }
                    triangle.AddRange(triA);
                    triangle.AddRange(triB);
                }

                setUVFunc(uv, left, right, width);

                SetUVFunc func = SetUV_Main;

                MeshData meshData = new MeshData();
                meshData.SetVertices(vertex);
                meshData.SetUV(uv);
                meshData.SetTriangle(triangle, materialNum);

                return meshData;
            }

            private static void SetUV_Main(List<Vector2> uv, List<Vector2> left, List<Vector2> right, float width)
            {
                int N = left.Count;
                int R = 0, L = left.Count;
                float lrsum = 0;
                for (int i = 0; i < N - 1; i++)
                {
                    if (i == 0)
                    {
                        uv[L] = new Vector2(0, 0);
                        uv[R] = new Vector2(0, 1);
                    }
                    float ldist = (left[i + 1] - left[i]).magnitude;
                    float rdist = (right[i + 1] - right[i]).magnitude;
                    lrsum += Mathf.Max(ldist, rdist);

                    uv[L + i + 1] = new Vector2(lrsum / width, 0);
                    uv[R + i + 1] = new Vector2(lrsum / width, 1);
                }
            }


            private static void SetUV_Left(List<Vector2> uv, List<Vector2> left, List<Vector2> right, float width)
            {
                int N = left.Count;
                int R = 0, L = left.Count;
                float len = (left[1] - left[0]).magnitude;

                uv[L] = new Vector2(0, 0);
                uv[L + 1] = new Vector2(len/ width, 0);
                uv[R] = new Vector2(0, 1);
                uv[R + 1] = new Vector2(len/width, 1);
            }

            private static void SetUV_Right(List<Vector2> uv, List<Vector2> left, List<Vector2> right, float width)
            {
                int N = left.Count;
                int R = 0, L = left.Count;
                float len = (left[1] - left[0]).magnitude;

                uv[L] = new Vector2(1 - len/width, 0);
                uv[L + 1] = new Vector2(1, 0);
                uv[R] = new Vector2(1 - len/width, 1);
                uv[R + 1] = new Vector2(1, 1);
            }


            private static bool TriEqual(Vector2 A, Vector2 B, Vector2 C)
            {
                return A == B && B == C;
            }

            private static float GetLengthSum(List<Vector2> line)
            {
                float ret = 0.0f;
                for (int i = 0; i < line.Count - 1; i++) ret += (line[i + 1] - line[i]).magnitude;
                return ret;
            }
            private static void ConnectTop(List<Vector2> left, List<Vector2> right, Vector2 A, Vector2 B, float width)
            {
                Vector2 eAB = (B - A).normalized;
                Vector2 tAB = Geometry2D.RotateVector(eAB, Mathf.PI / 2);

                left.Add(A + tAB * width / 2);
                right.Add(A - tAB * width / 2);
            }

            private static void Connect(List<Vector2> left, List<Vector2> right, Vector2 A, Vector2 B, Vector2 C, float width)
            {
                Vector2 eAB = (B - A).normalized;

                float angleABCh = Geometry2D.GetAngle(A - B, C - B) / 2;
                Vector2 eBD = Geometry2D.RotateVector(-eAB, angleABCh);

                Vector2 D = B + eBD * ((width / 2) / Mathf.Abs(Mathf.Sin(angleABCh)));
                //Vector2 E = B - eBD * width / 2;
                Vector2 E = B + (B - D);

                Vector2 outer1 = Geometry2D.Reflection(new Geometry2D.Line(A, B), D);
                Vector2 outer2 = Geometry2D.Reflection(new Geometry2D.Line(D, E), outer1);

                List<Vector2> outer = new List<Vector2>();
                outer.Add(outer1);
                outer.Add(E);
                outer.Add(outer2);

                if (angleABCh >= 0)
                {
                    left.AddRange(outer);
                    for (int i = 0; i < 3; i++) right.Add(D);
                }
                else
                {
                    for (int i = 0; i < 3; i++) left.Add(D);
                    right.AddRange(outer);
                }
            }
        }
    }
}