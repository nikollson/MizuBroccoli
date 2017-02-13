using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Library.EditablePolygon2D
{
    public class MeshData
    {
        public List<Vector3> vertices = new List<Vector3>();
        public List<Vector2> uv = new List<Vector2>();
        public List<Triangle> triangles = new List<Triangle>();

        public class Triangle
        {
            public List<int> points;
            public int materialNum;
            public Triangle(List<int> data, int materialNum)
            {
                this.points = new List<int>();
                foreach (var a in data) this.points.Add(a);
                this.materialNum = materialNum;
            }
        }

        public void SetVertices(List<Vector3> vertices)
        {
            this.vertices.Clear();
            foreach (var a in vertices) this.vertices.Add(a);
        }
        public void SetUV(List<Vector2> uv)
        {
            this.uv.Clear();
            foreach (var a in uv) this.uv.Add(a);
        }
        public void SetTriangle(List<int> points, int materialNum)
        {
            triangles.Add(new Triangle(points, materialNum));
        }

        public void SetMesh(Mesh mesh, int materialCount)
        {
            mesh.Clear();

            mesh.vertices = ConvArray<Vector3>(this.vertices);
            mesh.uv = ConvArray<Vector2>(this.uv);
            mesh.subMeshCount = materialCount;

            var materialTriangle = new List<int>[materialCount];
            for (int i = 0; i < materialCount; i++) materialTriangle[i] = new List<int>();

            foreach (var a in triangles)
            {
                int num = Mathf.Max(0, Mathf.Min(materialCount - 1, a.materialNum));
                materialTriangle[num].AddRange(a.points);
            }
            for (int i = 0; i < materialCount; i++)
            {
                var tri = ConvArray<int>(materialTriangle[i]);
                mesh.SetTriangles(tri, i);
            }

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

        public Mesh GetMesh(int materialCount)
        {
            Mesh mesh = new Mesh();
            SetMesh(mesh, materialCount);
            return mesh;
        }

        private T[] ConvArray<T>(List<T> list)
        {
            T[] ret = new T[list.Count];
            for (int i = 0; i < list.Count; i++) ret[i] = list[i];
            return ret;
        }

        public static MeshData Marge(MeshData A, MeshData B)
        {
            MeshData ret = new MeshData();
            ret.vertices.AddRange(A.vertices);
            ret.vertices.AddRange(B.vertices);

            ret.uv.AddRange(A.uv);
            ret.uv.AddRange(B.uv);

            ret.triangles.AddRange(A.triangles);

            int P = A.vertices.Count;
            List<Triangle> btri = new List<Triangle>();
            foreach (var b in B.triangles)
            {
                var dat = new List<int>();
                foreach (var bp in b.points) dat.Add(bp + P);
                btri.Add(new Triangle(dat, b.materialNum));
            }
            ret.triangles.AddRange(btri);
            return ret;
        }

        public void Rotate(Vector3 eularAngles)
        {
            Quaternion q = Quaternion.Euler(eularAngles);
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 next = q * vertices[i];
                vertices[i] = next;
            }
        }

        public void Move(Vector3 position)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] += position;
            }
        }
    }
}
