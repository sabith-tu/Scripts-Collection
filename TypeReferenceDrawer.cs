using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class TypeReference
{
    public string TypeName; // Store the type name
}

[CustomPropertyDrawer(typeof(TypeReference))]
public class TypeReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty typeNameProp = property.FindPropertyRelative("TypeName");

        // Create a dropdown for selecting the type
        if (
            GUI.Button(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                string.IsNullOrEmpty(typeNameProp.stringValue)
                    ? "Select Type"
                    : typeNameProp.stringValue,
                EditorStyles.miniPullDown
            )
        )
        {
            // Show a type selection dialog
            GenericMenu menu = new GenericMenu();
            foreach (var type in GetAllTypes())
            {
                menu.AddItem(
                    new GUIContent(type.FullName),
                    false,
                    () =>
                    {
                        typeNameProp.stringValue = type.FullName; // Store the full name of the type
                        Debug.Log($"[SAB] [Type] {typeNameProp.stringValue}");
                        property.serializedObject.ApplyModifiedProperties();
                    }
                );
            }
            menu.ShowAsContext();
        }
    }

    private Type[] GetAllTypes()
    {
        // Get all types from the current assembly
        return Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsGenericType) // Exclude abstract and generic types
            .ToArray();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight; // Adjust height for the button
    }
}
