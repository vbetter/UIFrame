using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;

public enum UI3WndType
{
    [EnumLabel("默认窗口类型")]
    None = 0,
    [EnumLabel("弹出窗口类型")]
    PopUp = 1,
    [EnumLabel("全屏有返回窗口类型")]
    ReturnFullScreen = 2,
    [EnumLabel("全局窗口类型")]
    Global = 3,
    [EnumLabel("全屏无返回窗口类型")]
    DonotReturnFullScreen = 4,
}

public enum UI3WndPanelLayerType
{
    //战斗界面层级，例如飘字，血条
    [EnumLabel("战斗界面使用")]
    BattleLayer ,
    //普通界面层级，
    [EnumLabel("普通界面使用")]
    DefaultLayer,
    //用于底栏、顶栏等层级，
    [EnumLabel("底栏、顶栏专用")]
    TopBarLayer,
    //用于模态窗口等层级，
    [EnumLabel("模态窗口专用")]
    PopUp,
    //用于新手引导等层级，
    [EnumLabel("新手引导层专用")]
    Guide,
    //用于UILoading等层级，
    [EnumLabel("loading界面专用")]
    LoadingLayer,
    [EnumLabel("层级5")]
    Layer5,
    [EnumLabel("层级6")]
    Layer6,
    [EnumLabel("层级7")]
    Layer7,
    [EnumLabel("层级8")]
    Layer8,
    [EnumLabel("层级9")]
    Layer9,
    [EnumLabel("层级10")]
    Layer10,
    [EnumLabel("层级11")]
    Layer11,
    [EnumLabel("层级12")]
    Layer12,
    [EnumLabel("层级13")]
    Layer13,
    [EnumLabel("层级14")]
    Layer14,
}

/// <summary>
/// 用于将Enum类型显示成下拉列表，所需特性，
/// </summary>
[AttributeUsage(AttributeTargets.All | AttributeTargets.Field)]
public class StringLabelAttribute : PropertyAttribute
{
    //下拉列表显示名称，
    public string label;
    //对type类型显示下拉列表，
    public Type type;
    //下拉列表默认选择数据，
    public string strValue;
    public StringLabelAttribute(string label,Type type,string strValue)
    {
        this.label = label;
        this.type = type;
        this.strValue = strValue;
    }
}
/// <summary>
/// 用于将Enum类型显示成下拉列表，所需特性，
/// </summary>
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(StringLabelAttribute))]
public class StringLabelAttributeDrawer : UnityEditor.PropertyDrawer
{
    //下拉列表数据数组，用于检查是否合法数据，
    private static string[] strs = null;
    //下拉列表显示内容，显示比较人性化内容，
    private static string[] labels = null;
    //当前选择内容对应下标，
    private int index = 0;
    //是否初始化，
    private static bool inited = false;
    //选择内容默认数据，
    private string strValue = "";

    //获取下拉列表显示内容，
    public string[] GetLabels()
    {
        //if(inited == false && stringLabelAttribute != null && stringLabelAttribute.type != null)
        {
            Type type = stringLabelAttribute.type;
            strs = Enum.GetNames(type);
            labels = new string[strs.Length];
            strValue = stringLabelAttribute.strValue;
            FieldInfo[] fieldInfoes = type.GetFields();
            int j = 0;
            for(int i = 0;i < fieldInfoes.Length;i++)
            {
                FieldInfo fieldInfo = fieldInfoes[i];
                object[] arr = fieldInfo.GetCustomAttributes(typeof(EnumLabelAttribute), true);
                if (arr.Length > 0)
                {
                    EnumLabelAttribute aa = (EnumLabelAttribute)arr[0];
                    string label = aa.label;
                    labels[j++] = label;
                    //Debug.Log("propers[" + i + "] " + fieldInfoes[i].ToString() + " " + label);
                }
            }
            inited = true;
        }
        return labels;
    }
    private StringLabelAttribute stringLabelAttribute
    {
        get
        {
            return (StringLabelAttribute)attribute;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GetLabels();
        index = -1;
        bool isValided = false;
        for (int i = 0;i < strs.Length;i++)
        {
            if(strs[i] == property.stringValue)
            {
                index = i;
                isValided = true;
                break;
            }
        }

        //没有数据，就设置默认数据，
        if(isValided == false)
        {
            property.stringValue = strValue;
        }

        index = EditorGUI.Popup(position,stringLabelAttribute.label,index,labels);
        if (index >= 0)
        {
            property.stringValue = strs[index];
        }

        //当弹出类型时候，选择模态层级，
        SerializedProperty wndType = property.serializedObject.FindProperty("m_wndType");
        if (wndType != null && wndType.enumValueIndex == (int)UI3WndType.PopUp)
        {
            property.stringValue = UI3WndPanelLayerType.PopUp.ToString();
        }
    }
}
#endif

/// <summary>
/// 用于控制不用类型的UI的panel depth,
/// </summary>

public class UI3WndPanelLayer
{
    public static UI3WndPanelLayer[] layers =
    {
        new UI3WndPanelLayer(UI3WndPanelLayerType.BattleLayer,100,UI3WndPanelLayerType.BattleLayer),
        new UI3WndPanelLayer(UI3WndPanelLayerType.DefaultLayer,1000000,UI3WndPanelLayerType.DefaultLayer),
        new UI3WndPanelLayer(UI3WndPanelLayerType.TopBarLayer,10000,UI3WndPanelLayerType.TopBarLayer),
        //不需要单独栈，所以和DefaultLayer公用一个，
        new UI3WndPanelLayer(UI3WndPanelLayerType.PopUp,10000,UI3WndPanelLayerType.DefaultLayer),
        new UI3WndPanelLayer(UI3WndPanelLayerType.Guide,10000,UI3WndPanelLayerType.Guide),
        new UI3WndPanelLayer(UI3WndPanelLayerType.LoadingLayer,10000,UI3WndPanelLayerType.LoadingLayer),
    };

