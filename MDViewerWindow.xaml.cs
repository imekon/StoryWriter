using System.Windows;

namespace StoryWriter
{
    /// <summary>
    /// Interaction logic for MDViewerWindow.xaml
    /// </summary>
    public partial class MDViewerWindow : Window
    {
        public MDViewerWindow(MDViewerWindowViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
