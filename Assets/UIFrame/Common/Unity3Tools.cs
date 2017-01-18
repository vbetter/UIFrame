using UnityEngine;
using System.Collections.Generic;
using System.Xml;


//Layer层枚举
public enum LayerEnum
{
    // 主摄像机开启层
    Default = 0,
    TransparentFX = 1, // 透明层 主摄像机不显示
    IgnoreRaycast = 2,
    Water = 4,
    UI =5,
    Actor=8,              //
    Collider_Walk = 9,
    Collider_Boundary=10,
    Water_Reraction=11,
    Water_Reflection = 12,
    UITop =13,    
    Camera=14,
    Effect = 15,
    StaticActor=16,
    LOD_Item = 17,
    LOD_Grass = 18,
    LOD_Tree = 19,
    UIModel = 20,
    QTEHL = 21,
    NotBlur = 22,
    UIText =23,
    UI_RT = 26,
    Guide = 27,
    Hide = 28,
}

public class Unity3Tools
{
    static public void setLayer(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform t in go.transform)
        {
            Unity3Tools.setLayer(t.gameObject, layer);
        }
    }

    static public void setParent(GameObject go, GameObject parent, bool updateLayer = false)
    {
        if (go == null || parent == null)
        {
            return;
        }

        if (go.transform.parent == parent)
        {
            return;
        }

        Transform t = go.transform;
        Vector3 localScale = t.localScale;
        Vector3 localPosition = t.localPosition;
        Quaternion localRotation = t.localRotation;

        t.parent = parent.transform;
        t.localPosition = localPosition;
        t.localRotation = localRotation;
        t.localScale = localScale;

        if (updateLayer)
        {
            setLayer(go, parent.layer);
        }
    }

    static public T FindInParent<T>(GameObject go) where T : Component
    {
        if (go == null)
        {
            return null;
        }

        object[] comp = go.GetComponents<T>();
       
        if (comp.Length == 0)
        {
            Transform t = go.transform.parent;

            while (t != null && comp.Length == 0)
            {
                comp = t.gameObject.GetComponents<T>();
                t = t.parent;
            }
        }

        if (comp.Length == 0)
        {
            return null;
        }

        return (T)comp[0];
    }

    //创建一张图片
    static public Texture2D CreatTexture(Color color, Vector2 size)
    {
        Texture2D tex = new Texture2D((int)size.x, (int)size.y);
        for (int y = 0; y < tex.height; ++y)
        {
            for (int x = 0; x < tex.width; ++x)
            {
                tex.SetPixel(x, y, color);
		    }
	    }
        return tex;
    }

    //创建一张图片
    static public Texture2D CreatTexture(Color color, Vector2 size, TextureFormat fmt)
    {
        Texture2D tex = new Texture2D((int)size.x, (int)size.y, fmt, false);
        for (int y = 0; y < tex.height; ++y)
        {
            for (int x = 0; x < tex.width; ++x)
            {
                tex.SetPixel(x, y, color);
            }
        }
        return tex;
    }

    public static int SetBitValueTrue(int c, int b)
    {
        return c |= 1 << b;
    }

    static public T AddChild<T>(GameObject parent, GameObject prefab) where T : Component
    {
        T ts = prefab.GetComponent<T>();
        if (ts == null)
        {
            return null;
        }

        GameObject go = GameObject.Instantiate(prefab) as GameObject;
#if UNITY_EDITOR
        UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = parent.layer;
        }
        T tsGO = go.GetComponent<T>();
        return tsGO;
    }

    // 获取屏幕宽
    public static int GetUIWidth(UIRoot root)
    {
        float s = (float)root.activeHeight / Screen.height;
        int width = Mathf.CeilToInt(Screen.width * s);
        return width;
    }

    // 获取屏幕高
    public static int GetUIHeight(UIRoot root)
    {
        float s = (float)root.activeHeight / Screen.height;
        int height = Mathf.CeilToInt(Screen.height * s);
        return height;
    }

    #region 读取解析XML数据
    /************************************
     * 函数说明: 读取一个xml节点的int值
     * 返 回 值: int 
     ************************************/
    public static int XmlReadInt(XmlNode xmlNode, string key, int def)
    {
        int result = 0;
        try
        {
            result = def;
            if (xmlNode != null)
            {
                if (xmlNode.Attributes.GetNamedItem(key) != null)
                {
                    result = int.Parse(xmlNode.Attributes[key].Value);
                }
            }
        }
        catch (System.Exception)
        {
            result = def;
        }
        return result;
    }

    /************************************
     * 函数说明: 读取一个xml节点的float值
     * 返 回 值: float 
     ************************************/
    public static float XmlReadFloat(XmlNode xmlNode, string key, float def)
    {
        float result = 0f;
        try
        {
            result = def;
            if (xmlNode != null)
            {
                if (xmlNode.Attributes.GetNamedItem(key) != null)
                {
                    result = float.Parse(xmlNode.Attributes.GetNamedItem(key).Value);
                }
            }
        }
        catch (System.Exception)
        {
            result = def;
        }
        return result;
    }

    /************************************
     * 函数说明: 读取一个xml节点的long值
     * 返 回 值: float 
     ************************************/
    public static long XmlReadLong(XmlNode xmlNode, string key, long def)
    {
        long result = 0;
        try
        {
            result = def;
            if (xmlNode != null)
            {
                if (xmlNode.Attributes.GetNamedItem(key) != null)
                {
                    result = long.Parse(xmlNode.Attributes[key].Value);
                }
            }
        }
        catch (System.Exception)
        {
            result = def;
        }
        return result;
    }

    /************************************
     * 函数说明: 读取一个xml节点的string值
     * 返 回 值: string 
     ************************************/
    public static string XmlReadString(XmlNode xmlNode, string key, string def)
    {
        string result = "";
        try
        {
            result = def;
            if (xmlNode != null)
            {
                if (xmlNode.Attributes.GetNamedItem(key) != null)
                {
                    result = xmlNode.Attributes[key].Value;
                }
            }
        }
        catch (System.Exception)
        {
            result = def;
        }
        return result;
    }
    #endregion

}
