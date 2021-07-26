using UnityEngine;

public class CategoryAttribute : PropertyAttribute
{
    public string category { get; private set; }
    public CategoryAttribute(int order, string category)
    {
        this.category = category;
        this.order = order;
    }

    public CategoryAttribute(string category) : this(0, category)
    {
    }
}
