using UnityEngine;
using System.Collections;

namespace Library
{
    [ExecuteInEditMode()]
    public class AutoSizeChangeGUI2D : MonoBehaviour
    {
        private CameraUpdateDetecter cameraUpdateDetecter = new CameraUpdateDetecter();
        private SizeManager sizeManager = new SizeManager();

        void Start()
        {
            cameraUpdateDetecter.Start(Camera.main);
            sizeManager.UpdateInnerData(gameObject);
        }

        void LateUpdate()
        {
            cameraUpdateDetecter.UpdateInformation(Camera.main);

            if (cameraUpdateDetecter.IsUpdated())
            {
                sizeManager.UpdateObjectData(gameObject);
            }
            else
            {
                sizeManager.UpdateInnerData(gameObject);
            }
        }

        
        //implement class
        class SizeManager
        {
            Vector3 centerScreenPosition;
            Vector3 neighborScreenPosition;
            float positionZ;
            float scaleZ;

            public void UpdateInnerData(GameObject gameObject)
            {
                Vector3 objectPosition = gameObject.transform.position;
                Vector3 objectScale = gameObject.transform.localScale;
                Vector3 neighborPosition = objectPosition + new Vector3(objectScale.x, objectScale.y, 0);

                centerScreenPosition = Camera.main.WorldToScreenPoint(objectPosition);
                neighborScreenPosition = Camera.main.WorldToScreenPoint(neighborPosition);

                positionZ = objectPosition.z;
                scaleZ = objectScale.z;
            }
            
            public void UpdateObjectData(GameObject gameObject)
            {
                Vector3 centerWoldPositoin = Camera.main.ScreenToWorldPoint(centerScreenPosition);
                Vector3 neightborWoldPosition = Camera.main.ScreenToWorldPoint(neighborScreenPosition);

                gameObject.transform.position = SetZData(centerWoldPositoin, positionZ);
                gameObject.transform.localScale = SetZData(neightborWoldPosition - centerWoldPositoin, scaleZ);
            }


            private Vector3 SetZData(Vector3 position, float z)
            {
                return new Vector3(position.x, position.y, z);
            }
        }

        
        //implement class
        class CameraUpdateDetecter
        {
            Vector3 position;
            float cameraSize;

            bool isStarted = false;
            bool isUpdated = false;
            public void Start(Camera camera)
            {
                UpdateInformation(camera);
            }
            public void UpdateInformation(Camera camera)
            {
                isUpdated = CheckUpdate(camera);
                Update(camera);
            }
            public bool IsUpdated()
            {
                return isUpdated;
            }
            

            bool CheckUpdate(Camera camera)
            {
                bool result = false;
                if (isStarted)
                {
                    if (position.x != camera.transform.position.x) result = true;
                    if (position.y != camera.transform.position.y) result = true;
                    if (position.z != camera.transform.position.z) result = true;
                    if (cameraSize != camera.orthographicSize) result = true;
                }
                isStarted = true;
                return result;
            }

            void Update(Camera camera)
            {
                position = camera.transform.position;
                cameraSize = camera.orthographicSize;
            }
        }
    }
}
