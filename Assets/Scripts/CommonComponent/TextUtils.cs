using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using LitJson;

public class TextUtils
{
    ///* 函数说明：读取一个tdr配置文件 */
    //public static T[] ReadTdrFile<T>(string strPath) where T : wl_res.tsf4g_csharp_interface, new()
    //{
    //    //TextAsset textAsset = Resources.Load(strPath) as TextAsset;
    //    TextAsset textAsset = ResManager.Instance().GetConfigText(strPath);
    //    if (textAsset == null)
    //    {
    //        Debuger.LogError("Bin File Read Failed! Path:" + strPath);
    //        return null;
    //    }

    //    byte[] rawBytes = textAsset.bytes;
    //    tsf4g_tdr_csharp.TdrReadBuf tdrBuff = new tsf4g_tdr_csharp.TdrReadBuf(ref rawBytes, rawBytes.Length);
    //    tdrBuff.disableEndian();

    //    //Parse Head
    //    tsf4g_tdr_csharp.TResHeadAll resHead = new tsf4g_tdr_csharp.TResHeadAll();
    //    resHead.load(ref tdrBuff);

    //    int count = resHead.mHead.iCount;
    //    T[] result = new T[count];

    //    for (int i = 0; i < count; i++)
    //    {
    //        T t = new T();
    //        t.load(ref tdrBuff, 0);
    //        result[i] = t;
    //    }

    //    Resources.UnloadAsset(textAsset);
    //    textAsset = null;
    //    return result;
    //}

    /* 函数说明：打开一个xml配置文件*/
    public static XmlDocument OpenXml(string fileName)
    {
        /*
        byte[] bytes = ResManager.Instance.LoadConfig(fileName);
        if (bytes == null || bytes.Length <= 0)
        {
            Debuger.LogError("can't load the xml file '" + fileName + "'");
            return null;
        }
        MemoryStream memStream = new MemoryStream();
        memStream.Write(bytes, 0, bytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(memStream);

        memStream.Close();
        memStream = null;
        return xmlDoc;
         */
        return null;
    }

    /* 函数说明：把BYTE数组转换成XML配置文件*/
    public static XmlDocument ByteConvertXml(byte[] bytes)
    {
        MemoryStream memStream = new MemoryStream();
        memStream.Write(bytes, 0, bytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(memStream);

        memStream.Close();
        memStream = null;
        return xmlDoc;
    }

    /* 函数说明：读取xml的int子节点 */
    public static int XmlReadInt(XmlNode node, string key, int def)
    {
        int result;
        try { result = int.Parse(node.Attributes[key].Value); }
        catch (Exception) { result = def; }
        return result;
    }

    /* 函数说明：读取xml的float子节点 */
    public static float XmlReadFloat(XmlNode node, string key, float def)
    {
        float result;
        try { result = float.Parse(node.Attributes[key].Value); }
        catch (Exception) { result = def; }
        return result;
    }

    /* 函数说明：读取xml的string子节点 */
    public static string XmlReadString(XmlNode node, string key, string def)
    {
        string result = "";
        try { result = node.Attributes[key].Value; }
        catch (Exception) { result = def; }
        return result;
    }

    /* 函数说明：字符串转int */
    public static int Str2Int(string str, int def = 0)
    {
        int result;
        try { result = int.Parse(str); }
        catch (Exception) { result = def; }
        return result;
    }

    /* 函数说明：字符串转int */
    public static uint Str2Uint(string str, uint def = 0)
    {
        uint result;
        try { result = uint.Parse(str); }
        catch (Exception) { result = def; }
        return result;
    }

    /* 函数说明：字符串转float */
    public static float Str2Float(string str, float def = 0.0f)
    {
        float result;
        try { result = float.Parse(str); }
        catch (Exception) { result = def; }
        return result;
    }

    /* 函数说明：字符串转int列表 */
    public static List<int> Str2IntList(string str, char sp)
    {
        List<int> result = new List<int>();
        string[] strArray = str.Split(new char[] { sp });
        if (strArray == null || strArray.Length == 0)
        {
            return result;
        }
        for (int i = 0; i < strArray.Length; i++)
        {
            result.Add(Str2Int(strArray[i]));
        }
        return result;
    }

    /* 函数说明：字符串转uint列表 */
    public static List<uint> Str2UintList(string str, char sp)
    {
        List<uint> result = new List<uint>();
        string[] strArray = str.Split(new char[] { sp });
        if (strArray == null || strArray.Length == 0 || strArray[0] == "")
        {
            return result;
        }
        for (int i = 0; i < strArray.Length; i++)
        {
            result.Add(Str2Uint(strArray[i]));
        }
        return result;
    }

    /* 函数说明：字符串转float列表 */
    public static List<float> Str2FloatList(string str, char sp)
    {
        List<float> result = new List<float>();
        string[] strArray = str.Split(new char[] { sp });
        if (strArray == null || strArray.Length == 0)
        {
            return result;
        }
        for (int i = 0; i < strArray.Length; i++)
        {
            result.Add(Str2Float(strArray[i]));
        }
        return result;
    }

    /* 函数说明：bytes转字符串 */
    public static string Bytes2String(ref byte[] bytes)
    {
        if (bytes == null)
            return "";

        string str = UTF8BytesToString(ref bytes);
        return str;
    }

    /// <summary>
    /// 将一个UTF8编码格式的byte数组，转换为一个String
    /// </summary>
    /// <param name="str">编码格式的数组</param>
    /// <returns>转换的string</returns>
    static public string UTF8BytesToString(ref byte[] str)
    {
        if (str == null)
            return "";

        //为了让string的长度正确，
        byte[] tempStr = new byte[strlen(str)];
        System.Buffer.BlockCopy(str, 0, tempStr, 0, tempStr.Length);
        return System.Text.Encoding.UTF8.GetString(tempStr);
    }

    /// <summary>
    /// 功能同c语言中的strlen
    /// </summary>
    /// <param name="str">输入的bytes</param>
    /// <returns>输入bytes的string长度</returns>
    static public int strlen(byte[] str)
    {
        if (str == null)
            return 0;

        byte nullChar = 0x00;
        int count = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (nullChar == str[i])
            {
                break;
            }

            count++;
        }

        return count;
    }

