using System.Windows.Input;

namespace StoryWriter
{
    public class CreateStoryWindowViewModel
    {
        private CreateStoryWindow m_window;

        public CreateStoryWindowViewModel(CreateStoryWindow window)
        {
            Title = "untitled";
            Folder = "Generic";
            Tags = "";

            m_window = window;
        }

        public string Title { get; set; }
        public string Folder { get; set; }
        public string Tags { get; set; }

        public ICommand OKCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    m_window.DialogResult = true;
                });
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    m_window.DialogResult = false;
                });
            }
        }
    }
}
