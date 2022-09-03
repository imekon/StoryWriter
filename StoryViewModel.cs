using System.Collections.ObjectModel;

namespace StoryWriter
{
    internal class StoryViewModel : ViewModelBase
    {
        private Story m_story;
        private ObservableCollection<StoryViewModel> m_children;

        public StoryViewModel(Story story)
        {
            m_story = story;
            m_children = new ObservableCollection<StoryViewModel>();
        }

        public string Title
        {
            get => m_story.Title;
            set
            {
                m_story.Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Text
        {
            get => m_story.Text;
            set
            {
                m_story.Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public string Folder
        {
            get => m_story.Folder;
            set
            {
                m_story.Folder = value;
                OnPropertyChanged(nameof(Folder));
            }
        }

        public string Tags
        {
            get => m_story.Tags;
            set
            {
                m_story.Tags = value;
                OnPropertyChanged(nameof(Tags));
            }
        }

        public int WordCount
        {
            get
            {
                return Helpers.GetWordCount(m_story.Text);
            }
        }

        public Story Story => m_story;

        public ObservableCollection<StoryViewModel> Children => m_children;

        public string GetMarkdown()
        {
            return m_story.GetMarkdown();
        }
    }
}
