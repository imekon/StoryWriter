namespace StoryWriter
{
    internal static class Helpers
    {
        public static int GetCharacterCount(string text)
        {
            int total = 0;

            foreach(var ch in text)
            {
                if (char.IsWhiteSpace(ch))
                    continue;

                total++;
            }

            return total;
        }

        public static int GetWordCount(string text)
        {
            int wordCount = 0, index = 0;

            // skip whitespace until first word
            while (index < text.Length && char.IsWhiteSpace(text[index]))
                index++;

            while (index < text.Length)
            {
                // check if current char is part of a word
                while (index < text.Length && !char.IsWhiteSpace(text[index]))
                    index++;

                wordCount++;

                // skip whitespace until next word
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                    index++;
            }

            return wordCount;
        }
    }
}
