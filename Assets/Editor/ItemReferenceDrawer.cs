using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR) 
[CustomPropertyDrawer(typeof(ItemReference))] 
public class ItemReferenceDrawer : PropertyDrawer
{
        /// <summary>
        /// Options to display in the popup to select constant or variable.
        /// </summary>
        private readonly string[] popupOptions =
            { "Weapon", "Heal", "Armor" };

        /// <summary> Cached style to use to draw the popup button. </summary>
        private GUIStyle popupStyle;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (popupStyle == null)
            {
                popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                popupStyle.imagePosition = ImagePosition.ImageOnly;
            }

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();

            // Get properties
            SerializedProperty option = property.FindPropertyRelative("chosenValue");
            SerializedProperty valueWeapon = property.FindPropertyRelative("weapon");
            SerializedProperty valueHeal = property.FindPropertyRelative("healthPack");
            SerializedProperty valueArmor = property.FindPropertyRelative("armorPack");

            // Calculate rect for configuration button
            Rect buttonRect = new Rect(position);
            buttonRect.yMin += popupStyle.margin.top;
            buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
            position.xMin = buttonRect.xMax;

            // Store old indent level and set it to 0, the PrefixLabel takes care of it
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            
            int result = EditorGUI.Popup(buttonRect, ItemReference.StringValueToIndex(option.stringValue), popupOptions, popupStyle);
            option.stringValue = ItemReference.PopupOptions[result];

        SerializedProperty thisProperty;
        switch (result)
        {
            case 0:
                thisProperty = valueWeapon;
                break;
            case 1:
                thisProperty = valueHeal;
                break;
            case 2:
                thisProperty = valueArmor;
                break;
            default:
                thisProperty = valueWeapon;
                break;
        }
        EditorGUI.PropertyField(position,
                thisProperty,
                GUIContent.none);

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
}
#endif