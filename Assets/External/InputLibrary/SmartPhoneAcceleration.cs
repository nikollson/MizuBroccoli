using UnityEngine;
using System.Collections;

public class SmartPhoneAcceleration : MonoBehaviour {

    [SerializeField] public SlopeGetter slopeGetter;
    [SerializeField] public ShakeDetecter shakeDetecter;
    public bool getMouseInputDefault = false;

    void Start()
    {
        if (getMouseInputDefault) slopeGetter.SetMouseMode(true);
    }

    void Update()
    {
        slopeGetter.Update();
        shakeDetecter.Update();

        UpdateShakeData();
    }

    ShakeData shakeData = new ShakeData(0, false, 0.0f);
    void UpdateShakeData()
    {
        if (shakeData.shaking && !shakeDetecter.IsShake()) shakeData.num++;
        bool isShake = shakeDetecter.IsShake();
        if (!shakeData.shaking && isShake) shakeData.shakeStartTime = Time.time;
        shakeData.shaking = shakeDetecter.IsShake();
    }

    public void SetDefaultAccerelationY(float value)
    {
        slopeGetter.defaultAccerelationY = value;
    }

    public Vector2 GetAnalogpad()
    {
        Vector2 data = slopeGetter.GetAnalogpad();
        return data;
    }

    public ShakeData GetShakeData()
    {
        return shakeData;
    }


    public struct ShakeData
    {
        public int num;
        public bool shaking;
        public float shakeStartTime;

        public ShakeData(int num, bool shaking, float shakeStartTime)
        {
            this.num = num;
            this.shaking = shaking;
            this.shakeStartTime = shakeStartTime;
        }
    }



    //implement class
    class MouseInputDetecter
    {
        public static bool HasMouseInput()
        {
            return Input.touchCount == 0 && (Input.GetMouseButton(0) || Input.GetMouseButton(1));
        }
    }


    // implement class
    [System.Serializable]
    public class ShakeDetecter
    {
        public float shakeDetectThreshold = 0.45f;

        private const int MEMO_SIZE = 10;
        private Vector3[] memo = new Vector3[MEMO_SIZE];
        private int shakeFrameLength = 3;
        private bool isMouseMode = false;

        public void Update()
        {
            for (int i = MEMO_SIZE - 1; i >= 1; i--) memo[i] = memo[i - 1];
            memo[0] = Input.acceleration;

            if (MouseInputDetecter.HasMouseInput()) isMouseMode = true;
        }

        public bool IsShake()
        {
            if (isMouseMode) return IsShakeFromMouse();
            return IsShakeFromAcceleration();
        }


        private bool IsShakeFromMouse()
        {
            return Input.GetMouseButton(1);
        }

        private bool IsShakeFromAcceleration()
        {
            float sum = 0;
            for (int i = 0; i < shakeFrameLength; i++)
            {
                Vector3 deltaMove = memo[i] - memo[i + 1];
                float z = deltaMove.z;
                float y = deltaMove.y;
                sum += Mathf.Sqrt(z * z + y * y);
            }

            if (sum / shakeFrameLength >= shakeDetectThreshold) return true;
            return false;
        }
    }


    //implement class
    [System.Serializable]
    public class SlopeGetter
    {
        private bool isMouseMode = false;
        public float slopeSensitivity = 3.0f;
        public float defaultAccerelationY = -0.5f;
        public float mouseXSensitivity = 1.0f;
        public float mouseYSensitivity = 1.0f;
        public Vector2 mouseCenter = new Vector2(0.5f, 0.5f);
        public float defaultWorldScreenLength = 5.0f;

        public void Update()
        {
            if (MouseInputDetecter.HasMouseInput()) SetMouseMode(true);
        }

        public void SetMouseMode(bool value)
        {
            isMouseMode = value;
        }

        public Vector2 GetAnalogpad()
        {
            if (isMouseMode) return GetAnalogpadFlomMouse();
            return GetAnalogpadFromSlope();
        }

        private Vector2 GetAnalogpadFlomMouse()
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 center = new Vector2(Screen.width * mouseCenter.x, Screen.height * mouseCenter.y);

            Vector2 screenPosition = mousePosition - center;
            Vector2 normalizedPosition = (new Vector2(screenPosition.x * mouseXSensitivity, screenPosition.y * mouseYSensitivity)) / (Screen.width / 2);

            float slopeScaleX = Mathf.Min(1.0f, Mathf.Max(-1.0f, normalizedPosition.x));
            float slopeScaleY = Mathf.Min(1.0f, Mathf.Max(-1.0f, normalizedPosition.y));

            return new Vector2(slopeScaleX, slopeScaleY);
        }

        private Vector2 GetAnalogpadFromSlope()
        {
            Vector3 acceleration = Input.acceleration;
            float slopeScaleX = acceleration.x * slopeSensitivity;
            float slopeScaleY = (acceleration.y - defaultAccerelationY) * slopeSensitivity;
            slopeScaleX = Mathf.Min(1.0f, Mathf.Max(-1.0f, slopeScaleX));
            slopeScaleY = Mathf.Min(1.0f, Mathf.Max(-1.0f, slopeScaleY));

            return new Vector2(slopeScaleX, slopeScaleY);
        }
    }
}
