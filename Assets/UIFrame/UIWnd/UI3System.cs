using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 计算节点需要排除自己的。而且可以给刚体调节深度。
 */

public struct GrayScreenStruct
{
    public int Depth;
    public uint UImark;
}

public class UI3System : MonoBehaviour
{
    public static UIRoot m_root;
    public static Camera m_camera;
    public static GameObject m_rootPanel;

    public static bool m_bSceneShow = true;

    public class UIResInfo
    {
        public string name;
        public string resName;
        public uint uiMark;
        public UI3WndType type = UI3WndType.None;
        public string layerType = UI3WndPanelLayerType.DefaultLayer.ToString();
        public UnityEngine.Object prefab = null;
        public List<GameObject> objList = new List<GameObject>();
    }
    private static Dictionary<string, UIResInfo> m_resInfoDict = new Dictionary<string, UIResInfo>();
    private const string RES_LOCK_SCREEN = "UIPrefabs/UILockScreen";
    private static UICamera m_uicamera;
    
    static public Dictionary<string,UIStack<string>> m_wndStacks = new Dictionary<string, UIStack<string>>();
	
    void SetupWnds()
    {
        UIResInfoAssetAssetBundleReader resInfoReaderInstance = UIResInfoAssetAssetBundleReader.Instance as UIResInfoAssetAssetBundleReader;
        Dictionary<string,UIResInfoAsset> resInfoes = UIResInfoAssetAssetBundleReader.dicts;
        List<string> keys = new List<string>(resInfoes.Keys);
        for(int i = 0;i < keys.Count;i++)
        {
            UIResInfoAsset resInfo = resInfoes[keys[i]];
            if(resInfoes != null)
            {
                SetupWnd(resInfo.id,resInfo.path,resInfo.wndType,resInfo.layerType);
            }
        }

        for(int i = 0;i < UI3WndPanelLayer.layers.Length;i++)
        {
            if(m_wndStacks.ContainsKey(UI3WndPanelLayer.layers[i].stackLayerType.ToString()) == true)
                continue;
            m_wndStacks.Add(UI3WndPanelLayer.layers[i].type.ToString(),new UIStack<string>(UI3WndPanelLayer.layers[i].type.ToString()));
        }
    }

    void Awake()
    {
        m_root = gameObject.GetComponent<UIRoot>();
        m_camera = transform.Find("Camera").GetComponent<Camera>();
        m_uicamera = m_camera.GetComponent<UICamera>();
        m_rootPanel = transform.Find("Camera/Panel").gameObject;
        
        //ResManager.Instance.SetupResources();

        SetupWnds();
    }

    private void SetupWnd(string name,string resPath,int wndType = 0,string layerType = "")
    {
        UIResInfo uiResInfo = new UIResInfo();
        uiResInfo.name = name;
        uiResInfo.type = (UI3WndType)wndType;
        uiResInfo.layerType = string.IsNullOrEmpty(layerType) == true ? UI3WndPanelLayerType.DefaultLayer.ToString() : layerType;
        uiResInfo.resName = resPath;

        m_resInfoDict[uiResInfo.name] = uiResInfo;
    }
    public static bool bInit = false;
    public static void Init()
    {
        if (bInit == false)
        {
            bInit = true;

            UnityEngine.Object rootPrefabs = Resources.Load("UI/UI3System");
            GameObject root = GameObject.Instantiate(rootPrefabs) as GameObject;
            GameObject.DontDestroyOnLoad(root);
            root.AddComponent<UI3System>();
            root.SetActive(true);
            //root.AddMissingComponent<ErrorShow>();
            //UITextSys.Init();
        }
    }

    public static UIRoot getUIRoot()
    {
        return m_root;
    }

    public static GameObject getUIRootPanel()
    {
        return m_rootPanel;
    }

    public static GameObject getWndRootPanel<T>() where T: UI3Wnd
    {
        string wndName = typeof(T).ToString();
        return getWndRootPanel(wndName);
    }

    public static GameObject getWndRootPanel(GameObject go)
    {
        if(go == null)
            return null;
        UI3Wnd wnd = go.GetComponent<UI3Wnd>();
        if(wnd == null)
            return null;

        string wndName = wnd.GetClassName(); ;
        return getWndRootPanel(wndName);
    }

    public static GameObject getWndRootPanel(string wndName)
    {
        GameObject root = getUIRootPanel();
        if (root == null)
            return null;
        UIResInfoAsset uiResInfoAsset = UIResInfoAssetAssetBundleReader.Instance.FindById(wndName);
        if (uiResInfoAsset == null)
            return null;

        UI3WndPanelLayerType layerType = (UI3WndPanelLayerType)Enum.Parse(typeof(UI3WndPanelLayerType),uiResInfoAsset.layerType);
        string wndRootName = uiResInfoAsset.layerType.ToString();
        Transform wndTran = root.transform.Find(wndRootName);
        if (wndTran == null)
        {
            GameObject panelGO = new GameObject(wndRootName);
            panelGO.transform.parent = root.transform;
            panelGO.transform.localPosition = Vector3.zero;
            panelGO.transform.localRotation = Quaternion.identity;
            panelGO.transform.localScale = Vector3.one;
            panelGO.layer = root.layer;
            wndTran = root.transform.Find(wndRootName);
        }

        int beginDepth = 0;
        if (UI3WndPanelLayer.layers[(int)layerType] != null)
            beginDepth = UI3WndPanelLayer.layers[(int)layerType].begin;

        UIPanel panel = wndTran.gameObject.GetComponent<UIPanel>();
        if (panel == null)
        {
            panel = wndTran.gameObject.AddComponent<UIPanel>();
            panel.depth = beginDepth;
        }

        return wndTran.gameObject;
    }

