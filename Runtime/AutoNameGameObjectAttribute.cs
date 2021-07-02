using System;

namespace AutoRenameGameObject
{
    /// <summary>
    /// This attribute can be added to MonoBehaviours. If a MonoBehaviour with this attribute
    /// is added to a an empty game object (with the default name), it will automatically be
    /// renamed to the name of the class
    /// Please note that this feature can be enabled/disabled in the Preferences Window
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoNameGameObjectAttribute : Attribute
    {
    }
}