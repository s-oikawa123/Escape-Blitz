using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

// LaunguageStringの拡張プロパティインスペクター
[CustomPropertyDrawer(typeof(LanguageString))]
public class LanguageStringEditor : PropertyDrawer
{
    private float height = 18f;
    private float space = 2f;
    private int expandValue = 3;
    private int fieldLength;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);
        contentPosition.height = height;

        var expandFlag = property.FindPropertyRelative("expandFlag");
        property.FindPropertyRelative("expandFlag").boolValue = EditorGUI.Toggle(contentPosition, "Expand Text Area" , expandFlag.boolValue);

        EditorGUI.indentLevel++;
        contentPosition = EditorGUI.IndentedRect(position); 
        EditorGUI.indentLevel = 0;
        contentPosition.height = height;

        TypeInfo typeInfo = typeof(LanguageString).GetTypeInfo();
        FieldInfo[] fieldInfos = typeInfo.GetFields();
        fieldLength = fieldInfos.Length - 1;
        contentPosition.y += height + space;

        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            if (fieldInfo.FieldType != typeof(string)) 
            {
                continue;
            }
            var item = property.FindPropertyRelative(fieldInfo.Name);
            if (expandFlag.boolValue)
            {
                EditorGUI.LabelField(contentPosition, fieldInfo.Name);
                contentPosition.y += height + space;
                contentPosition.height = height * expandValue + space * (expandValue - 1);
                var style = new GUIStyle(EditorStyles.textArea)
                {
                    wordWrap = true
                };
                item.stringValue = EditorGUI.TextArea(contentPosition, item.stringValue, style);
                contentPosition.y += height * expandValue + space * expandValue;
                contentPosition.height = height;
            }
            else
            {
                item.stringValue = EditorGUI.TextField(contentPosition, fieldInfo.Name, item.stringValue);
                contentPosition.y += height + space;
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.FindPropertyRelative("expandFlag").boolValue)
        {
            return height + (height + space) * (expandValue + 1) * fieldLength;
        }
        else
        {
            return height + (height + space) * fieldLength;
        }
    }
}