    public static void addCameraCullingMask(string layerName)
    {
        m_camera.cullingMask |= (1 << LayerMask.NameToLayer(layerName));
    }

    public static void delCameraCullingMask(string layerName)
    {
        m_camera.cullingMask &= ~(1 << LayerMask.NameToLayer(layerName));
    }

    public static void SetUILayer()
    {
        m_uicamera.eventReceiverMask = (1 << (int)LayerEnum.UI);
        m_uicamera.eventReceiverMask |= (1 << (int)LayerEnum.UIModel);
    }

    public static void SetUnUILayer()
    {
        m_uicamera.eventReceiverMask = 1 << (int)LayerEnum.Guide;
    }

    // 获取屏幕高
    public static int getHeight()
    {
        float s = (float)m_root.activeHeight / Screen.height;
        int height = Mathf.CeilToInt(Screen.height * s);
        return height;
    }

    // 获取屏幕宽
    public static int getWidth()
    {
        float s = (float)m_root.activeHeight / Screen.height;
        int width = Mathf.CeilToInt(Screen.width * s);
        return width;
    }

    // 获取最深的子节点,当参数go不空时候，表示出了go这个节点其他节点最深子节点，
    public static int getMaxDepth(GameObject obj,GameObject go = null)
    {
        int depth = int.MinValue;

        UIPanel[] panels = obj.GetComponentsInChildren<UIPanel>(false);
        UIPanel goPanel = null;
        if(go != null)
            goPanel = go.GetComponent<UIPanel>();
        for (int i = 0; i < panels.Length; ++i)
        {
            UIPanel p = panels[i];
            if(goPanel == p)
                continue;
            depth = Mathf.Max(depth, p.depth);
        }

        if (depth == int.MinValue)
        {
            depth = 0;
        }

        return depth;
    }

    #region 判断该ui是否在最上层

    /// <summary>
    ///判断是否需要bringTop，当panel已经是最顶层了（最顶层只有一个）
    /// </summary>
    /// <param name="root">当前类型stack root</param>
    /// <param name="go">当前panel所在的go</param>
    /// <returns></returns>
    public static bool canBringTop(GameObject root,GameObject go)
    {
        int depth = int.MinValue;
        UIPanel goPanel = go.GetComponent<UIPanel>();

        if(goPanel == null)
            return false;
        depth = getMaxDepth(root,go);
        return depth < goPanel.depth;
    }

    static bool CanGetDepth(string name)
    {
        string subStr = name.Substring(0, 2);
        if (subStr == "UI" && name != "UITop(Clone)" && name !="UIFightLoading(Clone)") return true;

        return false;
    }

    /// <summary>
    /// 判断该ui是否是当前最上层单独的ui
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    static public bool IsTopAndOnlyUI(GameObject obj)
    {
        GameObject root = getWndRootPanel(obj);
        if (root == null)
            return false;

        return canBringTop(root, obj) == false;
    }

    #endregion

    // 获取最浅的子节点
    public static int getMinDepth(GameObject obj)
    {
        int depth = int.MaxValue;

        UIPanel[] panels = obj.GetComponentsInChildren<UIPanel>(true);
        for (int i = 0; i < panels.Length; ++i)
        {
            UIPanel p = panels[i];
            //if (p.gameObject != m_lockScreen.gameObject)
            //{
                depth = Mathf.Min(depth, p.depth);
            //}
        }

        if (depth == int.MaxValue)
        {
            depth = 0;
        }

        return depth;
    }

    public static int getMinWidgetDepth(GameObject obj)
    {
        int depth = int.MaxValue;

        UIWidget[] widgets = obj.GetComponentsInChildren<UIWidget>(true);
        for (int i = 0; i < widgets.Length; ++i)
        {
            UIWidget w = widgets[i];
            //if (p.gameObject != m_lockScreen.gameObject)
            //{
            depth = Mathf.Min(depth, w.depth);
            //}
        }

        if (depth == int.MaxValue)
        {
            depth = 0;
        }

        return depth;
    }

    public static int getMaxWidgetDepth(GameObject obj)
    {
        int depth = int.MinValue;

        UIWidget[] widgets = obj.GetComponentsInChildren<UIWidget>(true);
        for (int i = 0; i < widgets.Length; ++i)
        {
            UIWidget w = widgets[i];
            //if (p.gameObject != m_lockScreen.gameObject)
            //{
            depth = Mathf.Max(depth, w.depth);
            //}
        }

        if (depth == int.MaxValue)
        {
            depth = 0;
        }

        return depth;
    }

