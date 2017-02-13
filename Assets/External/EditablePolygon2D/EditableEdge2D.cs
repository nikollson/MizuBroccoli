using UnityEngine;
using System.Collections;
namespace Library.EditablePolygon2D
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(EdgeCollider2D), typeof(MeshRenderer), typeof(MeshFilter))]
    public class EditableEdge2D : MonoBehaviour
    {

        EdgeCollider2D edgeCollider;
        public int materialNum = 0;
        public float width = 1.0f;
        public Additional additional;
        public AnimateMaterial animateMaterial;

        [System.Serializable]
        public struct Additional
        {
            public bool isAdditional;
            public int materialLeft;
            public int materialRight;
        }

        [System.Serializable]
        public class AnimateMaterial
        {
            public bool isAdditional;
            public int baseMaterialNum;
            public Texture2D[] texture;
            public float timeStep = 0.1f;

            public Texture2D GetTexture()
            {
                int cnt = (int)(Time.time / timeStep) % texture.Length;
                return texture[cnt];
            }
        }

        EdgeUpdateDetecter edgeUpdateDetecter;
        DataUpdateDetecter dataUpdateDetecter;

        MeshFilter meshFilter;
        MeshRenderer meshRenderer;

        void Start()
        {
            edgeCollider = this.GetComponent<EdgeCollider2D>();
            meshFilter = this.GetComponent<MeshFilter>();
            meshRenderer = this.GetComponent<MeshRenderer>();
            MakeMesh();
        }


        void Update()
        {
            if (edgeUpdateDetecter == null) edgeUpdateDetecter = new EdgeUpdateDetecter();
            if (dataUpdateDetecter == null) dataUpdateDetecter = new DataUpdateDetecter();

            if (!Application.isPlaying)
            {
                bool updated = false;
                updated |= edgeUpdateDetecter.IsUpdated(edgeCollider.points);
                edgeUpdateDetecter.Update(edgeCollider.points);
                updated |= dataUpdateDetecter.IsUpdated(this);
                dataUpdateDetecter.Update(this);

                if (updated)
                {
                    MakeMesh();
                }
            }

            if (Application.isPlaying)
            {
                if (animateMaterial.isAdditional)
                {
                    int baseNum = animateMaterial.baseMaterialNum;
                    meshRenderer.materials[baseNum].mainTexture = animateMaterial.GetTexture();
                }
            }
        }

        void MakeMesh()
        {
            MeshData meshData;
            if (additional.isAdditional)
            {
                meshData = LineDrawer.GetMeshDataMk2(edgeCollider.points, width, additional.materialLeft, materialNum, additional.materialRight, 0);
            }
            else
            {
                meshData = LineDrawer.GetMeshData(edgeCollider.points, width, materialNum, 0);
            }

            Mesh mesh = meshData.GetMesh(meshRenderer.sharedMaterials.Length);
            if (meshFilter.sharedMesh != null) DestroyImmediate(meshFilter.sharedMesh);
            meshFilter.mesh = mesh;
        }

        void OnDisable()
        {
            if (meshFilter.sharedMesh != null)
            {
                if (Application.isPlaying) Destroy(meshFilter.sharedMesh);
                else DestroyImmediate(meshFilter.sharedMesh);
            }
        }

        public class EdgeUpdateDetecter
        {
            bool once = false;
            Vector2[] vertex;
            public void Reset()
            {
                once = false;
            }

            public bool IsUpdated(Vector2[] vertex)
            {
                if (!once) return true;
                if (this.vertex == null) return true;
                if (this.vertex.Length != vertex.Length) return true;
                for (int i = 0; i < vertex.Length; i++) if (vertex[i] != this.vertex[i]) return true;
                return false;
            }

            public void Update(Vector2[] vertex)
            {
                once = true;
                this.vertex = new Vector2[vertex.Length];
                for (int i = 0; i < vertex.Length; i++) this.vertex[i] = vertex[i];
            }
        }

        class DataUpdateDetecter
        {
            bool once = false;
            int materialNum;
            float width;
            Additional additional;
            public bool IsUpdated(EditableEdge2D script)
            {
                if (!once) return true;
                if (materialNum != script.materialNum) return true;
                if (!additional.Equals(additional)) return true;
                if (width != script.width) return true;
                return false;
            }

            public void Update(EditableEdge2D script)
            {
                materialNum = script.materialNum;
                additional = script.additional;
                width = script.width;
            }
        }
    }
}