using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace StoryWriter
{
    internal class ProcessStoryViewModel : ViewModelBase
    {
        private Story story;

        public ProcessStoryViewModel(Story story)
        {
            this.story = story;
        }

        public string Title
        {
            get => story.Title;
            set
            {
                story.Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Text
        {
            get => story.Text;
        }

        public int WordCount => story.WordCount;
    }
}