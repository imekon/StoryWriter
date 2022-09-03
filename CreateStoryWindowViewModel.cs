using System.Windows.Input;

namespace StoryWriter
{
    public class CreateStoryWindowViewModel
    {
        public CreateStoryWindowViewModel()
        {
            Title = "untitled";
            Folder = "Generic";
            Tags = "";
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
                    
                });
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {

                });
            }
        }
    }
}
