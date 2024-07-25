using System.Collections.Generic;
using System.Linq;
using Property_Attributes;
using UnityEngine;

namespace PolyTypes
{

    [System.Serializable]
    public class PolyData
    {
        [SerializeField] private List<PolyType> values = new();

        public int count => values.Count;
        
        public PolyData(int indexCount = 20)
        {
            for (int i = 0; i < indexCount; i++)
            {
                var poly = new PolyType(i+1);
                values.Add(poly);
            }
        }

        public PolyData() => new PolyData(20);
    }

    [System.Serializable]
    public class PolyType
    {
        [SerializeField, HideInInspector] private int _power;
        [SerializeField, ReadOnly] private int _size;
        public Color color;

        public PolyType(int power)
        {
            _power = power;
            _size = (int)Mathf.Pow(2,power);
        }

        public int power => _power;
        public int size => _size;
    }
}