    public static Vector2 getMaxWidgetSize(GameObject obj)
    {
        Vector2 size = Vector2.zero;

        UIWidget[] widgets = obj.GetComponentsInChildren<UIWidget>(false);
        for(int i = 0;i < widgets.Length;i++)
        {
            UIWidget w = widgets[i];
            size.x = Mathf.Max(size.x, w.width);
            size.y = Mathf.Max(size.y, w.width);
        }

        return size;
    }

    public static Vector2 getMinWidgetSize(GameObject obj)
    {
        Vector2 size = Vector2.zero;

        UIWidget[] widgets = obj.GetComponentsInChildren<UIWidget>(false);
        for(int i = 0;i < widgets.Length;i++)
        {
            UIWidget w = widgets[i];
            size.x = Mathf.Min(size.x, w.width);
            size.y = Mathf.Min(size.y, w.width);
        }

        return size;
    }

    // 每个节点都加上这个深度,可以是负值
    public static void addDepth(GameObject obj, int depth)
    {
        UIPanel[] panels = obj.GetComponentsInChildren<UIPanel>(true);
        for (int i = 0; i < panels.Length; ++i)
        {
            UIPanel p = panels[i];
            p.depth += depth;
        }
    }

    public static void setDepth(GameObject obj, int depth)
    {
        int minDepth = getMinDepth(obj);
        int subDepth = depth - minDepth;
        addDepth(obj, subDepth);
    }

    public static Dictionary<uint, GameObject> CurrentOpenUI()
    {
        Dictionary<uint, GameObject> OpenUI = new Dictionary<uint, GameObject>();
        foreach(KeyValuePair<string,UIResInfo> main in m_resInfoDict)
        {
            if(main.Value.objList.Count>0)
            {
                for(int i=0;i<main.Value.objList.Count;i++)
                {
                    if(main.Value.objList[i].activeInHierarchy==true)
                    {
                        OpenUI[main.Value.uiMark] = main.Value.objList[i].gameObject;
                        continue;
                    }
                }
            }
        }
        return OpenUI;
    }

    // 加载预设
    public static UnityEngine.Object loadWindowRes<T>()
    {
        string className = typeof(T).ToString();
        return loadWindowRes(className);
    }

    public static UnityEngine.Object loadWindowRes(string className)
    {
        UIResInfo uiResInfo = null;
        if(m_resInfoDict.TryGetValue(className,out uiResInfo))
        {
            //return ResManager.Instance.LoadPrefab(uiResInfo.resName);
        }
        return null;
    }

    public static T findWindow<T>() where T : UI3Wnd
    {
        return findWindow<T>(0);
    }

    public static T findWindow<T>(int index) where T : UI3Wnd
    {
        string name = typeof(T).ToString();

        UIResInfo resInfo;
        if (!m_resInfoDict.TryGetValue(name, out resInfo))
        {
            return null;
        }

        if (resInfo.objList.Count > index)
        {
            return resInfo.objList[index].GetComponent<T>();
        }

        return null;
    }

    public static UI3Wnd findWindow(string className)
    {
        return findWindow(className, 0);
    }

    public static UI3Wnd findWindow(string className, int index)
    {
        UIResInfo resInfo;
        if (!m_resInfoDict.TryGetValue(className, out resInfo))
        {
            return null;
        }

        if (resInfo.objList.Count > index)
        {
            return resInfo.objList[index].GetComponent<UI3Wnd>();
        }

        return null;
    }

    // 创建
    public static T createWindow<T>() where T : UI3Wnd
    {
        GameObject parent = getWndRootPanel<T>();
        return createWindow<T>(parent, true);
    }

    // 创建
    public static T createWindow<T>(bool bSingle) where T : UI3Wnd
    {
        GameObject parent = getWndRootPanel<T>();
        return createWindow<T>(parent, bSingle);
    }

    // 创建
    public static T createWindow<T>(GameObject parent, bool bSingle) where T : UI3Wnd
    {
        if (bSingle)
        {
            T window = findWindow<T>();
            if (window != null)
            {
                return window;
            }
        }

        GameObject res = loadWindowRes<T>() as GameObject;
        if (res == null)
        {
            return null;
        }

        GameObject uiGo = NGUITools.AddChild(parent, res);
        UIAnchor[] Anchor = uiGo.GetComponentsInChildren<UIAnchor>(true);
        for (int i = 0; i < Anchor.Length; ++i)
        {
            if (Anchor[i].uiCamera == null)
            {
                Anchor[i].uiCamera = Unity3Tools.FindInParent<Camera>(uiGo);
            }
        }
        uiGo.SetActive(false);

        T t = uiGo.AddMissingComponent<T>();

        string className = typeof(T).ToString();
        UIResInfo resInfo = m_resInfoDict[className];
        if (resInfo != null)
        {
            t.WndType = resInfo.type;
            t.panelLayerType = resInfo.layerType;
            resInfo.objList.Add(uiGo);
        }

        ((UI3Wnd)t).ClassMark = resInfo.uiMark;

        return t;
    }

