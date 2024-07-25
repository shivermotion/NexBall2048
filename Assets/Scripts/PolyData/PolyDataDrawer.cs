#if UNITY_EDITOR

using Extension_Methods;
using UnityEditor;
using UnityEngine;


namespace PolyTypes
{
    [CustomPropertyDrawer(typeof(PolyData))]
    public class PolyDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.boxedValue is not PolyData polyData)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            
            return perItemHeight * (polyData.values.Count + 1);
        }

        private float perItemHeight => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var prop = property.FindPropertyRelative("values");

            Rect line = position.FirstLine();
            
            EditorGUI.LabelField(line, "Colors");

            line = line.NextVertical();
            
            for (int i = 0; i < prop.arraySize; i++)
            {
                var indexProp = prop.GetArrayElementAtIndex(i);

                if (indexProp.boxedValue is not PolyType polyType) continue;
                
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();

                Rect l = line.XPixels(80);
                Rect r = line.XPixelsRemainder(80);
                
                EditorGUI.LabelField(l, new GUIContent($"{i+1} ({Mathf.Pow(2,i+1)})"));
                polyType.color = EditorGUI.ColorField(r, polyType.color);
                
                if (EditorGUI.EndChangeCheck())
                {
                    indexProp.boxedValue = polyType;
                }
                EditorGUILayout.EndHorizontal();

                line = line.NextVertical();
            }
        }
    }
}

#endif