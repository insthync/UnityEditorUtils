using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class BaseCustomCategorizedEditor : BaseCustomEditor
{
    public const string UNCATEGORIZED_HEADER = "Uncategorized";
    public const int UNCATEGORIZED_ORDER = -10000;
    protected Dictionary<string, CategoryData> categorizedProperties;

    protected override void OnEnable()
    {
        base.OnEnable();
        categorizedProperties = new Dictionary<string, CategoryData>();
        SerializedProperty property = serializedObject.GetIterator();
        PrepareFields(property);
    }

    protected void PrepareFields(SerializedProperty obj)
    {
        if (obj.NextVisible(true))
        {
            string lastHeader = string.Empty;
            CategoryData tempCategoryData;
            do
            {
                CategoryAttribute category = GetPropertyAttribute<CategoryAttribute>(obj, false);
                if (category == null && string.IsNullOrEmpty(lastHeader))
                {
                    // Uncategorized
                    lastHeader = UNCATEGORIZED_HEADER;
                    tempCategoryData = CreateOrGetCategoryData(categorizedProperties, lastHeader, UNCATEGORIZED_ORDER);
                }
                else if (category != null)
                {
                    // Categorized by header
                    lastHeader = category.category;
                    tempCategoryData = CreateOrGetCategoryData(categorizedProperties, lastHeader, category.order);
                }
                else
                {
                    // Get category data by previous header
                    tempCategoryData = CreateOrGetCategoryData(categorizedProperties, lastHeader, 0);
                }
                tempCategoryData.PropertyPaths.Add(obj.propertyPath);
            } while (obj.NextVisible(false));
        }
    }

    protected override void RenderFields()
    {
        // Sort category by order
        List<CategoryData> categorizedPropertiesValues = new List<CategoryData>(categorizedProperties.Values);
        categorizedPropertiesValues.Sort();
        foreach (CategoryData categoryData in categorizedPropertiesValues)
        {
            categoryData.Show = EditorGUILayout.BeginFoldoutHeaderGroup(categoryData.Show, categoryData.Name);
            if (categoryData.Show)
            {
                EditorGUILayout.BeginVertical();
                foreach (string propertyPath in categoryData.PropertyPaths)
                {
                    RenderField(serializedObject.FindProperty(propertyPath));
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }

    protected CategoryData CreateOrGetCategoryData(Dictionary<string, CategoryData> categorizedProperties, string category, int order)
    {
        CategoryData tempCategoryData;
        if (!categorizedProperties.ContainsKey(category))
        {
            tempCategoryData = new CategoryData(category, order);
            categorizedProperties.Add(category, tempCategoryData);
        }
        else
        {
            tempCategoryData = categorizedProperties[category];
        }
        return tempCategoryData;
    }

    protected override void RenderField(SerializedProperty obj)
    {
        base.RenderField(obj);
    }

    public T GetPropertyAttribute<T>(SerializedProperty prop, bool inherit)
        where T : PropertyAttribute
    {
        if (prop == null)
            return null;
        Type lookupType = target.GetType();
        do
        {
            FieldInfo field = lookupType.GetField(prop.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            T[] attributes;
            if (field != null)
            {
                attributes = field.GetCustomAttributes(typeof(T), inherit) as T[];
                return attributes.Length > 0 ? attributes[0] : null;
            }
            lookupType = lookupType.BaseType;
        } while (lookupType != typeof(UnityEngine.Object));
        return null;
    }

    protected class CategoryData : IComparable
    {
        public string Name { get; private set; }
        public int Order { get; set; }
        public List<string> PropertyPaths { get; private set; } = new List<string>();
        public bool Show { get; set; }

        public CategoryData(string name, int order)
        {
            Name = name;
            Order = order;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is CategoryData))
                return -1;
            return Order.CompareTo(((CategoryData)obj).Order);
        }
    }
}
