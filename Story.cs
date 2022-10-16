using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using LiteDB;

namespace StoryWriter
{
    public class Story
    {
        private string m_title;
        private string m_text;
        private string m_folder;
        private string m_tags;
        private StoryState m_state;

        public Story()
        {
            m_title = "";
            m_text = "";
            m_folder = "Generic";
            m_tags = "";
            m_state = StoryState.Normal;
        }

        [JsonIgnore]
        public int Id { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        public StoryState State
        {
            get => m_state;
            set => m_state = value;
        }

        public string Title
        {
            get => m_title;
            set
            {
                m_title = value;
                if (m_state == StoryState.Normal)
                    m_state = StoryState.Modified;
            }
        }

        public string Text
        {
            get => m_text;
            set
            {
                m_text = value;
                if (m_state == StoryState.Normal)
                    m_state = StoryState.Modified;
            }
        }

        public string Folder
        {
            get => m_folder;
            set
            {
                m_folder = value;
                if (m_state == StoryState.Normal)
                    m_state = StoryState.Modified;
            }
        }

        public string Tags
        {
            get => m_tags;
            set
            {
                m_tags = value;
                if (m_state == StoryState.Normal)
                    m_state = StoryState.Modified;
            }
        }

        public bool Contains(string text)
        {
            return m_text.Contains(text);
        }

        public string[] GetTags()
        {
            if (string.IsNullOrEmpty(m_tags))
                return Array.Empty<string>();

            return m_tags.Split(",");
        }

        public bool AddTag(string tag)
        {
            var tags = new List<string>(GetTags());

            if (tags.Contains(tag))
                return false;

            tags.Add(tag);

            m_tags = string.Join(",", tags);
            if (m_state == StoryState.Normal)
                m_state = StoryState.Modified;

            return true;
        }

        public bool RemoveTag(string tag)
        {
            var tags = new List<string>(GetTags());

            if (!tags.Contains(tag))
                return false;

            tags.Remove(tag);

            m_tags = string.Join(",", tags);
            if (m_state == StoryState.Normal)
                m_state = StoryState.Modified;

            return true;
        }

        [BsonIgnore]
        public int CharacterCount
        {
            get => Helpers.GetCharacterCount(m_text);
        }

        [BsonIgnore]
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

        public bool ContainsTag(string tag)
        {
            if (string.IsNullOrEmpty(m_tags))
                return false;

            return m_tags.Contains(tag);
        }
    }
}
