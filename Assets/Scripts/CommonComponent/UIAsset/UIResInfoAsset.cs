using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;

public class UIResInfoAsset
{
	public string id = "";
	public string name = "";
	public string path = "";
	public int wndType = 0;
	public string layerType = "";

}

public class UIResInfoAssetReader : IText<UIResInfoAsset>{

    private static string fileName = "Package/LocalConfig/uIResInfoAsset.bytes";
    private static UIResInfoAssetReader instance = null;
    public static Dictionary<string, UIResInfoAsset> dicts = new Dictionary<string, UIResInfoAsset>();
    public virtual void Init () {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(Application.dataPath + "/" + fileName);
		
        if(xmlDocument != null)
        {
            ReadListOfTFromXml(xmlDocument);
        }
	}

    public static UIResInfoAssetReader Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new UIResInfoAssetReader();
                instance.Init();
            }
            return instance;
        }
    }

    public Dictionary<string, UIResInfoAsset> ReadListOfTFromXml(XmlDocument xmlDocument) {
        XmlNodeList nodeList = xmlDocument.ChildNodes[0].ChildNodes;
        for(int i = 0;i < nodeList.Count;i++)
        {
            XmlNode node = nodeList[i];
            UIResInfoAsset uiresinfoassetInstance = ReadTFromXml(node);
            if(uiresinfoassetInstance != null)
            {
                dicts[uiresinfoassetInstance.id] = uiresinfoassetInstance;
            }
        }
        return dicts;
	}

    public UIResInfoAsset ReadTFromXml(XmlNode xmlNode)
    {
		UIResInfoAsset uiresinfoassetInstance = new UIResInfoAsset();

		uiresinfoassetInstance.id = TextUtils.XmlReadString(xmlNode, "id", "");
		uiresinfoassetInstance.name = TextUtils.XmlReadString(xmlNode, "name", "");
		uiresinfoassetInstance.path = TextUtils.XmlReadString(xmlNode, "path", "");
		uiresinfoassetInstance.wndType = TextUtils.XmlReadInt(xmlNode, "wndType", 0);
		uiresinfoassetInstance.layerType = TextUtils.XmlReadString(xmlNode, "layerType", "");
		return uiresinfoassetInstance;

    }

    public UIResInfoAsset FindById(string id)
    {
        if(dicts.ContainsKey(id))
            return dicts[id];
        return null;
    }

    public UIResInfoAsset FindByKeyAndValue(string key,object obj)
    {
        FieldInfo fieldInfo = typeof(UIResInfoAsset).GetField(key);
        if(fieldInfo != null && dicts.Count > 0)
        {
            List<UIResInfoAsset> list = new List<UIResInfoAsset>(dicts.Values);
            for(int i = 0;i < list.Count;i++)
            {
                if (list[i] != null && fieldInfo.GetValue(list[i]).Equals(obj))
                    return list[i];
            }
        }

        return null;
    }


    public List<UIResInfoAsset> FindListByKeyAndValue(string key, object obj)
    {
        FieldInfo fieldInfo = typeof(UIResInfoAsset).GetField(key);
        if (fieldInfo != null && dicts.Count > 0)
        {
            List<UIResInfoAsset> list = new List<UIResInfoAsset>(dicts.Values);
            List<UIResInfoAsset> retList = new List<UIResInfoAsset>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null && fieldInfo.GetValue(list[i]).Equals(obj))
                    retList.Add(list[i]);
            }
            return retList;
        }

        return null;
    }
}

public class UIResInfoAssetAssetBundleReader : UIResInfoAssetReader{

    private static string fileName = "LocalConfig/uIResInfoAsset";
    private static UIResInfoAssetAssetBundleReader instance = null;
    public override void Init () {
        XmlDocument xmlDocument = TextUtils.OpenXml(fileName);
        if(xmlDocument != null)
        {
            ReadListOfTFromXml(xmlDocument);
        }
    }

    public new static UIResInfoAssetAssetBundleReader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UIResInfoAssetAssetBundleReader();
                instance.Init();
            }
            return instance;
        }
    }
}