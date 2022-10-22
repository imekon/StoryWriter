using System.Collections.ObjectModel;
using ICSharpCode.AvalonEdit.Editing;

namespace StoryWriter
{
    public class StoryViewModel : ViewModelBase
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
                OnPropertyChanged(nameof(IsModified));
            }
        }

        public string Text
        {
            get => m_story.Text;
            set
            {
                m_story.Text = value;
                OnPropertyChanged(nameof(Text));
                OnPropertyChanged(nameof(IsModified));
            }
        }

        public string Folder
        {
            get => m_story.Folder;
            set
            {
                m_story.Folder = value;
                OnPropertyChanged(nameof(Folder));
                OnPropertyChanged(nameof(IsModified));
            }
        }

        public string Tags
        {
            get => m_story.Tags;
            set
            {
                m_story.Tags = value;
                OnPropertyChanged(nameof(Tags));
                OnPropertyChanged(nameof(IsModified));
            }
        }

        public int WordCount
        {
            get
            {
                return m_story.WordCount;
            }
        }

        public bool IsModified
        {
            get => m_story.State != StoryState.Normal;
        }

        public bool IsFavourite
        {
            get => m_story.ContainsTag("favourite");
        }

        public Story Story => m_story;

        public ObservableCollection<StoryViewModel> Children => m_children;

        public string GetMarkdown()
        {
            return m_story.GetMarkdown();
        }

        public void StatePropertyChanged()
        {
            OnPropertyChanged(nameof(IsModified));
            OnPropertyChanged(nameof(IsFavourite));
            OnPropertyChanged(nameof(WordCount));
        }
    }
}
