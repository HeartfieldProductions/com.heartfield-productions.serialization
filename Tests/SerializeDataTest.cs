using System;
using UnityEngine;
using System.Collections.Generic;

namespace Heartfield.Serialization.Tests
{
    public class SerializeDataTest : MonoBehaviour, ISaveable
    {
        [SerializeField] internal float a = float.MaxValue;
        [SerializeField] internal int b = int.MinValue;
        [SerializeField] internal bool c = true;
        [SerializeField] internal string d = "asdfghjk";
        [SerializeField] internal List<int> e = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        Dictionary<int, Temp> f = new Dictionary<int, Temp>() { 
            { 1, new Temp() { g = 7, test = "a" } },
            { 2, new Temp() { g = 3, test = "b" } }, 
            { 3, new Temp() { g = 10, test = "c" } } };

        [SerializeField] int id;

        [Serializable]
        struct Temp
        {
            [SerializeField] internal int g;
            [SerializeField] internal string test;
        }

        [SerializeField] Temp tmp = new Temp();

        void OnValidate()
        {
            this.Register(id);
        }

        void ISaveable.OnPopulateSave()
        {
            this.AddData(a);
            this.AddData(b);
            this.AddData(c);
            this.AddData(d);
            this.AddData(e);
            this.AddData(f);
            this.AddData(tmp);
        }

        void ISaveable.OnLoadFromSave()
        {
            this.GetData(ref a);
            this.GetData(ref b);
            this.GetData(ref c);
            this.GetData(ref d);
            this.GetData(ref e);
            this.GetData(ref f);
            this.GetData(ref tmp);
        }
    }
}