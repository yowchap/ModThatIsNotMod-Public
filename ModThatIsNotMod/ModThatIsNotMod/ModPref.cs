using MelonLoader;

namespace ModThatIsNotMod
{
    public class ModPref<T>
    {
        public string categoryId { get; private set; }
        public string id { get; private set; }
        public T value { get; private set; }

        public ModPref(string categoryId, string id, T value)
        {
            this.categoryId = categoryId;
            this.id = id;
            this.value = value;
        }

        /// <summary>
        /// Creates an entry in MelonPreferences.cfg
        /// </summary>
        public void CreateEntry(bool dontSaveDefault = false)
        {
            MelonPreferences.CreateEntry(categoryId, id, value, dont_save_default: dontSaveDefault);
        }

        /// <summary>
        /// Sets the value in MelonPreferences.cfg
        /// </summary>
        public void SetValue(T value)
        {
            this.value = value;
            MelonPreferences.SetEntryValue(categoryId, id, value);
            MelonPreferences.Save();
        }

        /// <summary>
        /// Reads the value from MelonPreferences.cfg
        /// </summary>
        public T ReadValue()
        {
            value = MelonPreferences.GetEntryValue<T>(categoryId, id);
            return value;
        }
    }
}
