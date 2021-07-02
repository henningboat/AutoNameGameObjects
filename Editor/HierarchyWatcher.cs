using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public static class HierarchyWatcher
    {
        private const string DefaultGameobjectName = "GameObject";
        private const string DefaultCopiedGameObjectName = "GameObject (";

        #region Static

        private static GameObject s_ObservedObject;
        private static readonly TypeCache.TypeCollection s_TypesWithAttributes;

        private static bool HasDefaultGameObjectName(GameObject gameObject)
        {
            return gameObject.name == DefaultGameobjectName || gameObject.name.StartsWith(DefaultCopiedGameObjectName);
        }

        private static void OnHierarchyChanged()
        {
            if (s_ObservedObject == null)
            {
                return;
            }

            var components = s_ObservedObject.GetComponents<Component>();
            if (components.Length != 1)
            {
                foreach (var component in components)
                    if (HasAttribute(component))
                    {
                        RenameGameObject(component);
                    }

                s_ObservedObject = null;
            }
        }

        private static void RenameGameObject(Component component)
        {
            s_ObservedObject.name = component.GetType().Name;
            s_ObservedObject = null;
        }

        private static bool HasAttribute(Component component)
        {
            return s_TypesWithAttributes.Contains(component.GetType());
        }

        private static void OnSelectionChanged()
        {
            s_ObservedObject = null;

            //should we add support for multiple game objects?
            if (Selection.count != 1)
            {
                return;
            }

            if (Selection.activeGameObject == null)
            {
                return;
            }

            if (!HasDefaultGameObjectName(Selection.activeGameObject))
            {
                return;
            }

            if (PrefabUtility.IsPartOfAnyPrefab(Selection.activeGameObject))
            {
                return;
            }
            
            //An empty game object always contains a Transform, so we check if the component count is 1
            var components = Selection.activeGameObject.GetComponents<Component>();
            if (components.Length != 1)
            {
                return;
            }

            s_ObservedObject = Selection.activeGameObject;
        }

        static HierarchyWatcher()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            Selection.selectionChanged += OnSelectionChanged;

            s_TypesWithAttributes = TypeCache.GetTypesWithAttribute<AutoNameGameObjectAttribute>();
        }

        #endregion
    }
}