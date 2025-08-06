#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Data;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GameplayTag))]
public class GameplayTagPropertyDrawer : PropertyDrawer
{
    // Instance field to store the callback for this specific property
    private System.Action<string> _onTagSelected;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        var tagNameProperty = property.FindPropertyRelative("tagName");
        
        // Safety check
        if (tagNameProperty == null)
        {
            EditorGUI.LabelField(position, label, new GUIContent("Error: tagName field not found"));
            EditorGUI.EndProperty();
            return;
        }
        
        // Get all available tags
        var allTags = GetAllGameplayTags();
        var currentValue = tagNameProperty.stringValue ?? "";
        
        // Draw current value as a button that opens the selector
        var buttonPosition = EditorGUI.PrefixLabel(position, label);
        
        var displayText = string.IsNullOrEmpty(currentValue) ? "Select Tag..." : currentValue;
        if (GUI.Button(buttonPosition, displayText, EditorStyles.popup))
        {
            // Store the callback for this specific property instance
            _onTagSelected = (selectedTag) => {
                tagNameProperty.stringValue = selectedTag;
                property.serializedObject.ApplyModifiedProperties();
            };
            
            // Show tag selection window
            TagSelectionWindow.Show(allTags, currentValue, _onTagSelected);
        }
        
        EditorGUI.EndProperty();
    }
    
    private List<GameplayTag> GetAllGameplayTags()
    {
        var result = new List<GameplayTag>();
        
        try
        {
            var gameplayTagsType = typeof(GameplayTags);
            var fields = gameplayTagsType.GetFields(BindingFlags.Public | BindingFlags.Static);
            
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(GameplayTag))
                {
                    var tag = (GameplayTag)field.GetValue(null);
                    if (!string.IsNullOrEmpty(tag.TagName))
                    {
                        result.Add(tag);
                    }
                }
            }
        }
        catch (System.Exception)
        {
            // If reflection fails, return empty list
        }
        
        return result.OrderBy(t => t.TagName).ToList();
    }
}

// Simple working tree view window
public class TagSelectionWindow : EditorWindow
{
    private System.Action<string> onTagSelected; // Instance field instead of static
    private List<GameplayTag> allTags;
    private string currentSelection;
    private Vector2 scrollPosition;
    private string searchFilter = "";
    
    // Instance field for expanded states (per window instance)
    private HashSet<string> expandedNodes = new HashSet<string>();
    
    public static void Show(List<GameplayTag> tags, string currentTag, System.Action<string> callback)
    {
        var window = CreateInstance<TagSelectionWindow>();
        window.allTags = tags.OrderBy(t => t.TagName).ToList();
        window.currentSelection = currentTag;
        window.onTagSelected = callback;
        
        window.titleContent = new GUIContent("Select Gameplay Tag");
        window.minSize = new Vector2(300, 400);
        window.ShowAuxWindow();
    }
    
    void OnGUI()
    {
        // Search field
        searchFilter = EditorGUILayout.TextField("Search", searchFilter);
        
        EditorGUILayout.Space();
        
        // Scroll view for tags
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        if (string.IsNullOrEmpty(searchFilter))
        {
            DrawTree();
        }
        else
        {
            DrawFilteredResults();
        }
        
        EditorGUILayout.EndScrollView();
        
        // None option
        if (GUILayout.Button("None"))
        {
            onTagSelected?.Invoke("");
            Close();
        }
    }
    
    private void DrawTree()
    {
        if (allTags == null) return;
        
        // Sort tags by name to ensure consistent ordering
        var sortedTags = allTags.OrderBy(t => t.TagName).ToList();
        
        // Group tags by their hierarchy level
        var rootTags = sortedTags.Where(t => !t.TagName.Contains(".")).ToList();
        
        foreach (var tag in rootTags)
        {
            DrawTagRecursive(tag, 0, sortedTags);
        }
    }
    
    private void DrawTagRecursive(GameplayTag tag, int depth, List<GameplayTag> allSortedTags)
    {
        GUILayout.BeginHorizontal();
        
        // Indentation
        GUILayout.Space(depth * 20);
        
        // Check if this tag has children
        var children = allSortedTags.Where(t => t.TagName.StartsWith(tag.TagName + ".") 
                                             && t.TagName.Count(c => c == '.') == tag.TagName.Count(c => c == '.') + 1)
                                   .ToList();
        
        bool hasChildren = children.Count > 0;
        
        // Expand/collapse button
        if (hasChildren)
        {
            bool isExpanded = expandedNodes.Contains(tag.TagName);
            string arrow = isExpanded ? "▼" : "▶";
            
            if (GUILayout.Button(arrow, GUILayout.Width(20)))
            {
                if (isExpanded)
                {
                    expandedNodes.Remove(tag.TagName);
                }
                else
                {
                    expandedNodes.Add(tag.TagName);
                }
                Repaint(); // Force repaint to update the view
                return;
            }
        }
        else
        {
            GUILayout.Space(25); // Space for no arrow + padding
        }
        
        // Tag selection button
        var isCurrent = tag.TagName == currentSelection;
        var buttonStyle = new GUIStyle(GUI.skin.label);
        buttonStyle.alignment = TextAnchor.MiddleLeft;
        
        // Change selected value to white bold
        if (isCurrent)
        {
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.normal.textColor = Color.white;
        }
        
        // Show only the last part of the tag name for better readability
        var displayName = tag.TagName.Contains(".") ? 
                         tag.TagName.Substring(tag.TagName.LastIndexOf('.') + 1) : 
                         tag.TagName;
        
        if (GUILayout.Button(displayName, buttonStyle))
        {
            onTagSelected?.Invoke(tag.TagName);
            Close();
        }
        
        GUILayout.EndHorizontal();
        
        // Draw children if expanded
        if (hasChildren && expandedNodes.Contains(tag.TagName))
        {
            foreach (var child in children.OrderBy(c => c.TagName))
            {
                DrawTagRecursive(child, depth + 1, allSortedTags);
            }
        }
    }
    
    private void DrawFilteredResults()
    {
        if (allTags == null) return;
        
        var filteredTags = allTags.Where(t => t.TagName.ToLower().Contains(searchFilter.ToLower()))
                                 .OrderBy(t => t.TagName)
                                 .ToList();
        
        foreach (var tag in filteredTags)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            
            var isCurrent = tag.TagName == currentSelection;
            var buttonStyle = new GUIStyle(GUI.skin.button);
            if (isCurrent)
            {
                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.normal.textColor = Color.white;
            }
            
            if (GUILayout.Button(tag.TagName, buttonStyle))
            {
                onTagSelected?.Invoke(tag.TagName);
                Close();
            }
            
            GUILayout.EndHorizontal();
        }
    }
}
#endif