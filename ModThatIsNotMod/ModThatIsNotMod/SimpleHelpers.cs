using System.Text.RegularExpressions;

namespace ModThatIsNotMod
{
    public static class SimpleHelpers
    {
        /// <summary>
        /// Removes things like [2] and (Clone)
        /// </summary>
        public static string GetCleanObjectName(string name)
        {
            Regex regex = new Regex(@"\[\d+\]|\(\d+\)"); // Stuff like (1) or [24]
            name = regex.Replace(name, "");
            name = name.Replace("(Clone)", "");
            return name.Trim();
        }
    }
}
