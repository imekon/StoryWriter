using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StoryWriter
{
    internal class TagWindowViewModel : ViewModelBase
    {
        private ObservableCollection<TagViewModel> m_tags;

        public TagWindowViewModel(List<string> tags)
        {
            m_tags = new ObservableCollection<TagViewModel>();

            foreach (var tag in tags)
                m_tags.Add(new TagViewModel(tag));
        }

        public ObservableCollection<TagViewModel> Tags => m_tags;
    }
}