    public static UI3Wnd createWindow(string className)
    {
        UI3Wnd window = findWindow(className);
        if (window != null)
        {
            return window;
        }

        GameObject res = loadWindowRes(className) as GameObject;
        if (res == null)
        {
            return null;
        }

        GameObject parent = getWndRootPanel(className);
        GameObject uiGo = NGUITools.AddChild(parent, res);
        UIAnchor[] Anchor = uiGo.GetComponentsInChildren<UIAnchor>(true);
        for (int i = 0; i < Anchor.Length; ++i)
        {
            if (Anchor[i].uiCamera == null)
            {
                Anchor[i].uiCamera = Unity3Tools.FindInParent<Camera>(uiGo);
            }
        }
        uiGo.SetActive(false);

        window = uiGo.AddMissingComponent<UI3Wnd>();

        int layerDepth = 0;
        UI3WndPanelLayerType layerType = (UI3WndPanelLayerType)Enum.Parse(typeof(UI3WndPanelLayerType),window.panelLayerType);
        if (UI3WndPanelLayer.layers.Length > (int)layerType)
            layerDepth = UI3WndPanelLayer.layers[(int)layerType].begin;
        window.addDepth(layerDepth);

        UIResInfo resInfo = m_resInfoDict[className];
        if (resInfo != null)
        {
            window.WndType = resInfo.type;
            resInfo.objList.Add(uiGo);
        }

        window.ClassMark = resInfo.uiMark;

        return window;
    }

    public static void destroyWindow<T>(GameObject uiGo) where T : UI3Wnd
    {
        string className = typeof(T).ToString();

        destroyWindow(className, uiGo);
    }

    public static void destroyWindow(string className, GameObject uiGo)
    {
        UIResInfo resInfo;
        if (m_resInfoDict.TryGetValue(className, out resInfo))
        {
            for (int i = 0; i < resInfo.objList.Count; i++)
            {
                if (uiGo == resInfo.objList[i])
                {
                    NGUITools.Destroy(uiGo);
                    resInfo.objList[i] = null;
                    resInfo.objList.RemoveAt(i);
                }
            }

            PopBack(resInfo.name);
            resInfo.objList.Clear();
            resInfo.prefab = null;
        }
    }

    public static void destroyWindow<T>()
    {
        string name = typeof(T).ToString();
        UIResInfo resInfo;
        if (m_resInfoDict.TryGetValue(name, out resInfo))
        {
            for (int i = 0; i < resInfo.objList.Count; i++)
            {
                if (resInfo.objList[i] != null)
                {
                    NGUITools.Destroy(resInfo.objList[i]);
                    resInfo.objList[i] = null;
                }
            }
            PopBack(name);
            resInfo.objList.Clear();
            resInfo.prefab = null;
        }
    }

    public static void destroyAllWindows(bool destroyAll = false)
    {
        foreach (KeyValuePair<string, UIResInfo> pair in m_resInfoDict)
        {
            UIResInfo resInfo = pair.Value;
            if (destroyAll == false)
            {
                UIResInfoAsset resInfoAsset = UIResInfoAssetAssetBundleReader.Instance.FindById(resInfo.name);
                if(resInfoAsset != null && resInfoAsset.wndType == (int)UI3WndType.Global)
                    continue;
            }
            for (int i = 0; i < resInfo.objList.Count; i++)
            {
                if (resInfo.objList[i] != null)
                {
                    NGUITools.Destroy(resInfo.objList[i]);
                    resInfo.objList[i] = null;
                }
            }
            PopBack(resInfo.name);
            resInfo.objList.Clear();
            resInfo.prefab = null;
        }

        Resources.UnloadUnusedAssets();
    }

    public static void destroyAllWindowsOnLoadScene(bool destroyAll = false)
    {
        foreach (KeyValuePair<string, UIResInfo> pair in m_resInfoDict)
        {
            UIResInfo resInfo = pair.Value;
            if (destroyAll == false)
            {
                UIResInfoAsset resInfoAsset = UIResInfoAssetAssetBundleReader.Instance.FindById(resInfo.name);
                if (resInfoAsset != null && resInfoAsset.wndType == (int)UI3WndType.Global)
                    continue;
            }
            if (resInfo.objList.Count > 0)
            {
                UI3Wnd wnd = resInfo.objList[0].GetComponent<UI3Wnd>();
                if (wnd != null && wnd.isDestroyOnLoadLevel == true)
                {
                    for (int i = 0; i < resInfo.objList.Count; i++)
                    {
                        if (resInfo.objList[i] != null)
                        {
                            NGUITools.Destroy(resInfo.objList[i]);
                            resInfo.objList[i] = null;
                        }
                    }
                    PopBack(resInfo.name);
                    resInfo.objList.Clear();
                    resInfo.prefab = null;
                }
            }
        }

        Resources.UnloadUnusedAssets();
    }

