using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Состояние объекта
public class ObjectState
{
    //Список
    public Dictionary<string, StateValue> pars = new Dictionary<string, StateValue>();

    //События
    public delegate void UnitStateDelegate(string _key, StateValue _value, StateValue _fromValue);
    public event UnitStateDelegate onStateChange = delegate { };

    protected bool stopChange = false;

    //Конструктор
    public ObjectState ()
    {
        pars = new Dictionary<string, StateValue>();
    }

    //Прочитать состояние
    public StateValue Get(string _key)
    {
        if (pars != null)
            if (pars.ContainsKey(_key))
                return pars[_key];
        return new StateValue("");
    }

    public void StopChange(){
        stopChange = true;
    }
   
    //Установить состояне
    public void Set(string _key, StateValue param)
    {
        if (pars != null)
            if (!pars.ContainsKey(_key))
            {   
                onStateChange(_key, param, new StateValue("", 0f));

                //Перед изменением проверять stopChange для того чтобы обработчики могли остановить смену состояния
                if (!stopChange)  pars.Add(_key, param); else stopChange = false; 
            }
            else
            {
                StateValue last = pars[_key];

                if (last != param)
                    onStateChange(_key, param, last);

                if (!stopChange)  pars[_key] = param; else stopChange = false;
            }
    }


    //Установить состояне
    public void Set(string _key, string param) {
        Set(_key, new StateValue(param));
    }


    //Установить состояне
    public void Set(string _key, float param)
    {
        Set(_key, new StateValue(param));
    }



    //наличие состояния
    public bool Has(string _key)
    {
        if (pars == null) return false;
        return pars.ContainsKey(_key);
    }

    //удалить состояние
    public void Remove(string _key)
    {
        if (pars == null) return;
        if (!Has(_key)) return;
        StateValue _fromValue = Get(_key);

        onStateChange(_key, new StateValue("REMOVE"), _fromValue);

        if (!stopChange) pars.Remove(_key); else stopChange = false;
    }


    public float GetFloat(string _key)
    {
        StateValue value = Get(_key);
        return value.Value;
    }

    public string GetStr(string _key)
    {
        StateValue value = Get(_key);
        return value.Str;
    }

    public dynamic this[string _key]
    {
        get
        {
            StateValue value = Get(_key);
                return value.Value;
        }
        set
        {
            if (value is StateValue)
                Set(_key, (StateValue)value);
            if (value is float)
                Set(_key, (float)value);
            if (value is string)
                Set(_key, (string)value);
        }
    }

}

[System.Serializable]
//Параметр состояния
public struct StateValue
{
    public float Value; 
    string strValue;


    public StateValue(string value)
    {
        Value = 0f;
        strValue = value;
    }

    public StateValue(float value)
    {
        Value = value;
        strValue = "";
    }

    public StateValue(string str, float value)
    {
        Value = value;
        strValue = str;
    }

    public static bool operator ==(StateValue a, StateValue b)
    {
        return ((a.Value == b.Value) && (a.Str == b.Str));
    }


    public static bool operator !=(StateValue a, StateValue b)
    {
        return ((a.Value != b.Value) || (a.Str != b.Str));
    }


    public string Str
    {  get { if (strValue == null) return ""; return strValue; }}
}