using System.Linq;
using System.Windows;

namespace StoryWriter
{
    /// <summary>
    /// Interaction logic for TagWindow.xaml
    /// </summary>
    public partial class TagWindow : Window
    {
        public TagWindow(string[] tags)
        {
            InitializeComponent();

            DataContext = new TagWindowViewModel(tags.ToList());
        }
    }
}
