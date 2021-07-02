using System.Collections.Generic;
using UnityEditor;

namespace AutoRenameGameObject.Editor
{
    internal static class Settings
    {
        public const string EditorPrefsKey = "AutoNameGameObject.enabled";

        private static readonly Dictionary<Mode, string> s_Descriptions = new Dictionary<Mode, string>
        {
            {Mode.Disabled, "Empty Game Objects will never be renamed"},
            {Mode.ForComponentsWithAttribute, "Empty Game Objects will be renamed if a component is added that has the [AutoNameGameObject] attribute"},
            {Mode.ForAllComponents, "Empty Game Objects will be renamed if any component is added"}
        };

        public static Mode Mode
        {
            get => (Mode) EditorPrefs.GetInt(EditorPrefsKey, (int) Mode.ForComponentsWithAttribute);
            set => EditorPrefs.SetInt(EditorPrefsKey, (int) value);
        }

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            var provider = new SettingsProvider("Preferences/AutoNameGameObject", SettingsScope.User)
            {
                label = "Auto Name Game Object",

                guiHandler = searchContext =>
                {
                    EditorGUILayout.HelpBox(s_Descriptions[Mode], MessageType.Info);
                    Mode = (Mode) EditorGUILayout.EnumPopup("Mode", Mode);
                }
            };

            return provider;
        }
    }
    
    internal enum Mode
    {
        Disabled = 0,
        ForComponentsWithAttribute = 1,
        ForAllComponents = 2
    }
}