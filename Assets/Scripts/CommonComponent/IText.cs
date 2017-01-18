using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public interface IText<T>
{
    Dictionary<string,T> ReadListOfTFromXml(XmlDocument xmlDocument);
    T ReadTFromXml(XmlNode xmlNode);
}
