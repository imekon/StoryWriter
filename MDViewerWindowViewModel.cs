using System.Windows.Documents;
using MdXaml;

namespace StoryWriter
{
    public class MDViewerWindowViewModel
    {
        private Story m_story;

        public MDViewerWindowViewModel(Story story)
        {
            m_story = story;
        }

        public string Markdown
        {
            get
            {
                var markdown = m_story.GetMarkdown();
                return markdown;
            }
        }
    }
}
