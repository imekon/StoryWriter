namespace StoryWriter
{
    internal class Story
    {
        private string m_title;
        private string m_text;

        public Story()
        {
            m_title = "";
            m_text = "";
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
    }
}