    public static void HideWindow<T>(GameObject uiGo) where T : UI3Wnd
    {
        string className = typeof(T).ToString();

        HideWindow(className, uiGo);
    }

    public static void HideWindow(string className, GameObject uiGo)
    {
        UIResInfo resInfo;
        if (m_resInfoDict.TryGetValue(className, out resInfo))
        {
            for (int i = 0; i < resInfo.objList.Count; i++)
            {
                if (uiGo == resInfo.objList[i])
                {
                    NGUITools.SetActive(uiGo,false);
                }
            }
            PopBack(resInfo.name);
        }
    }

    public static void HideWindow<T>()
    {
        string name = typeof(T).ToString();
        UIResInfo resInfo;
        if (m_resInfoDict.TryGetValue(name, out resInfo))
        {
            for (int i = 0; i < resInfo.objList.Count; i++)
            {
                if (resInfo.objList[i] != null)
                {
                    NGUITools.SetActive(resInfo.objList[i], false);
                }
            }
            PopBack(resInfo.name);
        }
    }

    public static void ClearUIByLayerType(UI3WndPanelLayerType type)
    {
        foreach (KeyValuePair<string, UIResInfo> pair in m_resInfoDict)
        {
            UIResInfo resInfo = pair.Value;
            for (int i = 0; i < resInfo.objList.Count; i++)
            {
                if (resInfo.objList[i] != null && resInfo.layerType == type.ToString())
                {
                    NGUITools.Destroy(resInfo.objList[i]);
                    resInfo.objList[i] = null;
                    resInfo.objList.Clear();
                    resInfo.prefab = null;
                }
            }
        }

        ClearStack(type);
    }

    public static void ShowWindows<T>()
    {
        string className = typeof(T).ToString();

        ShowWindows(className);
    }

    public static void ShowWindows(string className)
    {
        UIResInfo resInfo;
        if (m_resInfoDict.TryGetValue(className, out resInfo))
        {
            PushBack(className);
            for (int i = 0; i < resInfo.objList.Count; i++)
            {
                if (resInfo.objList[i] != null)
                {
                    NGUITools.SetActive(resInfo.objList[i], true);
                }
            }
        }
    }
    public static void lockScreen(string text, float timeOut, Action timeoutCallback = null)
    {
        //if (timeOut <= 0)
        //    return;

        //if (m_lockScreen == null)
        //    return;

        //m_lockScreen.Lock(text, timeOut, timeoutCallback);
    }

    public static void unlockScreen()
    {
        //if (m_lockScreen != null)
        //{
        //    m_lockScreen.Unlock();
        //}
    }

    public static void bringTop(GameObject obj)
    {
        if (obj == null)
            return;
        GameObject root = getWndRootPanel(obj);
        if(root == null)
            return;
        UIPanel panel = obj.GetComponent<UIPanel>();
        if(panel == null)
            return;
        int maxDepth = getMaxDepth(root.gameObject,obj);
        if(maxDepth < panel.depth)
            return;
        int minDepth = getMinDepth(obj);
        addDepth(obj, maxDepth + (0 - minDepth) + 2);
    }

    public static GameObject getLockGameobject()
    {
        return null;// m_lockScreen.gameObject;
    }

    public static void hideScene()
    {

    }

    public static void showScene()
    {
        if (m_bSceneShow == true)
        {
            return;
        }
    }

    /// <summary>
    /// 根据不同layer，进行入栈，当栈中已经有这个界面，不入栈，isPushBack属性，用于控制是否需要栈管理，例如，GrayScreen不需要栈管理，
    /// </summary>
    /// <param name="name"></param>
    public static void PushBack(string name)
    {
        if(string.IsNullOrEmpty(name) == true)
            return;
        string panelLayer = GetPanelLayer(name);
        UIStack<string> stack = GetStackByPanelLayer(panelLayer);
        if(stack == null)
            return;
        if(stack.Have(name))
        {
            return;
        }
        else if(stack.Have(GetValid(name, true)))
        {
            UIStackBringTop(name);
            return;
        }
        UIResInfoAsset resInfo = GetUIResInfoAsset(name);
        stack.DisplayList();
        OnPushBack(resInfo,stack);
        stack.DisplayList();
    }

    /// <summary>
    /// 根据不同layer，出栈，在获取上一个bringTop界面，在进行bringTop操作，
    /// 该方法只适用于ReturnFullScreen界面，
    /// </summary>
    /// <param name="name"></param>
    public static void PopBack(string name)
    {
        if (IsVaild(name,false) == false)
            return;
        string panelLayer = GetPanelLayer(name);
        UIStack<string> stack = GetStackByPanelLayer(panelLayer);
        if (stack == null)
            return;
        stack.DisplayList();

        int index = stack.FindFirstByName(name);

        if(index < 0)
            return;
        //if(CanPopBack(name,index,stack) == false)
        //    return;
        bool isTop = stack.IsTop(name);
        bool isPopup = true;
        UI3Wnd wnd = findWindow(name);
        if(wnd != null)
        {
            isPopup = wnd.isHideOnFullScreen;
        }
        if(isPopup == false)
        {
            return;
        }
        stack.PopAt(index);
    }