    /* 函数说明：bytes转int */
    public static int Bytes2Int(ref byte[] bytes)
    {
        string str = Bytes2String(ref bytes);
        return Str2Int(str, 0);
    }

    /* 函数说明：bytes转uint */
    public static uint Bytes2Uint(ref byte[] bytes)
    {
        string str = Bytes2String(ref bytes);
        return Str2Uint(str, 0);
    }

    /* 函数说明：bytes转float */
    public static float Bytes2Float(ref byte[] bytes)
    {
        string str = Bytes2String(ref bytes);
        return Str2Float(str, 0);
    }

    /* 函数说明：bytes转int列表 */
    public static List<int> Bytes2IntList(ref byte[] bytes, char sp)
    {
        string str = Bytes2String(ref bytes);
        return Str2IntList(str, sp);
    }

    /* 函数说明：bytes转uint列表 */
    public static List<uint> Bytes2UintList(ref byte[] bytes, char sp)
    {
        string str = Bytes2String(ref bytes);
        return Str2UintList(str, sp);
    }

    /* 函数说明：bytes转float列表 */
    public static List<float> Bytes2FloatList(ref byte[] bytes, char sp)
    {
        string str = Bytes2String(ref bytes);
        return Str2FloatList(str, sp);
    }

    /* 函数说明：读取json的int字段 */
    public static bool JsonReadBool(JsonData jsonData, string key, bool def = false)
    {
        return JsonDataToBool(GetJsonData(jsonData, key));
    }

    public static int JsonReadInt(JsonData jsonData, string key, int def = 0)
    {
        return JsonDataToInt(GetJsonData(jsonData, key));
    }

    /* 函数说明：读取json的uint字段 */
    public static uint JsonReadUint(JsonData jsonData, string key, uint def = 0)
    {
        return JsonDataToUint(GetJsonData(jsonData, key));
    }

    /* 函数说明：读取json的ulong字段 */
    public static ulong JsonReadUlong(JsonData jsonData, string key, ulong def = 0)
    {
        return JsonDataToUlong(GetJsonData(jsonData, key));
    }

    /* 函数说明：读取json的float字段 */
    public static long JsonReadLong(JsonData jsonData, string key, long def = 0)
    {
        return JsonDataToLong(GetJsonData(jsonData, key));
    }

    /* 函数说明：读取json的float字段 */
    public static float JsonReadFloat(JsonData jsonData, string key, float def = 0)
    {
        return JsonDataToFloat(GetJsonData(jsonData, key));
    }

    /* 函数说明：读取json的string字段 */
    public static string JsonReadString(JsonData jsonData, string key, string def = "")
    {
        return JsonDataToString(GetJsonData(jsonData, key));
    }

    /* 函数说明：读取json的string转换的long字段 */
    public static long JsonReadLongFromString(JsonData jsonData, string key, string def = "")
    {
        string curStr = JsonDataToString(GetJsonData(jsonData, key));
        long curLong = 0;
        if (long.TryParse(curStr, out curLong))
        {
            return curLong;
        }
        return -1;
    }

    //=============================================
    //转换jsondata到指定数据类型
    public static JsonData GetJsonData(JsonData data, string key)
    {
        if (IsEmptyJsonData(data))
        {
            return null;
        }
        JsonData result = null;
        if (data.Contains(key))
            result = data[key];
        return result;
    }

    private static bool JsonDataToBool(JsonData data)
    {
        if (IsEmptyJsonData(data) || !data.IsBoolean)
        {
            return false;
        }
        return (bool)data;
    }

