using UnityEngine;

public class CustomNameAttribute : PropertyAttribute
{
    public string NewName { get; private set; }
    public CustomNameAttribute(string name)
    {
        NewName = name;
    }
}