    /// <summary>
    /// 出栈指定元素，不会返回之前的元素，
    /// </summary>
    public static bool CanPopBack(string name,int index,UIStack<string> stack)
    {
        if(index < 0 || stack == null)
            return false;

        string nextName = stack.Peek(index + 1);
        if(string.IsNullOrEmpty(nextName) == true)
            return true;
        UI3WndType wndType = GetWndType(nextName);
        return wndType != UI3WndType.ReturnFullScreen;
    }

    public static bool IsVaild(string name,bool isPush,bool isDebug = false)
    {
        if(string.IsNullOrEmpty(name) == true)
            return false;
        if(isPush)
        {
            if(name.IndexOf("____CLONE") < 0)
            {
                if(isDebug)
                {
                    Debug.LogError("Push is not IsVaild " + name);
                }
                return false;
            }
        }
        else
        {
            if (name.IndexOf("____CLONE") >= 0)
            {
                if(isDebug)
                {
                    Debug.LogError("Push CLONE is not IsVaild " + name);
                }
                return false;
            }
        }
        return true;
    }

    public static string GetValid(string name,bool isPush)
    {
        if(string.IsNullOrEmpty(name) == true)
        {
            return "";
        }

        if(isPush)
        {
            if(IsVaild(name,true) == true)
                return name;
            return name + "____CLONE";
        }else
        {

            if (!name.Contains("____CLONE"))
            {
                return name;
            }
            else
            {
                return name.Substring(0, name.IndexOf("____CLONE"));
            }
        }
    }
    /// <summary>
    /// bringTop当前栈界面，
    /// </summary>
    /// <param name="name"></param>
    public static void UIStackBringTop(string name,bool isClearStack = false)
    {
        string panelLayer = GetPanelLayer(name);
        UIStack<string> stack = GetStackByPanelLayer(panelLayer);
        if (stack == null)
            return;
        stack.DisplayList();
        string pushName = GetValid(name,true);
        if (stack.Have(name) == false && stack.Have(pushName) == false)
            return;
        if (stack.IsTop(name) == true)
            return;
        /****
         * 
        UI3WndType wndType = UI3WndType.None;
        UI3WndType nextWndType = UI3WndType.None;
        //UI3WndType lastWndType = UI3WndType.None;
        int nextIndex = -1;
        wndType = GetWndType(name);
        int index = stack.FindFirstByName(name);
        if (index < 0)
            return;
        string nextName = stack.FindDifferenct(index,name,out nextIndex);
        if(string.IsNullOrEmpty(nextName) == false)
        {
            nextWndType = GetWndType(nextName);
            int lastIndex = -1;
            string lastName = stack.FindDifferenct(index,name,out lastIndex,false);
            if (nextWndType == UI3WndType.ReturnFullScreen)
            {
                if (lastIndex < 0)
                {
                    stack.PopAt(nextIndex - 1);
                }
                else
                {
                    stack.SetAt(nextIndex - 1,lastName);
                }
            }
        }
        stack.PopAt(index);
        if (wndType == UI3WndType.ReturnFullScreen)
        {
            stack.PopAt(index - 1);
        }
        **
        ****/
        //当前界面存在，当前复制界面存在，
        if(stack.Have(name) && stack.Have(pushName))
        {
            int index = stack.FindFirstByName(name);
            string lastName = stack.Peek(index - 1);
            //上一个界面不为空，上一个界面没有被复制，需要被复制，
            if(string.IsNullOrEmpty(lastName) == false && IsVaild(lastName,true) == false)
            {
                stack.SetAt(index,GetValid(lastName,true));
            }
        }
        //只是当前界面存在，同时，上一个元素是当前界面的复制界面，所以需要清除复制界面，
        else if(stack.Have(name))
        {
            int index = stack.FindFirstByName(name);
            string lastName = stack.Peek(index - 1);
            if(string.IsNullOrEmpty(lastName) == false && IsVaild(lastName,true) == true)
            {
                stack.PopAt(lastName);
            }
        }
        //只是当前界面复制界面存在，需要判断上一个界面是否被复制，没有复制就复制上一个界面，
        else
        {
            int index = stack.FindFirstByName(pushName);
            string lastName = stack.Peek(index - 1);
            if(string.IsNullOrEmpty(lastName) == false && IsVaild(lastName,true) == false)
            {
                stack.SetAt(index,GetValid(lastName, true));
            }
        }
        //出栈当前界面原界面和复制界面，
        stack.PopAt(name);
        stack.PopAt(pushName);
        PushBack(name);
        stack.DisplayList();
    }

    public static void ClearStack(UI3WndPanelLayerType type)
    {
        string panelLayer = type.ToString();
        UIStack<string> stack = GetStackByPanelLayer(panelLayer);
        if (stack == null)
            return;
        stack.Clear();
    }

