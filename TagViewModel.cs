namespace StoryWriter
{
    internal class TagViewModel : ViewModelBase
    {
        private bool m_selected;
        private string m_name;

        public TagViewModel(string name)
        {
            m_selected = false;
            m_name = name;
        }

        public bool Selected
        {
            get => m_selected;
            set
            {
                m_selected = value;
                OnPropertyChanged(nameof(Selected));
            }
        }

        public string Name
        {
            get => m_name;
        }
    }
}
