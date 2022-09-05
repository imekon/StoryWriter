using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace StoryWriter
{
    public class Story
    {
        private string m_title;
        private string m_text;
        private string m_folder;
        private string m_tags;

        public Story()
        {
            m_title = "";
            m_text = "";
            m_folder = "Generic";
            m_tags = "";
        }

        public string Title
        {
            get => m_title;
            set
            {
                m_title = value;
            }
        }

        public string Text
        {
            get => m_text;
            set
            {
                m_text = value;
            }
        }

        public string Folder
        {
            get => m_folder;
            set
            {
                m_folder = value;
            }
        }

        public string Tags
        {
            get => m_tags;
            set => m_tags = value;
        }

        public bool Contains(string text)
        {
            return m_text.Contains(text);
        }

        public string[] GetTags()
        {
            return m_tags.Split(",");
        }

        public bool AddTag(string tag)
        {
            var tags = new List<string>(GetTags());

            if (tags.Contains(tag))
                return false;

            tags.Add(tag);

            m_tags = string.Join(",", tags);

            return true;
        }

        public bool RemoveTag(string tag)
        {
            var tags = new List<string>(GetTags());

            if (!tags.Contains(tag))
                return false;

            tags.Remove(tag);

            m_tags = string.Join(",", tags);

            return true;
        }

        public int CharacterCount
        {
            get => Helpers.GetCharacterCount(m_text);
        }

        public int WordCount
        {
            get => Helpers.GetWordCount(m_text);
        }

        public string GetMarkdown()
        {
            var markdown = new StringBuilder();

            markdown.AppendLine("# " + m_title + " #");
            markdown.Append(m_text);

            return markdown.ToString();
        }
    }
}
