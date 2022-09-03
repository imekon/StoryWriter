using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace StoryWriter
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private bool m_modified;
        private string m_filename;
        private StoryViewModel? m_story;
        private List<Story> m_stories;
        private ObservableCollection<StoryViewModel> m_storyViewModels;
        private ObservableCollection<FolderViewModel> m_folders;

        public MainWindowViewModel()
        {
            m_modified = false;
            m_filename = "";
            m_story = null;
            m_stories = new List<Story>();
            m_folders = new ObservableCollection<FolderViewModel>();
            m_storyViewModels = new ObservableCollection<StoryViewModel>();
        }

        public StoryViewModel? Story
        {
            get => m_story;
            set
            {
                m_story = value;
                OnPropertyChanged(nameof(Story));
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(Text));
                OnPropertyChanged(nameof(Folder));
            }
        }

        public string ApplicationTitle
        {
            get
            {
                var title = "Story Writer";

                if (!string.IsNullOrEmpty(m_filename))
                    title = title + ": " + Path.GetFileNameWithoutExtension(m_filename);

                if (m_modified)
                    title = title + " *";

                return title;
            }
        }

        public ObservableCollection<StoryViewModel> Stories => m_storyViewModels;

        public string Title
        {
            get
            {
                if (m_story != null)
                    return m_story.Title;

                return "untitled";
            }

            set
            {
                if (m_story != null)
                {
                    m_modified = true;
                    m_story.Title = value;
                    OnPropertyChanged(nameof(Title));
                    OnPropertyChanged(nameof(ApplicationTitle));
                }
            }
        }

        public string Text
        {
            get
            {
                if (m_story != null)
                    return m_story.Text;

                return "none";
            }

            set
            {
                if (m_story != null)
                {
                    m_modified = true;
                    m_story.Text = value;
                    OnPropertyChanged(nameof(Text));
                    OnPropertyChanged(nameof(ApplicationTitle));
                }
            }
        }

        public string Folder
        {
            get
            {
                if (m_story != null)
                    return m_story.Folder;

                return "none";
            }

            set
            {
                if (m_story != null)
                {
                    m_modified = true;
                    m_story.Folder = value;
                    OnPropertyChanged(nameof(Folder));
                    OnPropertyChanged(nameof(ApplicationTitle));
                }
            }
        }

        public string Tags
        {
            get
            {
                if (m_story != null)
                    return m_story.Tags;

                return "none";
            }

            set
            {
                if (m_story != null)
                {
                    m_modified = true;
                    m_story.Tags = value;
                    OnPropertyChanged(nameof(Tags));
                    OnPropertyChanged(nameof(ApplicationTitle));
                }
            }
        }

        public ObservableCollection<FolderViewModel> Folders => m_folders;

        public StoryViewModel SelectedStory
        {
            get => m_story!;
            set
            {
                m_story = value;
                OnPropertyChanged(nameof(Story));
                OnPropertyChanged(nameof(SelectedStory));
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(Text));
                OnPropertyChanged(nameof(Folder));
                OnPropertyChanged(nameof(Tags));
            }
        }

        public ICommand NewCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    m_modified = false;
                    m_story = null;
                    m_stories = new List<Story>();

                    Build();

                    OnPropertyChanged(nameof(ApplicationTitle));
                    OnPropertyChanged(nameof(Title));
                    OnPropertyChanged(nameof(Text));
                    OnPropertyChanged(nameof(Folders));
                });
            }
        }

        public ICommand OpenCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    var dialog = new OpenFileDialog
                    {
                        Title = "Open stories",
                        DefaultExt = ".story",
                        FileName = m_filename,
                        Filter = "Stories (*.story)|*.story"
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        m_modified = false;
                        m_filename = dialog.FileName;

                        var text = File.ReadAllText(dialog.FileName);
                        var stories = JsonSerializer.Deserialize<List<Story>>(text);

                        if (stories != null)
                            m_stories = stories;

                        Build();

                        m_story = m_storyViewModels[0];

                        OnPropertyChanged(nameof(ApplicationTitle));
                        OnPropertyChanged(nameof(Title));
                        OnPropertyChanged(nameof(Text));
                        OnPropertyChanged(nameof(Folders));
                        OnPropertyChanged(nameof(Stories));
                    }
                });
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    if (string.IsNullOrEmpty(m_filename))
                        return;


                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    var text = JsonSerializer.Serialize(m_stories, options);
                    File.WriteAllText(m_filename, text);
                    
                    m_modified = false;
                    OnPropertyChanged(nameof(ApplicationTitle));
                });
            }
        }

        public ICommand SaveAsCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    var dialog = new SaveFileDialog
                    {
                        Title = "Save Stories",
                        DefaultExt = ".story",
                        FileName = m_filename,
                        Filter = "Stories (*.story)|*.story"
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        m_filename = dialog.FileName;

                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true
                        };
                        var text = JsonSerializer.Serialize(m_stories, options);
                        File.WriteAllText(dialog.FileName, text);

                        m_modified = false;
                        OnPropertyChanged(nameof(ApplicationTitle));
                    }
                });
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    if (m_modified)
                    {
                        var query = MessageBox.Show("Do you wish to save your changes?", "Story Writer", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (query == MessageBoxResult.Yes)
                        {
                            if (string.IsNullOrEmpty(m_filename))
                            {
                                var dialog = new SaveFileDialog
                                {
                                    Title = "Save Stories",
                                    DefaultExt = ".story",
                                    FileName = m_filename,
                                    Filter = "Stories (*.story)|*.story"
                                };

                                if (dialog.ShowDialog() == true)
                                {
                                    var options = new JsonSerializerOptions
                                    {
                                        WriteIndented = true
                                    };
                                    var text = JsonSerializer.Serialize(m_stories, options);
                                    File.WriteAllText(dialog.FileName, text);
                                }
                            }
                            else
                            {
                                var options = new JsonSerializerOptions
                                {
                                    WriteIndented = true
                                };
                                var text = JsonSerializer.Serialize(m_stories, options);
                                File.WriteAllText(m_filename, text);
                            }
                        }
                    }

                    Application.Current.Shutdown(0);
                });
            }
        }

        public ICommand MDViewerCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    if (m_story == null)
                        return;

                    var mdViewerViewModel = new MDViewerWindowViewModel(m_story.Story);
                    var mdViewer = new MDViewerWindow(mdViewerViewModel);
                    mdViewer.Show();
                });
            }
        }

        private FolderViewModel? FindFolder(string name)
        {
            foreach(var folderViewModel in m_folders)
            {
                if (folderViewModel.Title == name)
                    return folderViewModel;
            }

            return null;
        }

        private void Build()
        {
            foreach(var story in m_stories)
            {
                var storyViewModel = new StoryViewModel(story);
                m_storyViewModels.Add(storyViewModel);
            }

            var view = CollectionViewSource.GetDefaultView(m_storyViewModels);
            var groupDesc = new PropertyGroupDescription("Folder");
            view.GroupDescriptions.Add(groupDesc);
        }
    }
}
