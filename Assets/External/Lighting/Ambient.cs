using UnityEngine;
using System.Collections;

namespace Library
{
    [ExecuteInEditMode]
    public class Ambient : MonoBehaviour
    {
        public Color color;
        public float intensity = 1.0f;
        
        void Update()
        {
            RenderSettings.ambientIntensity = intensity;
            RenderSettings.ambientLight = color;
        }
    }
}
