using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace StoryWriter
{
    internal class TagWindowViewModel : ViewModelBase
    {
        private ObservableCollection<StoryViewModel> m_stories;
        private ObservableCollection<TagViewModel> m_tags;
        private List<string> m_selectedTags;
        private StoryViewModel? m_selectedStory;

        private ICollectionView m_storiesView;

        public TagWindowViewModel(StoryViewModel[] stories, string[] tags)
        {
            m_stories = new ObservableCollection<StoryViewModel>();
            m_tags = new ObservableCollection<TagViewModel>();
            m_selectedTags = new List<string>();
            m_selectedStory = null;

            foreach (var story in stories)
                m_stories.Add(story);

            foreach (var tag in tags)
                m_tags.Add(new TagViewModel(tag));

            foreach(var tagViewModel in m_tags)
            {
                tagViewModel.PropertyChanged += OnTagPropertyChanged;
            }

            m_storiesView = CollectionViewSource.GetDefaultView(m_stories);
            m_storiesView.Filter = OnFilterStories;
        }

        private void OnTagPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                var tagViewModel = sender as TagViewModel;
                if (tagViewModel == null)
                    return;

                if (tagViewModel.Selected)
                    m_selectedTags.Add(tagViewModel.Name);
                else
                    m_selectedTags.Remove(tagViewModel.Name);

                m_storiesView.Refresh();
            }
        }

        private bool OnFilterStories(object obj)
        {
            if (!m_selectedTags.Any())
                return true;

            var story = obj as StoryViewModel;
            if (story == null)
                return false;

            foreach(var tag in m_selectedTags)
            {
                if (story.Story.ContainsTag(tag))
                    return true;
            }

            return false;
        }

        public ICollectionView Stories => m_storiesView;
        public ObservableCollection<TagViewModel> Tags => m_tags;

        public StoryViewModel? SelectedStory
        {
            get => m_selectedStory;
            set
            {
                m_selectedStory = value;
                OnPropertyChanged(nameof(SelectedStory));
            }
        }

        public ICommand GotoCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    if (mainWindow == null)
                        return;

                    mainWindow.listView.ScrollIntoView(m_selectedStory);

                    MainWindowViewModel.Instance.SelectedStory = m_selectedStory;
                });
            }
        }

        public ICommand CloseCommand
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
