using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;

public class ResourceAsset
{
	public string id = "";
	public string name = "";
	public string path = "";
	public int version = 0;
	public string hash = "";

}

public class ResourceAssetReader : IText<ResourceAsset>{

    private static string fileName = "Package/LocalConfig/resourceAsset.bytes";
    private static ResourceAssetReader instance = null;
    public Dictionary<string, ResourceAsset> dicts = new Dictionary<string, ResourceAsset>();
    public virtual void Init () {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(Application.dataPath + "/" + fileName);
		
        if(xmlDocument != null)
        {
            ReadListOfTFromXml(xmlDocument);
        }
	}

    public static ResourceAssetReader Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new ResourceAssetReader();
                instance.Init();
            }
            return instance;
        }
    }

    public Dictionary<string, ResourceAsset> ReadListOfTFromXml(XmlDocument xmlDocument) {
        XmlNodeList nodeList = xmlDocument.ChildNodes[0].ChildNodes;
        for(int i = 0;i < nodeList.Count;i++)
        {
            XmlNode node = nodeList[i];
            ResourceAsset resourceassetInstance = ReadTFromXml(node);
            if(resourceassetInstance != null)
            {
                dicts[resourceassetInstance.id] = resourceassetInstance;
            }
        }
        return dicts;
	}

    public ResourceAsset ReadTFromXml(XmlNode xmlNode)
    {
		ResourceAsset resourceassetInstance = new ResourceAsset();

		resourceassetInstance.id = TextUtils.XmlReadString(xmlNode, "id", "");
		resourceassetInstance.name = TextUtils.XmlReadString(xmlNode, "name", "");
		resourceassetInstance.path = TextUtils.XmlReadString(xmlNode, "path", "");
		resourceassetInstance.version = TextUtils.XmlReadInt(xmlNode, "version", 0);
		resourceassetInstance.hash = TextUtils.XmlReadString(xmlNode, "hash", "");
		return resourceassetInstance;

    }

    public ResourceAsset FindById(string id)
    {
        if(dicts.ContainsKey(id))
            return dicts[id];
        return null;
    }

    public ResourceAsset FindByKeyAndValue(string key,object obj)
    {
        FieldInfo fieldInfo = typeof(ResourceAsset).GetField(key);
        if(fieldInfo != null && dicts.Count > 0)
        {
            List<ResourceAsset> list = new List<ResourceAsset>(dicts.Values);
            for(int i = 0;i < list.Count;i++)
            {
                if (list[i] != null && fieldInfo.GetValue(list[i]).Equals(obj))
                    return list[i];
            }
        }

        return null;
    }


    public List<ResourceAsset> FindListByKeyAndValue(string key, object obj)
    {
        FieldInfo fieldInfo = typeof(ResourceAsset).GetField(key);
        if (fieldInfo != null && dicts.Count > 0)
        {
            List<ResourceAsset> list = new List<ResourceAsset>(dicts.Values);
            List<ResourceAsset> retList = new List<ResourceAsset>();
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

public class ResourceAssetAssetBundleReader : ResourceAssetReader{

    private static string fileName = "LocalConfig/resourceAsset";
    private static ResourceAssetAssetBundleReader instance = null;
    //public Dictionary<string, ResourceAsset> dicts = new Dictionary<string, ResourceAsset>();
    public override void Init () {
        XmlDocument xmlDocument = TextUtils.OpenXml(fileName);
        if(xmlDocument != null)
        {
            ReadListOfTFromXml(xmlDocument);
        }
    }

    public new static ResourceAssetAssetBundleReader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ResourceAssetAssetBundleReader();
                instance.Init();
            }
            return instance;
        }
    }
}