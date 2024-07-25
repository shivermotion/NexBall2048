using System.Collections.Generic;
using UnityEngine;

namespace PolyTypes
{
    [CreateAssetMenu(menuName = "Poly/Color Data", fileName = "PolyColorDataSO")]
    public class PolyDataSO : ScriptableObject
    {
        public PolyData data = new PolyData(20);

        public Color GetColor(int index)
        {
            if (index >= data.values.Count || index < 0)
            {
                Debug.LogWarning($"Index value outside of range! Index: {index} Of: {data.values.Count}", this);
                return Color.black;
            }

            return data.values[index].color;
        }

        public float GetSize(int index) => basePolyhedronWorldSize + (index * .1f);
        
        public int GetValue(int index)
        {
            if (index >= data.values.Count || index < 0)
            {
                Debug.LogWarning($"Index value outside of range! Index: {index} Of: {data.values.Count}", this);
                return 2;
            }

            return data.values[index].size;
        }
        
        [Header("Variables")]
        public float basePolyhedronWorldSize = 0.8f; // Base size of the poly in the world
        public float shootForce = 30f;
        public float horizontalSpeed = 20f; // Adjustable horizontal speed
        public float instantiationDelay = 0.2f; // Delay before instantiating the next polyhedron
        public float planeHeight = 10f; // Height of the plane
        public float popUpVelocity = 2f; // Velocity for the new polyhedron to pop up
        public float wiggleDuration = 0.5f; // Duration of the wiggle effect
        public float wiggleMagnitude = 0.1f; // Magnitude of the wiggle effect
        public float spinTorque = 5f; // Torque for the spin effect
        public float gravityScale = 2f; // Gravity scale to make objects fall faster
        public float polyhedronMass = 0.5f; // Mass of the polyhedrons
        public float polyhedronDrag = 0.1f; // Drag of the polyhedrons
        public float polyhedronAngularDrag = 0.05f; // Angular drag of the polyhedrons
        public float polyhedronBounciness = 0.9f; // Bounciness of the polyhedrons
        public float explosionForce = 5f; // Force of the explosion applied to new polyhedrons
    }
}