using System.Collections.ObjectModel;

namespace StoryWriter
{
    public class FolderViewModel : ViewModelBase
    {
        private string m_title;
        private ObservableCollection<StoryViewModel> m_children;

        public FolderViewModel(string title)
        {
            m_title = title;
            m_children = new ObservableCollection<StoryViewModel>();
        }

        public string Title => m_title;

        public ObservableCollection<StoryViewModel> Children => m_children;
    }
}
