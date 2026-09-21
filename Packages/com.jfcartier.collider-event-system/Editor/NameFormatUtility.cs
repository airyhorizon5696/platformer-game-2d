using System.Text;

namespace ColliderEventSystem.Editor
{
    public static class NameFormatUtility
    {
        /// <summary>
        /// Inserts spaces before capital letters, e.g. "LookingAtCondition" -> "Looking At Condition".
        /// </summary>
        public static string AddSpacesToPascalCase(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            var result = new StringBuilder(text.Length * 2);
            result.Append(text[0]);

            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && !char.IsUpper(text[i - 1]))
                {
                    result.Append(' ');
                }

                result.Append(text[i]);
            }

            return result.ToString();
        }
    }
}
