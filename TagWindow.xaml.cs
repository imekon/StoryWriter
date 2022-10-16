using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace StoryWriter
{
    /// <summary>
    /// Interaction logic for TagWindow.xaml
    /// </summary>
    public partial class TagWindow : Window
    {
        public TagWindow(object[] stories, string[] tags)
        {
            InitializeComponent();

            var list = new List<StoryViewModel>();

            foreach (var story in stories)
                list.Add(story as StoryViewModel);

            DataContext = new TagWindowViewModel(list.ToArray(), tags);
        }
    }
}