    private static int JsonDataToInt(JsonData data)
    {
        if (IsEmptyJsonData(data) || !data.IsInt)
        {
            return 0;
        }
        return (int)data;
    }

    private static uint JsonDataToUint(JsonData data)
    {
        if (IsEmptyJsonData(data))
        {
            return 0;
        }
        if (data.IsInt)
        {
            int tmp = (int)data;
            return (uint)tmp;
        }
        else if (data.IsLong)
        {
            long tmp = (long)data;
            return (uint)tmp;
        }
        return 0;
    }
    private static long JsonDataToLong(JsonData data)
    {
        if (IsEmptyJsonData(data))
        {
            return 0;
        }
        if (data.IsInt)
        {
            int tmp = (int)data;
            return (long)tmp;
        }
        else if (data.IsLong)
        {
            long tmp = (long)data;
            return (long)tmp;
        }
        return 0;
    }
    private static ulong JsonDataToUlong(JsonData data)
    {
        if (IsEmptyJsonData(data))
        {
            return 0;
        }
        if (data.IsInt)
        {
            int tmp = (int)data;
            return (ulong)tmp;
        }
        else if (data.IsLong)
        {
            long tmp = (long)data;
            return (ulong)tmp;
        }
        else if (data.IsDouble)
        {
            double tmp = (double)data;
            return Convert.ToUInt64(tmp);
        }
        return 0;
    }
    private static float JsonDataToFloat(JsonData data)
    {
        if (IsEmptyJsonData(data) || !data.IsDouble)
        {
            return 0.0f;
        }

        double tmp = (double)data;
        return (float)tmp;
    }

    private static string JsonDataToString(JsonData data)
    {
        if (IsEmptyJsonData(data))
        {
            return "";
        }
        return (string)data;
    }

    public static bool IsEmptyJsonData(JsonData data)
    {
        if (data == null)
        {
            return true;
        }

        if (data.IsString)
        {
            string str = (string)data;
            return str == "";
        }
        return false;
    }
    //=============================================

    /* 将“秒”格式化成字符串 */
    public static string Second2String(int totalSecond)
    {
        totalSecond = Mathf.Max(0, totalSecond);
        int hh = totalSecond / 3600;
        int mm = totalSecond / 60 % 60;
        int ss = totalSecond % 60;
        string timeFromat = mm.ToString().PadLeft(2, '0') + ":" + ss.ToString().PadLeft(2, '0');
        return timeFromat;
    }

    // Byte转16进制字符串
    static char[] hexChars = "0123456789ABCDEF".ToCharArray();
    public static String Bytes2HexStr(byte[] bs, int index, int length)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder("");
        sb.Append("[");

        int bit;
        for (int i = index; i < index + length; i++)
        {
            bit = (bs[i] & 0x0f0) >> 4;
            sb.Append(hexChars[bit]);
            bit = bs[i] & 0x0f;
            sb.Append(hexChars[bit]);
            sb.Append(" ");
        }

        sb.Append("]");
        return sb.ToString();
    }

    // String转16进制字符串
    public static String Str2HexStr(string str, int index, int length)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder("");
        sb.Append("[");

        byte[] bs = System.Text.Encoding.UTF8.GetBytes(str);
        int bit;
        for (int i = index; i < index + length; i++)
        {
            bit = (bs[i] & 0x0f0) >> 4;
            sb.Append(hexChars[bit]);
            bit = bs[i] & 0x0f;
            sb.Append(hexChars[bit]);
            sb.Append(" ");
        }

        sb.Append("]");
        return sb.ToString();
    }

    // 创建一个T对象，
    public static T ReadTFromXml<T>(XmlDocument xmlDocument) where T : IText<T>,new()
    {
        T t = new T();
        return t.ReadTFromXml(xmlDocument);
    }



    public static Vector3 Str2Vector3(ref byte[] bytes, char sp = ' ')
    {
        string str = Bytes2String(ref bytes);

        return Str2Vector3(str, sp);
    }

    public static Vector3 Str2Vector3(string str, char sp = ' ')
    {
        Vector3 tempVec3 = Vector3.zero;
        string[] strArr = str.Split(new System.Char[] { sp });

        bool isError = false;
        if (strArr != null && strArr.Length == 3)
        {
            float value1 = 0, value2 = 0, value3 = 0;
            if (!isError && float.TryParse(strArr[0], out value1))
            {
                tempVec3.x = value1;
            }
            else { isError = true; }

            if (!isError && float.TryParse(strArr[1], out value2))
            {
                tempVec3.y = value2;
            }
            else { isError = true; }

            if (!isError && float.TryParse(strArr[2], out value3))
            {
                tempVec3.z = value3;
            }
            else { isError = true; }
        }
        if (isError) Debug.LogError(" Fail! Str2Vector3:" + str);
        return tempVec3;
    }

}
