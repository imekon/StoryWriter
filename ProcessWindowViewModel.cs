using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace StoryWriter
{
    internal class ProcessWindowViewModel : ViewModelBase
    {
        private string processPath;
        private ObservableCollection<ProcessStoryViewModel> processStories;

        public ProcessWindowViewModel()
        {
            processPath = @"F:\stories";
            processStories = new ObservableCollection<ProcessStoryViewModel>();
        }

        public string ProcessPath
        {
            get => processPath;
            set
            {
                processPath = value;
                OnPropertyChanged(nameof(ProcessPath));
            }
        }

        public ICommand ProcessCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    var files = Directory.GetFiles(processPath, "*.md");
                    foreach(var filename in files)
                    {
                        var story = new Story();
                        story.Title = Path.GetFileNameWithoutExtension(filename);
                        story.Text = File.ReadAllText(filename);
                        var storyViewModel = new ProcessStoryViewModel(story);
                        processStories.Add(storyViewModel);
                    }
                });
            }
        }

        public ObservableCollection<ProcessStoryViewModel> ProcessStories => processStories;
    }
}
