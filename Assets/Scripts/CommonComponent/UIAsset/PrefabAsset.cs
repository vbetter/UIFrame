using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;

public class PrefabAsset
{
	public string id = "";
	public string name = "";
	public string bundleName = "";

}

public class PrefabAssetReader : IText<PrefabAsset>{

    private static string fileName = "Package/LocalConfig/prefabAsset.bytes";
    private static PrefabAssetReader instance = null;
    public Dictionary<string, PrefabAsset> dicts = new Dictionary<string, PrefabAsset>();
    public virtual void Init () {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(Application.dataPath + "/" + fileName);
		
        if(xmlDocument != null)
        {
            ReadListOfTFromXml(xmlDocument);
        }
	}

    public static PrefabAssetReader Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new PrefabAssetReader();
                instance.Init();
            }
            return instance;
        }
    }

    public Dictionary<string, PrefabAsset> ReadListOfTFromXml(XmlDocument xmlDocument) {
        XmlNodeList nodeList = xmlDocument.ChildNodes[0].ChildNodes;
        for(int i = 0;i < nodeList.Count;i++)
        {
            XmlNode node = nodeList[i];
            PrefabAsset prefabassetInstance = ReadTFromXml(node);
            if(prefabassetInstance != null)
            {
                dicts[prefabassetInstance.id] = prefabassetInstance;
            }
        }
        return dicts;
	}

    public PrefabAsset ReadTFromXml(XmlNode xmlNode)
    {
		PrefabAsset prefabassetInstance = new PrefabAsset();

		prefabassetInstance.id = TextUtils.XmlReadString(xmlNode, "id", "");
		prefabassetInstance.name = TextUtils.XmlReadString(xmlNode, "name", "");
		prefabassetInstance.bundleName = TextUtils.XmlReadString(xmlNode, "bundleName", "");
		return prefabassetInstance;

    }

    public PrefabAsset FindById(string id)
    {
        if(dicts.ContainsKey(id))
            return dicts[id];
        return null;
    }

    public PrefabAsset FindByKeyAndValue(string key,object obj)
    {
        FieldInfo fieldInfo = typeof(PrefabAsset).GetField(key);
        if(fieldInfo != null && dicts.Count > 0)
        {
            List<PrefabAsset> list = new List<PrefabAsset>(dicts.Values);
            for(int i = 0;i < list.Count;i++)
            {
                if (list[i] != null && fieldInfo.GetValue(list[i]).Equals(obj))
                    return list[i];
            }
        }

        return null;
    }


    public List<PrefabAsset> FindListByKeyAndValue(string key, object obj)
    {
        FieldInfo fieldInfo = typeof(PrefabAsset).GetField(key);
        if (fieldInfo != null && dicts.Count > 0)
        {
            List<PrefabAsset> list = new List<PrefabAsset>(dicts.Values);
            List<PrefabAsset> retList = new List<PrefabAsset>();
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

public class PrefabAssetAssetBundleReader : PrefabAssetReader{

    private static string fileName = "LocalConfig/prefabAsset";
    private static PrefabAssetAssetBundleReader instance = null;
    //public Dictionary<string, PrefabAsset> dicts = new Dictionary<string, PrefabAsset>();
    public override void Init () {
        XmlDocument xmlDocument = TextUtils.OpenXml(fileName);
        if(xmlDocument != null)
        {
            ReadListOfTFromXml(xmlDocument);
        }
    }

    public new static PrefabAssetAssetBundleReader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PrefabAssetAssetBundleReader();
                instance.Init();
            }
            return instance;
        }
    }
}