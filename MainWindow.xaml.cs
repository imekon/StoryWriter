using System.Windows;

namespace StoryWriter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel m_viewModel;

        public MainWindow()
        {
            InitializeComponent();

            m_viewModel = new MainWindowViewModel();

            DataContext = m_viewModel;
        }

        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var storyViewModel = e.NewValue as StoryViewModel;
            if (storyViewModel == null)
                return;

            m_viewModel.SetStory(storyViewModel);
        }
    }
}
