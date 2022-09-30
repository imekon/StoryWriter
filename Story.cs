using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using LiteDB;

namespace StoryWriter
{
    public class Story
    {
        private string m_title;
        private string m_text;
        private string m_folder;
        private string m_tags;
        private bool m_modified;

        public Story()
        {
            m_title = "";
            m_text = "";
            m_folder = "Generic";
            m_tags = "";
            m_modified = false;
        }

        [JsonIgnore]
        public int Id { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        public bool IsModified
        {
            get => m_modified;
            set => m_modified = value;
        }

        public string Title
        {
            get => m_title;
            set
            {
                m_title = value;
                m_modified = true;
            }
        }

        public string Text
        {
            get => m_text;
            set
            {
                m_text = value;
                m_modified = true;
            }
        }

        public string Folder
        {
            get => m_folder;
            set
            {
                m_folder = value;
                m_modified = true;
            }
        }

        public string Tags
        {
            get => m_tags;
            set
            {
                m_tags = value;
                m_modified = true;
            }
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
            m_modified = true;

            return true;
        }

        public bool RemoveTag(string tag)
        {
            var tags = new List<string>(GetTags());

            if (!tags.Contains(tag))
                return false;

            tags.Remove(tag);

            m_tags = string.Join(",", tags);
            m_modified = true;

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
