using System;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class AutoNameGameObjectAttribute : Attribute
    {
        #region Public Fields

        public readonly string name;

        #endregion

        #region Constructors

        public AutoNameGameObjectAttribute(string name = default)
        {
            this.name = name;
        }

        #endregion
    }


    [InitializeOnLoad]
    public static class HierarchyWatcher
    {
        #region Static

        static GameObject s_ObservedObject;
        static int s_CachedComponentCount;


        static bool HasDefaultGameObjectName(GameObject gameObject)
        {
            return gameObject.name.StartsWith("GameObject");
        }

        static void OnHierarchyChanged()
        {
            if (s_ObservedObject == null)
            {
                return;
            }

            var components = s_ObservedObject.GetComponents<Component>();
            if (components.Length != s_CachedComponentCount)
            {
                foreach (Component component in components)
                {
                    if (HasAttribute(component))
                    {
                        RenameGameObject(component);
                    }
                }
            }

            s_CachedComponentCount = components.Length;
        }

        static void RenameGameObject(Component component)
        {
            s_ObservedObject.name = component.GetType().Name;
            s_ObservedObject = null;
        }

        static bool HasAttribute(Component component)
        {
            return component.GetType() != typeof(Transform);
        }

        static void OnSelectionChanged()
        {
            s_ObservedObject = null;

            //todo add support for multiple GameObjects
            if (Selection.count != 1)
            {
                return;
            }

            if (Selection.activeGameObject == null)
            {
                return;
            }

            if (HasDefaultGameObjectName(Selection.activeGameObject))
            {
                s_ObservedObject = Selection.activeGameObject;
                var components = s_ObservedObject.GetComponents<Component>();
                s_CachedComponentCount = components.Length;
            }
        }

        static HierarchyWatcher()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            Selection.selectionChanged += OnSelectionChanged;
        }

        #endregion
    }
}