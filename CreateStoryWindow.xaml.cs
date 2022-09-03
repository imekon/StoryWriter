using System.Windows;

namespace StoryWriter
{
    /// <summary>
    /// Interaction logic for CreateStoryWindow.xaml
    /// </summary>
    public partial class CreateStoryWindow : Window
    {
        private CreateStoryWindowViewModel m_viewModel;

        public CreateStoryWindow()
        {
            InitializeComponent();

            m_viewModel = new CreateStoryWindowViewModel(this);

            DataContext = m_viewModel;
        }

        public string StoryTitle
        {
            get => m_viewModel.Title;
            set => m_viewModel.Title = value;
        }

        public string Folder
        {
            get => m_viewModel.Folder;
            set => m_viewModel.Folder = value;
        }

        public string Tags
        {
            get => m_viewModel.Tags;
            set => m_viewModel.Tags = value;
        }
    }
}
