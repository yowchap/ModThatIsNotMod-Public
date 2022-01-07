using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace ModThatIsNotMod.MonoBehaviours
{
    // Hopefully some day soon the unhollower will get full support for injected types and I can just delete all this...
    public static class CustomMonoBehaviourHandler
    {
        private static readonly string assembliesFolder = @"MonoBehaviours";
        private static readonly string identifier = "CUSTOM_MONOBEHAVIOUR";
        private static readonly string splitter = "-::-";

        private static Dictionary<string, Type> customMonoBehaviours = new Dictionary<string, Type>();
        private static Dictionary<Type, Dictionary<string, Dictionary<string, object>>> typeValues = new Dictionary<Type, Dictionary<string, Dictionary<string, object>>>(); // Type -> UUID -> field name -> value of field
        private static HashSet<int> customMonoBehaviourInstances = new HashSet<int>();

        private static MethodInfo staticDeserializeJsonMethod;


        internal static void Setup()
        {
            staticDeserializeJsonMethod = typeof(JsonConvert).GetMethods(AccessTools.all).Where(m => m.IsGenericMethod && m.Name == "DeserializeObject" && m.GetParameters().Length == 1).First();

            CreateAssembliesDir();
            RegisterDefaultTypes();
            LoadAndRegisterMonoBehavioursFromUserData();
        }

        private static void RegisterDefaultTypes()
        {
            RegisterAllInAssembly(typeof(Main).Assembly);
        }

        /// <summary>
        /// Sets the values of all fields on the custom monobehaviour.
        /// </summary>
        public static void SetFieldValues(MonoBehaviour monoBehaviour)
        {
            GameObject go = monoBehaviour.gameObject;
            int instanceId = monoBehaviour.GetInstanceID();
            if (customMonoBehaviourInstances.Contains(instanceId))
                return;

            customMonoBehaviourInstances.Add(instanceId);

            Type type = monoBehaviour.GetType();

            LocalizedText[] texts = go.GetComponents<LocalizedText>();
            int componentIndex = GetCustomComponentIndex(go, type);
            if (componentIndex < texts.Length)
            {
                string uuid = texts[componentIndex].key;

                if (typeValues.ContainsKey(type) && typeValues[type].ContainsKey(uuid))
                {
                    Dictionary<string, object> values = typeValues[type][uuid];

                    foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                        if (values.ContainsKey(field.Name))
                            field.SetValue(monoBehaviour, values[field.Name]);
                }
                else
                {
                    ModConsole.Msg($"Couldn't find the correct saved field values for {monoBehaviour.gameObject.name}", LoggingMode.DEBUG);
                }
            }
            else
            {
                ModConsole.Msg($"Couldn't find the correct LocalizedText on {monoBehaviour.gameObject.name}", LoggingMode.DEBUG);
            }
        }

        public static void RegisterMonoBehaviourInIl2Cpp<T>() => RegisterMonoBehaviourInIl2Cpp(typeof(T));
        public static void RegisterMonoBehaviourInIl2Cpp(Type type)
        {
            if (!customMonoBehaviours.ContainsKey(type.FullName))
            {
                MethodInfo registerTypeMethod = typeof(ClassInjector).GetMethod("RegisterTypeInIl2Cpp", AccessTools.all, null, new Type[] { typeof(bool) }, null).MakeGenericMethod(type);
                registerTypeMethod.Invoke(null, new object[] { false });

                customMonoBehaviours.Add(type.FullName, type);

                ModConsole.Msg($"Registered {type.FullName} in IL2CPP", LoggingMode.DEBUG);
            }
        }

        /// <summary>
        /// Reads the values of the monobehaviour and saves them to be applied later.
        /// </summary>
        public static void SaveCustomMonoBehaviour<T>(GameObject obj) where T : MonoBehaviour
        {
            T comp = obj.GetComponent<T>();

            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                values.Add(field.Name, field.GetValue(comp));

            SaveTypeValues(typeof(T), values, obj);
        }

        /// <summary>
        /// Adds an instance of the given monobehaviour where it's needed
        /// </summary>
        public static void AddMonoBehavioursFromJson(GameObject prefab)
        {
            LocalizedText[] localizedTexts = prefab.GetComponentsInChildren<LocalizedText>();
            foreach (LocalizedText text in localizedTexts)
            {
                if (text.key.StartsWith(identifier))
                {
                    // Get the type name and json from the localizedtext component
                    string[] parts = text.key.Split(new string[] { splitter }, StringSplitOptions.None);
                    string typeName = parts[1];
                    string json = parts[2];

                    // Check that this type has been registered as a custom monobehaviour
                    if (customMonoBehaviours.ContainsKey(typeName))
                    {
                        Type type = customMonoBehaviours[typeName];
                        Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json); // Deserialize the json, some types will be wrong here (all int's will be int64's for example)

                        if (text.gameObject.GetComponent(Il2CppType.From(type)) == null)
                            text.gameObject.AddComponent(Il2CppType.From(type)); // Add back the component since it'll be missing from the assetbundle

                        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                        {
                            if (values.ContainsKey(field.Name))
                            {
                                // Reserialize and deserialze the object but this time into the correct type, messy but it works ¯\_(ツ)_/¯
                                string jsonObj = JsonConvert.SerializeObject(values[field.Name]);
                                object castedValue = staticDeserializeJsonMethod.MakeGenericMethod(field.FieldType).Invoke(null, new object[] { jsonObj });
                                values[field.Name] = castedValue;
                            }
                        }

                        SaveTypeValues(type, values, text.gameObject);
                    }
                    else
                    {
                        ModConsole.Msg($"Couldn't find {typeName} in type list", LoggingMode.DEBUG);
                        ModConsole.Msg($"Register types with CustomMonoBehaviourHandler instead of ClassInjector", LoggingMode.DEBUG);
                    }
                }
            }
        }

        /// <summary>
        /// Adds an instance of the given monobehaviour where it's needed
        /// </summary>
        public static void AddMonoBehavioursFromJson(GameObject prefab, JsonSerializerSettings settings)
        {
            LocalizedText[] localizedTexts = prefab.GetComponentsInChildren<LocalizedText>();
            foreach (LocalizedText text in localizedTexts)
            {
                if (text.key.StartsWith(identifier))
                {
                    // Get the type name and json from the localizedtext component
                    string[] parts = text.key.Split(new string[] { splitter }, StringSplitOptions.None);
                    string typeName = parts[1];
                    string json = parts[2];

                    // Check that this type has been registered as a custom monobehaviour
                    if (customMonoBehaviours.ContainsKey(typeName))
                    {
                        Type type = customMonoBehaviours[typeName];
                        Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, settings); // Deserialize the json, some types will be wrong here (all int's will be int64's for example)

                        if (text.gameObject.GetComponent(Il2CppType.From(type)) == null)
                            text.gameObject.AddComponent(Il2CppType.From(type)); // Add back the component since it'll be missing from the assetbundle

                        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                        {
                            if (values.ContainsKey(field.Name))
                            {
                                // Reserialize and deserialze the object but this time into the correct type, messy but it works ¯\_(ツ)_/¯
                                string jsonObj = JsonConvert.SerializeObject(values[field.Name], settings);
                                object castedValue = JsonConvert.DeserializeObject(jsonObj, field.FieldType, settings);
                                values[field.Name] = castedValue;
                            }
                        }

                        SaveTypeValues(type, values, text.gameObject);
                    }
                    else
                    {
                        ModConsole.Msg($"Couldn't find {typeName} in type list", LoggingMode.DEBUG);
                        ModConsole.Msg($"Register types with CustomMonoBehaviourHandler instead of ClassInjector", LoggingMode.DEBUG);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the field values to a dictionary and updates the localized text component so it can be accessed later.
        /// </summary>
        private static void SaveTypeValues(Type type, Dictionary<string, object> values, GameObject obj)
        {
            // Init the <UUID, <FieldName, FieldValue>> dictionary if it doesn't exist yet
            if (!typeValues.ContainsKey(type))
                typeValues.Add(type, new Dictionary<string, Dictionary<string, object>>());

            string uuid = GetValuesUUID(values, obj);

            // Save the current field values into the dictionary
            if (typeValues[type].ContainsKey(uuid))
                typeValues[type][uuid] = values;
            else
                typeValues[type].Add(uuid, values);

            // Get the LocalizedText with the matching component index as the monobehaviour, or add a new one if it doesn't exist
            LocalizedText[] texts = obj.GetComponents<LocalizedText>();
            LocalizedText text = null;
            if (texts.Length > 0)
            {
                int componentIndex = GetCustomComponentIndex(obj, type);
                if (componentIndex < texts.Length)
                    text = texts[componentIndex];
                else
                    text = obj.AddComponent<LocalizedText>();
            }
            else
            {
                // TODO: Add multiple LocalizedTexts to make sure the order is still correct
                text = obj.AddComponent<LocalizedText>();
            }

            text.key = uuid; // UUID is used for setting the values again when the gameobject is instantiated

            ModConsole.Msg($"Saved fields of {type.FullName} on {obj.name}", LoggingMode.DEBUG);
        }

        private static int GetCustomComponentIndex(GameObject obj, Type type)
        {
            // Get all the components on the gameobject, yes this entire mess is all necessary
            Il2CppSystem.Collections.Generic.List<MonoBehaviour> componentsIl2Cpp = new Il2CppSystem.Collections.Generic.List<MonoBehaviour>();
            obj.GetComponents(componentsIl2Cpp);
            List<MonoBehaviour> components = new List<MonoBehaviour>();
            foreach (var item in componentsIl2Cpp)
                components.Add(item);

            // Select just the custom monobehaviours from that list
            Type[] types = components.Where(x => customMonoBehaviours.ContainsValue(x?.GetType())).Select(x => x?.GetType()).ToArray();

            int componentIndex = Array.IndexOf(types, type);
            return componentIndex;
        }

        /// <summary>
        /// Loads all .dlls in the folder for them, finds all monobehaviours, and registers them in the il2cpp domain
        /// </summary>
        private static void LoadAndRegisterMonoBehavioursFromUserData()
        {
            string[] dlls = GetDllFilePaths(Path.Combine(MelonUtils.UserDataDirectory, assembliesFolder));
            foreach (string dll in dlls)
            {
                Assembly assembly = Assembly.LoadFile(dll);
                RegisterAllInAssembly(assembly);
            }
        }

        /// <summary>
        /// Finds every MonoBehaviour in the assembly and registers it
        /// </summary>
        public static void RegisterAllInAssembly(Assembly assembly)
        {
            RegisterAndReturnAllInAssembly(assembly);
        }

        /// <summary>
        /// Finds every MonoBehaviour in the assembly and registers it
        /// </summary>
        public static Type[] RegisterAndReturnAllInAssembly(Assembly assembly)
        {
            List<Type> monoBehaviours = new List<Type>();
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    if (type.BaseType.IsSubclassOf(typeof(MonoBehaviour)))
                        RegisterMonoBehaviourInIl2Cpp(type.BaseType);

                    RegisterMonoBehaviourInIl2Cpp(type);
                    monoBehaviours.Add(type);
                }
            }
            return monoBehaviours.ToArray();
        }

        /// <summary>
        /// Recursively finds all .dll files in the given path.
        /// </summary>
        private static string[] GetDllFilePaths(string path)
        {
            List<string> files = new List<string>();

            foreach (string file in Directory.GetFiles(path))
                if (Path.GetExtension(file) == ".dll")
                    files.Add(file);

            foreach (string dir in Directory.GetDirectories(path))
                files.AddRange(GetDllFilePaths(dir));

            return files.ToArray();
        }

        private static string GetValuesUUID<TKey, TValue>(Dictionary<TKey, TValue> dictionary, GameObject obj) => string.Join(",", dictionary) + obj.name + obj.GetInstanceID();

        private static void CreateAssembliesDir() => Directory.CreateDirectory(Path.Combine(MelonUtils.UserDataDirectory, assembliesFolder));


        private struct ValueData
        {
            public string uuid;
            public Dictionary<string, object> values;

            public ValueData(string uuid, Dictionary<string, object> values)
            {
                this.uuid = uuid;
                this.values = values;
            }
        }
    }
}