    public UI3WndPanelLayer(UI3WndPanelLayerType argType,int argStep, UI3WndPanelLayerType argStackLayerType)
    {
        this.type = argType;
        this.begin = curBegin;
        this.end = this.begin + argStep;
        this.step = argStep;
        curBegin += argStep;
        this.stackLayerType = argStackLayerType;
    }

    public UI3WndPanelLayerType type = UI3WndPanelLayerType.DefaultLayer;
    //开始depth,
    public int begin = 0;
    //结束depth,
    public int end = 0;
    //layer的大小,
    public int step = 0;
    //当前栈所在的layer，
    public UI3WndPanelLayerType stackLayerType;

    //当前的开始depth,
    public static int curBegin = 0;
    public override bool Equals(object obj)
    {
        return this.ToString().Equals(obj.ToString());
    }

    public override string ToString()
    {
        return "PANEL_" + type.ToString();
    }

    public override int GetHashCode()
    {
        return (int)type;
    }
}

public abstract class UI3Wnd : MonoBehaviour
{
    // 窗口关闭事件（destroy或hide均可能触发）
    public System.Action OnClose;
    // 窗口完成显示事件（show或showWithScale均可能触发）
    public System.Action OnFinshShow;

    // 窗口是否调用了bringTop（bringTop均可能触发）
    [HideInInspector][SerializeField]
    public bool isBringTop = false;

    // 窗口关闭当前grayscreen事件（点当前窗口类型是PopUp均可能触发）
    public System.Action OnClickGrayScreen;

    // 窗口是否调用了PushBack（PushBack均可能触发）
    [HideInInspector][SerializeField]
    public bool isPushBack = true;

    // 窗口是否调用了init（bringTop均可能触发）
    [HideInInspector][SerializeField]
    public bool isInited = false;

    // 窗口是否调用了show/showWithScale（show/showWithScale均可能触发）
    [HideInInspector][SerializeField]
    public bool isShow = false;

    //窗口type
    [EnumLabel("窗口类型")]
    public UI3WndType m_wndType = UI3WndType.None;
    // 窗口是否处于正在关闭的过程中
    private bool m_isClosing = false;
    
    [HideInInspector][SerializeField]
    public uint ClassMark = 0;//UI编号

    //panel layer type,用于控制panel depth，
    [StringLabelAttribute("窗口层级",typeof(UI3WndPanelLayerType),"DefaultLayer")]
    public string panelLayerType = UI3WndPanelLayerType.DefaultLayer.ToString();

    //是否场景加载时候销毁，
    [SerializeField]
    public bool isDestroyOnLoadLevel = false;

    //是否当全屏时候隐藏，
    [SerializeField]
    public bool isHideOnFullScreen = true;

    public UI3WndType WndType
    {
        get { return m_wndType; }
        set { m_wndType = value; }
    }

    public bool IsClosing()
    {
        return m_isClosing;
    }

    public string GetClassName()
    {
        string className = this.GetType().FullName;
        return className;
    }

    public int getMaxDepth()
    {
        if (this == null || this.gameObject == null)
            return 0;

        return UI3System.getMaxDepth(this.gameObject);
    }

    public int getMinDepth()
    {
        if (this == null || this.gameObject == null)
            return 0;

        return UI3System.getMinDepth(this.gameObject);
    }

    public void addDepth(int depth)
    {
        UI3System.addDepth(this.gameObject, depth);
    }

    public void setDepth(int depth)
    {
        UI3System.setDepth(this.gameObject, depth);
    }

    public bool isActive()
    {
        return gameObject != null && gameObject.activeInHierarchy;
    }

    public void setActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public virtual void Init()
    { }

    public void bringTop()
    {
        UI3System.bringTop(gameObject);
        UI3System.UIStackBringTop(this.GetClassName());
        isBringTop = true;
        UI3System.ShowGrayScreen(this);
    }

