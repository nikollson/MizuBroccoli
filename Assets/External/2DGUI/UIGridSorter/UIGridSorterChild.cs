using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UIGridSorterChild : MonoBehaviour
{

    public float x = 0;
    public float y = 0;
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
        bool updated =  updateDetecter.IsUpdated();
        updateDetecter.DoUpdate();

        if(updated)
        {
            UpdatePosition();
        }
    }

    void UpdatePosition()
    {
        RectTransform rect = this.transform.GetComponent<RectTransform>();

        var parent = this.transform.GetComponentInParent<UIGridSorter>();

        if(parent!= null)
        {
            rect.localPosition = new Vector3(x * parent.W, y * parent.H, 0);
        }
    }


    class UpdateDetecter
    {
        float x;
        float y;
        bool once = false;
        UIGridSorterChild script;

        public UpdateDetecter(UIGridSorterChild script)
        {
            this.script = script;
        }

        public bool IsUpdated()
        {
            if (!once) return true;
            if (this.x != script.x || this.y != script.y) return true;
            return false;
        }

        public void DoUpdate()
        {
            this.x = script.x;
            this.y = script.y;
            once = true;
        }
    }
}
