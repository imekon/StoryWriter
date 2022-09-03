using System.Windows;

namespace StoryWriter
{
    /// <summary>
    /// Interaction logic for CreateStoryWindow.xaml
    /// </summary>
    public partial class CreateStoryWindow : Window
    {
        public CreateStoryWindow(CreateStoryWindowViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