    public virtual void show()
    {
        m_isClosing = false;
        OnShow();
    }

    /// <summary>
    /// 激活高斯模糊
    /// 1.激活相机上的高斯模糊脚本
    /// 2.设置当前弹出框的UILayer层级
    /// </summary>
    /// <param name="value"></param>
    public void SetActiveRapidBlur(bool value)
    {
        /*
        GameObject cameraObject = GameObject.FindGameObjectWithTag("UICameraEffect");
        if (cameraObject != null)
        {
            
            RapidBlurEffect effect = cameraObject.GetComponent<RapidBlurEffect>();
            if(effect!=null)
            {
                if (effect.enabled!=value)
                effect.enabled = value;

                int layerMask = value ? (int)LayerEnum.UITop : (int)LayerEnum.UI;
                NGUITools.SetLayer(gameObject, layerMask);
            }
        }
         */
    }

    public virtual void showWithScale(float time = 0.15f)
    {
        m_isClosing = false;
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject.SetActive(true);
        //UIGlobal.myScaleFrom(gameObject, "OnShow", gameObject);
    }

    public virtual void hide(bool isPopup = true)
    {
        isHideOnFullScreen = isPopup;
        UI3System.HideGrayScreen(this);
        if (gameObject.activeInHierarchy)
        {
            OnHide();
        }
    }

    public void hideWithScale(bool isPopup = true)
    {
        m_isClosing = true;

        //UIGlobal.myScaleTo(gameObject,"OnHide",gameObject);
    }

    private void OnShow()
    {
        if (m_wndType == UI3WndType.PopUp)
        {
            //高斯模糊
            SetActiveRapidBlur(true);
        }

        gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        gameObject.SetActive(true);

        if (OnFinshShow != null)
        {
            OnFinshShow();
            OnFinshShow = null;
        }

        UI3System.PushBack(this.GetClassName());
        UI3System.ShowGrayScreen(this);
        bringTop();
    }

    private void OnHide()
    {
        UI3System.HideGrayScreen(this);
        if (gameObject != null)
        {
            if (m_wndType == UI3WndType.PopUp)
            {
                //高斯模糊
                SetActiveRapidBlur(true);
            }

            gameObject.SetActive(false);

            if (OnClose != null)
            {
                OnClose();
                OnClose = null;
            }
            UI3System.HideWindow(this.GetClassName(),gameObject);
        }
        UI3System.PopBack(this.GetClassName());
    }

    public virtual void destroy(bool isPopup = true)
    {
        isHideOnFullScreen = isPopup;
        OnDestroyWithScale();
    }

    public virtual void back(bool isHide = false,bool isPopup = true)
    {
        UI3System.HideGrayScreen(this);
        if(gameObject.activeInHierarchy)
        {
            OnBack(isHide);
        }
    }

    private void OnBack(bool isHide = false)
    {
        UI3System.HideGrayScreen(this);
        if (gameObject != null)
        {
            if (isHide)
            {
                gameObject.SetActive(false);
                UI3System.HideWindow(this.GetClassName(),gameObject);
            }
            else
            {
                gameObject.SetActive(false);
                UI3System.destroyWindow(this.GetClassName(),gameObject);
            }

            if (OnClose != null)
            {
                OnClose();
                OnClose = null;
            }
            UI3System.BackPopup(this.GetClassName());
        }
    }

    public virtual void HideGrayScreen()
    {
        UI3System.HideGrayScreen(this);
    }

    public virtual void destroyWithScale(bool isPopup = true)
    {
        m_isClosing = true;
        isHideOnFullScreen = isPopup;
        //UIGlobal.myScaleTo(gameObject,"OnDestroyWithScale",gameObject);
    }

    private void OnDestroyWithScale()
    {
        if(gameObject != null)
        {
            if (m_wndType == UI3WndType.PopUp)
            {
                //高斯模糊
                SetActiveRapidBlur(false);
            }

            if (OnClose != null)
            {
                OnClose();
                OnClose = null;
            }
            gameObject.SetActive(false);
            UI3System.HideGrayScreen(this);
            UI3System.destroyWindow(this.GetClassName(),gameObject);
        }
        UI3System.PopBack(this.GetClassName());
    }

    void OnDestroy()
    {  
    }

    public int GetPanelLayerBegin()
    {
        UI3WndPanelLayerType layerType = (UI3WndPanelLayerType)Enum.Parse(typeof(UI3WndPanelLayerType), panelLayerType);
        if (UI3WndPanelLayer.layers.Length <= 0 || (int)layerType >= UI3WndPanelLayer.layers.Length)
            return 0;
        return UI3WndPanelLayer.layers[(int)layerType].begin;
    }

    public void reshow()
    {
        UI3System.SetTop(this.GetClassName());
        this.show();
    }
}

