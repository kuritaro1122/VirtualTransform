using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using VirtualTransform;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(VTransform))]
internal class VTransformDrawer : PropertyDrawer {
    private class PropertyData {
        //public SerializedProperty useTransform;
        public SerializedProperty optionType;
        public SerializedProperty axisTransform;
        public SerializedProperty axisPosition;
        public SerializedProperty axisRotation;
    }

    private Dictionary<string, PropertyData> _propertyDataPerPropertyPath = new Dictionary<string, PropertyData>();
    private PropertyData _property;


    private float LineHeight { get { return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; } }

    private void Init(SerializedProperty property) {
        if (_propertyDataPerPropertyPath.TryGetValue(property.propertyPath, out _property)) {
            return;
        }

        _property = new PropertyData();
        //_property.useTransform = property.FindPropertyRelative("useTransform");
        _property.optionType = property.FindPropertyRelative("optionType");
        _property.axisTransform = property.FindPropertyRelative("axisTransform");
        _property.axisPosition = property.FindPropertyRelative("axisPosition");
        _property.axisRotation = property.FindPropertyRelative("axisRotation");
        _propertyDataPerPropertyPath.Add(property.propertyPath, _property);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        Init(property);
        Rect fieldRect = position;
        fieldRect.height = LineHeight;

        using (new EditorGUI.PropertyScope(fieldRect, label, property)) {
            property.isExpanded = EditorGUI.Foldout(new Rect(fieldRect), property.isExpanded, label);

            EditorGUI.indentLevel++;
            if (property.isExpanded) ElementsGUI(ref fieldRect);
            EditorGUI.indentLevel--;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        Init(property);
        // (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) x 行数 で描画領域の高さを求める
        int line = 1;
        if (property.isExpanded)
            switch (_property.optionType.enumValueIndex) {
                case 0:
                    line += 2;
                    break;
                case 1:
                    line += 3;
                    break;
                case 2:
                    line += 2;
                    break;
            }
        return line * LineHeight;
    }

    private void PropertyFieldGUI(ref Rect rect, SerializedProperty serializedProperty, bool includeChildren = false) {
        rect.y += LineHeight;
        EditorGUI.PropertyField(new Rect(rect), serializedProperty, includeChildren);
        if (!includeChildren || !serializedProperty.isExpanded) return;
        if (serializedProperty.isArray) rect.y += serializedProperty.arraySize + 1;
    }
    /*private void AddLineHeight(int add = 1) {
        fieldRect.y += add * LineHeight;
    }*/

    private void ElementsGUI(ref Rect rect) {
        PropertyFieldGUI(ref rect, _property.optionType);
        switch (_property.optionType.enumValueIndex) {
            case 0:
                PropertyFieldGUI(ref rect, _property.axisTransform);
                break;
            case 1:
                PropertyFieldGUI(ref rect, _property.axisPosition);
                PropertyFieldGUI(ref rect, _property.axisRotation);
                break;
            case 2:
            default:
                rect.y += LineHeight;
                EditorGUI.HelpBox(new Rect(rect), "Can't edit from inspector.", MessageType.Info);
                break;
        }
    }
}
#endif