using UnityEditor;
using UnityEngine;

namespace AutoRenameGameObject.Editor
{
    /// <summary>
    ///     Static class that observes changes in the hierarchy. It will notice when
    ///     the user is adding a script to an empty game object. If this script is marked
    ///     with the AutoNameGameObjectAttribute, it will rename the class if the game object
    ///     currently has the default name ("GameObject" or "GameObject (x)"
    /// </summary>
    [InitializeOnLoad]
    internal static class HierarchyWatcher
    {
        private const string DefaultGameObjectName = "GameObject";
        private const string DefaultCopiedGameObjectName = "GameObject (";

        private static GameObject s_ObservedGameObject;
        private static readonly TypeCache.TypeCollection s_TypesWithAttributes;

        static HierarchyWatcher()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            Selection.selectionChanged += OnSelectionChanged;

            s_TypesWithAttributes = TypeCache.GetTypesWithAttribute<AutoNameGameObjectAttribute>();
        }

        private static void OnHierarchyChanged()
        {
            if (Settings.Mode == Mode.Disabled)
            {
                return;
            }

            if (s_ObservedGameObject == null)
            {
                return;
            }

            if (!HasDefaultGameObjectName(s_ObservedGameObject))
            {
                s_ObservedGameObject = null;
                return;
            }

            var components = s_ObservedGameObject.GetComponents<Component>();
            if (components.Length != 1)
            {
                if (ShouldRenameGameObject(components, out var name))
                {
                    s_ObservedGameObject.name = name;
                    s_ObservedGameObject = null;
                }
            }
        }

        private static bool ShouldRenameGameObject(Component[] components, out string name)
        {
            name = default;
            if (Settings.Mode == Mode.Disabled)
            {
                return false;
            }

            foreach (var component in components)
            {
                var componentType = component.GetType();

                //all game objects have a Transform and we never want to use that as the name
                if (componentType == typeof(Transform))
                {
                    continue;
                }

                if (Settings.Mode == Mode.ForComponentsWithAttribute)
                {
                    if (!s_TypesWithAttributes.Contains(componentType))
                    {
                        continue;
                    }
                }

                name = ObjectNames.NicifyVariableName(componentType.Name);
                return true;
            }

            return false;
        }

        private static bool HasDefaultGameObjectName(GameObject gameObject)
        {
            return gameObject.name == DefaultGameObjectName || gameObject.name.StartsWith(DefaultCopiedGameObjectName);
        }

        private static void OnSelectionChanged()
        {
            s_ObservedGameObject = null;

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

            s_ObservedGameObject = Selection.activeGameObject;
        }
    }
}