    public static void ClearStack(string name)
    {
        string panelLayer = GetPanelLayer(name);
        UIStack<string> stack = GetStackByPanelLayer(panelLayer);
        if (stack == null)
            return;
        stack.Clear();
    }
    /// <summary>
    /// 获取上一个bringTop界面
    /// </summary>
    /// <param name="stack"></param>
    /// <returns></returns>
    public static string Peek(UIStack<string> stack)
    {
        //string lastStr = "";
        //List<string> list = stack.List;
        //if(list != null || list.Count > 0)
        //{
        //    for(int i = list.Count - 1; i >= 0;i--)
        //    {
        //        UI3Wnd curWnd = findWindow(list[i]);
        //        if(curWnd != null && curWnd.isBringTop == true)
        //            return lastStr = curWnd.GetClassName();
        //    }

        //}
        return stack.Peek();
    }

    public static void SetTop(string name)
    {
        string panelLayer = GetPanelLayer(name);
        UIStack<string> stack = GetStackByPanelLayer(panelLayer);
        if (stack == null)
            return;
        stack.SetTop(name);
    }

    public static void Reshow(string name)
    {
        SetTop(name);
        ShowWindows(name);
    }
    /// <summary>
    /// 入栈完成之后操作，
    /// 需要注意，这些步骤不能修改，修改的话，会有问题，
    /// 步骤1，打开ReturnFullScreen界面时候，需要先复制栈顶界面在入栈本界面，
    /// 步骤2，打开全屏界面时候需要隐藏之前接口，同时隐藏PopUp layer之前所有界面，
    /// 步骤3，打开DonotReturnFullScreen界面时候，清空栈，
    /// 步骤4，入栈该界面，这个时候只有一个界面，
    /// </summary>
    /// <param name="wnd"></param>
    public static void OnPushBack(UIResInfoAsset resInfoAsset,UIStack<string> stack = null)
    {
        stack = stack == null ? GetStackByPanelLayer(resInfoAsset.layerType.ToString()) : stack;
        if (stack == null)
            return;
        stack.DisplayList();
        UI3WndType wndType = (UI3WndType)resInfoAsset.wndType;
        //步骤1，打开ReturnFullScreen界面时候，需要先复制栈顶界面在入栈本界面，
        if(wndType == UI3WndType.ReturnFullScreen)
        {
            string lastT = stack.Peek();
            lastT = GetValid(lastT,true);
            if(stack.Have(lastT) == false)
            {
                stack.PushBack(lastT);
            }
        }

        //步骤2，打开全屏界面时候需要隐藏之前接口，同时隐藏PopUp layer之前所有界面，
        if (wndType == UI3WndType.ReturnFullScreen || wndType == UI3WndType.DonotReturnFullScreen)
        {
            HideStackWindows(resInfoAsset.layerType, resInfoAsset.name);
        }

        //步骤3，打开DonotReturnFullScreen界面时候，清空栈，
        if (wndType == UI3WndType.DonotReturnFullScreen)
        {
            //当前layer栈clear，
            stack.Clear();
        }

        //步骤4，入栈该界面，这个时候只有一个界面，
        if(GetIsPushStack(resInfoAsset.name) == true)
        {
            stack.PushBack(resInfoAsset.name);
        }
    }

    /// <summary>
    /// 出栈需要返回界面（栈顶），显示界面，
    /// </summary>
    /// <param name="wnd"></param>
    public static void BackPopup(string name)
    {
        UI3WndType type = GetWndType(name);
        if (UI3WndType.ReturnFullScreen != type)
        {
            return;
        }
        UIStack<string> stack = GetStackByWndName(name);
        string lastName = Peek(stack);
        lastName = GetValid(lastName, false);
        stack.DisplayList();
        if (string.IsNullOrEmpty(lastName) == true)
            return;
        UI3Wnd wnd = createWindow(lastName);
        if (wnd == null)
            return;
        int index = stack.FindLastByName(GetValid(lastName, true));
        stack.PopAt(index);
        wnd.show();
        stack.DisplayList();
    }

    /// <summary>
    /// 显示GrayScreen，
    /// </summary>
    /// <param name="wnd"></param>
    /// <param name="OnClickGrayScreen"></param>
    public static void ShowGrayScreen(UI3Wnd wnd,System.Action OnClickGrayScreen = null)
    {
        if (wnd == null || wnd.gameObject == null || wnd.WndType != UI3WndType.PopUp || wnd.GetClassName() == "UIGrayScreen")
            return;
        UIGrayScreen grayWnd = createWindow<UIGrayScreen>();
        if(grayWnd == null)
            return;
        grayWnd.OnClickEventCallback = OnClickGrayScreen != null ? OnClickGrayScreen : wnd.OnClickGrayScreen;
        grayWnd.Wnd = wnd.gameObject;
        int depth = wnd.getMinDepth();
        grayWnd.setDepth(depth - 1);
        grayWnd.setActive(true);
    }

