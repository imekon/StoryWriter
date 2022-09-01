using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StoryWriter
{
    internal class Story
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
    }
}
