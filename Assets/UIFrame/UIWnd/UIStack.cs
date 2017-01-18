using UnityEngine;
using System.Collections.Generic;
using System.Xml;

public class UIStack<T>
{
    //保存当前ui列表，
    private List<T> _list = new List<T>();
    private int _index = -1;
    public string strId = "";

    public UIStack(string argId)
    {
        this.strId = argId;
    }

    public bool PushBack(T wnd)
    {
        if(_list != null && wnd != null)
        {
            _list.Add(wnd);
            return true;
        }
        return false;
    }

    public T PopBack()
    {
        if(_list != null)
        {
            T wnd = _list[_list.Count - 1];
            _list.RemoveAt(_list.Count - 1);
            return wnd;
        }
        return default(T);
    }

    public int FindLastByName(string name)
    {
        if(_list != null && string.IsNullOrEmpty(name) == false)
        {
            for(int i = _list.Count - 1; i >= 0;i--)
            {
                T wnd = _list[i];
                if(wnd != null && wnd.ToString() == name)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public int FindFirstByName(string name)
    {
        if (_list != null && string.IsNullOrEmpty(name) == false)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                T wnd = _list[i];
                if (wnd != null && wnd.ToString() == name)
                {
                    return i;
                }
            }
        }
        return -1;
    }
    //
    public T PopAt(int index)
    {
        T wnd = default(T);
        if (index < _list.Count && index >= 0)
        {
            wnd = _list[index];
            _list.RemoveAt(index);
        }
        return wnd;
    }

    //
    public T PopAt(string name)
    {
        int index = FindLastByName(name);
        return PopAt(index);
    }

    //
    public bool BringTop(string name)
    {
        //if (_list != null && string.IsNullOrEmpty(name) == false)
        //{
        //    int index = FindLastByName(name);
        //    if (index >= 0 && index < _list.Count - 1)
        //    {
        //        T wnd = PopAt(index);
        //        PushBack(wnd);
        //        return true;
        //    }
        //}
        return false;
    }

    //
    public T Peek()
    {
        if(_list != null && _list.Count > 0 && _list[_list.Count - 1] != null)
        {
            return _list[_list.Count - 1];
        }
        return default(T);
    }

    public T Peek(int index)
    {
        if (_list != null && _list.Count > index && index >= 0)
        {
            return _list[index];
        }
        return default(T);
    }

    public bool IsTop(string name)
    {
        return _list != null && _list.Count > 0 && _list[_list.Count - 1].ToString() == name;
    }

    public bool Have(T t)
    {
        return _list.Contains(t);
    }

    public void DisplayList()
    {
        string output = "";
        for(int i = _list.Count - 1; i >= 0;i--)
        {
            T wnd = _list[i];
            if(wnd != null)
            {
                output += wnd.ToString() + ">";
            }else
            {
                output += "UI3Wnd[" + i + "] null";
            }
        }
        Debug.Log("DisplayList " + strId + " " + output);
    }

    public List<T> List
    {
        get
        {
            return _list;
        }
    }

    public void Clear()
    {
        _list.Clear();
    }

    public void SetTop(T t)
    {
        for(int i = _list.Count - 1;i >= 0;i--)
        {
            if(_list[i] != null && _list[i].Equals(t))
                break;
            _list.RemoveAt(i);
        }
    }

    public T FindDifferenct(int index,T t,out int curIndex,bool top = true)
    {
        curIndex = -1;
        if (t == null)
            return default(T);
        if(top == true)
        {
            for (int i = index; i < _list.Count; i++)
            {
                T curT = _list[i];
                if (t.Equals(curT) == false)
                {
                    curIndex = i;
                    return curT;
                }
            }
        }else
        {
            for (int i = index; i >= 0; i--)
            {
                T curT = _list[i];
                if (t.Equals(curT) == false)
                {
                    curIndex = i;
                    return curT;
                }
            }
        }

        return default(T);
    }

    public void SetAt(int index,T t)
    {
        if (_list != null && _list.Count > index && index >= 0)
        {
            _list[index] = t;
        }
    }
}