    /// <summary>
    /// 隐藏GrayScreen，
    /// </summary>
    /// <param name="wnd"></param>
    public static void HideGrayScreen(UI3Wnd wnd)
    {
        if (wnd == null || wnd.gameObject == null || wnd.WndType != UI3WndType.PopUp || wnd.GetClassName() == "UIGrayScreen")
            return;
        UIGrayScreen grayWnd = findWindow<UIGrayScreen>();
        if (grayWnd == null)
            return;
        string panelLayer = GetPanelLayer(wnd.GetClassName());
        UIStack<string> stack = GetStackByPanelLayer(panelLayer);
        if (stack == null)
            return;
        UI3Wnd lastWnd = GetLastGrayScreen(stack);
        if(lastWnd == null)
        {
            grayWnd.setActive(false);
        }else
        {
            ShowGrayScreen(lastWnd);
        }
    }

    /// <summary>
    /// 获取上一个Popup类型的界面，
    /// </summary>
    /// <param name="stack"></param>
    /// <returns></returns>
    public static UI3Wnd GetLastGrayScreen(UIStack<string> stack)
    {
        UI3Wnd lastWnd = null;
        List<string> list = stack.List;
        if (list != null || list.Count > 0)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                UI3Wnd curWnd = findWindow(list[i]);
                if (curWnd != null && curWnd.WndType == UI3WndType.PopUp && curWnd.gameObject.activeInHierarchy == true)
                    return lastWnd = curWnd;
            }

        }
        return lastWnd;
    }

    public static UIResInfoAsset GetUIResInfoAsset(string name)
    {
        UIResInfoAsset resInfo = UIResInfoAssetAssetBundleReader.Instance.FindById(name);
        return resInfo;
    }
    /// <summary>
    /// 获取panelLayer，
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetPanelLayer(string name)
    {
        UIResInfoAsset resInfo = UIResInfoAssetAssetBundleReader.Instance.FindById(name);
        if (resInfo == null)
            return "";
        return resInfo.layerType.ToString();
    }

    /// <summary>
    /// 获取WndType，
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static UI3WndType GetWndType(string name)
    {
        UIResInfoAsset resInfo = UIResInfoAssetAssetBundleReader.Instance.FindById(name);
        if (resInfo == null)
            return UI3WndType.None;
        return (UI3WndType)resInfo.wndType;
    }

    /// <summary>
    /// 获取栈所在layer，
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetStackPanelLayerByWndName(string name,string panelLayer = "")
    {
        panelLayer = string.IsNullOrEmpty(panelLayer) == false ? panelLayer : GetPanelLayer(name);
        UI3WndPanelLayer wndPanelLayer = GetWndPanelLayer(panelLayer);
        if (wndPanelLayer == null)
            return null;
        string stackLayerType = wndPanelLayer.stackLayerType.ToString();
        return stackLayerType;
    }

    /// <summary>
    /// 获取栈所在layer，
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetStackPanelLayerByPanelLayer(string panelLayer)
    {
        UI3WndPanelLayer wndPanelLayer = GetWndPanelLayer(panelLayer);
        if (wndPanelLayer == null)
            return null;
        string stackLayerType = wndPanelLayer.stackLayerType.ToString();
        return stackLayerType;
    }

    /// <summary>
    /// 获取是否需要入栈，这个暂时先这样实现，可能以后需要弄成配置，通过读取配置，
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool GetIsPushStack(string name)
    {
        return name.Equals("UIGrayScreen") == false;
    }
    /// <summary>
    /// 获取对应栈对象，
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static UIStack<string> GetStackByWndName(string name,string panelLayer = "")
    {
        panelLayer = string.IsNullOrEmpty(panelLayer) == false ? panelLayer : GetPanelLayer(name);
        return GetStackByPanelLayer(panelLayer);
    }

    public static UIStack<string> GetStackByPanelLayer(UI3WndPanelLayerType layerType)
    {
        return GetStackByPanelLayer(layerType.ToString());
    }

    public static UIStack<string> GetStackByPanelLayer(string panelLayer)
    {
        string stackLayerType = GetStackPanelLayerByPanelLayer(panelLayer);
        if (m_wndStacks.ContainsKey(stackLayerType) == false)
            return null;
        return m_wndStacks[stackLayerType];
    }

    public static void HideStackWindows(string layerType, string name = null)
    {
        UIStack<string> stack = GetStackByPanelLayer(layerType);
        if(stack == null)
            return;
        for (int i = 0; i < stack.List.Count; i++)
        {
            string strVal = stack.List[i];
            if (string.IsNullOrEmpty(name) == false && name.Equals(strVal))
                continue;
            UI3Wnd curWnd = findWindow(strVal);
            if (curWnd != null && curWnd.isHideOnFullScreen == true)
            {
                curWnd.hide();
            }
        }
    }

    public static UI3WndPanelLayer GetWndPanelLayer(string layerType)
    {
        for (int i = 0;i < UI3WndPanelLayer.layers.Length;i++)
        {
            if(UI3WndPanelLayer.layers[i].type.ToString() == layerType)
            {
                return UI3WndPanelLayer.layers[i];
            }
        }
        return null;
    }
}
