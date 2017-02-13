using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class UIGridSorter : MonoBehaviour
{
    public float W = 10;
    public float H = 10;

    float wmemo = 0;
    float hmemo = 0;

    UpdateDetecter updateDetecter;

    void Start()
    {
        updateDetecter = new UpdateDetecter(this);
        CheckUpdate();
    }

    void Update()
    {
        CheckUpdate();
    }

    void CheckUpdate()
    {
        if (updateDetecter == null) return;
        bool updated = updateDetecter.IsUpdated();
        updateDetecter.Update();
        
        if(updated)
        {
            foreach(Transform t in transform)
            {
                t.gameObject.SendMessage("UpdatePosition");
            }
        }
    }

    
    class UpdateDetecter
    {
        UIGridSorter script;
        bool once = false;

        float w;
        float h;

        public UpdateDetecter(UIGridSorter script)
        {
            this.script = script;
        }

        public bool IsUpdated()
        {
            if (!once) return true;
            if (w != script.W) return true;
            if (h != script.H) return true;
            return false;
        }

        public void Update()
        {
            w = script.W;
            h = script.H;
            once = true;
        }
    }
}
