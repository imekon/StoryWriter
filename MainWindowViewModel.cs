using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using MoonSharp.Interpreter;

namespace StoryWriter
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private bool m_modified;
        private string m_filename;
        private StoryViewModel? m_story;
        private List<Story> m_stories;
        private string m_statusText;
        private byte[]? m_key;
        private KeySetting m_keySetting;
        private ObservableCollection<StoryViewModel> m_storyViewModels;
        private ObservableCollection<FolderViewModel> m_folders;

        private Script m_script;

        private static MainWindowViewModel? m_instance;

        public MainWindowViewModel()
        {
            var appSettings = ConfigurationManager.AppSettings;

            var keyed = appSettings["Keyed"];
            switch(keyed)
            {
                case "None":
                    m_keySetting = KeySetting.None;
                    break;

                case "OnLoad":
                    m_keySetting = KeySetting.OnLoad;
                    break;

                case "OnSave":
                    m_keySetting = KeySetting.OnSave;
                    break;

                default:
                    m_keySetting = KeySetting.All;
                    break;
            }

            var keyFile = appSettings["KeyFile"];
            m_key = null;
            if (keyFile != null)
                m_key = File.ReadAllBytes(keyFile);

            m_instance = this;

            m_modified = false;
            m_filename = "";
            m_story = null;
            m_stories = new List<Story>();
            m_folders = new ObservableCollection<FolderViewModel>();
            m_storyViewModels = new ObservableCollection<StoryViewModel>();

            m_script = new Script();

            UserData.RegisterType<Story>();

            m_script.Globals["GetStoryCount"] = (Func<int>)GetStoryCount;
            m_script.Globals["GetStory"] = (Func<int, Story?>)GetStory;

            m_script.Options.DebugPrint = s => Print(s);

            m_statusText = "";
        }

        #region Parameters
        public static MainWindowViewModel? Instance => m_instance;

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

        public string StatusText => m_statusText;
        #endregion

        #region Commands
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
                        LoadStories(dialog.FileName);
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

                    SaveStories(m_filename);                    
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
                        SaveStories(m_filename);
                        m_modified = false;
                        OnPropertyChanged(nameof(ApplicationTitle));
                    }
                });
            }
        }

        public ICommand ProcessCommand
        {
            get
            {
                return new DelegateCommand((o) => 
                {
                    var processWindow = new ProcessWindow();
                    processWindow.Show();
                });
            }
        }

        public ICommand GenerateCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    var dialog = new SaveFileDialog
                    {
                        Title = "Generate Key",
                        DefaultExt = ".key",
                        Filter = "Key (*.key)|*.key"
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        var random = new Random();
                        var bytes = new byte[32];
                        random.NextBytes(bytes);
                        File.WriteAllBytes(dialog.FileName, bytes);
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

        public ICommand AddStoryCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    var dialog = new CreateStoryWindow();

                    if (m_story != null)
                    {
                        dialog.StoryTitle = m_story.Title;
                        dialog.Folder = m_story.Folder;
                        dialog.Tags = m_story.Tags;
                    }

                    if (dialog.ShowDialog() == true)
                    {
                        if (m_story != null)
                        {
                            var newStory = new Story { Folder = dialog.Folder, Title = dialog.StoryTitle, Tags = dialog.Tags };
                            var newStoryViewModel = new StoryViewModel(newStory);
                            
                            var index = m_stories.IndexOf(m_story.Story);
                            m_stories.Insert(index + 1, newStory);

                            m_storyViewModels.Insert(index + 1, newStoryViewModel);
                            m_modified = true;
                            OnPropertyChanged(nameof(ApplicationTitle));
                        }
                        else
                        {
                            var newStory = new Story { Folder = dialog.Folder, Title = dialog.StoryTitle, Tags = dialog.Tags };
                            var newStoryViewModel = new StoryViewModel(newStory);

                            m_stories.Add(newStory);
                            m_storyViewModels.Add(newStoryViewModel);
                            m_modified = true;
                            OnPropertyChanged(nameof(ApplicationTitle));
                        }
                    }
                });
            }
        }

        public ICommand DuplicateStoryCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    var dialog = new CreateStoryWindow();

                    if (m_story != null)
                    {
                        dialog.StoryTitle = m_story.Title;
                        dialog.Folder = m_story.Folder;
                        dialog.Tags = m_story.Tags;
                    }

                    if (dialog.ShowDialog() == true)
                    {
                        if (m_story != null)
                        {
                            var newStory = new Story { Folder = dialog.Folder, Title = dialog.StoryTitle, Tags = dialog.Tags, Text = m_story.Text };
                            var newStoryViewModel = new StoryViewModel(newStory);

                            var index = m_stories.IndexOf(m_story.Story);
                            m_stories.Insert(index + 1, newStory);

                            m_storyViewModels.Insert(index + 1, newStoryViewModel);
                            m_modified = true;
                            OnPropertyChanged(nameof(ApplicationTitle));
                        }
                    }
                });
            }
        }

        public ICommand RemoveStoryCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    if (m_story == null)
                    {
                        MessageBox.Show("No story selected to remove", "Story Writer", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var story = m_story;
                    m_story = null;

                    m_storyViewModels.Remove(story);
                    m_stories.Remove(story.Story);
                    m_modified = true;
                    OnPropertyChanged(nameof(ApplicationTitle));
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

        public ICommand RunScriptCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    var dialog = new OpenFileDialog
                    {
                        Title = "Run script",
                        DefaultExt = ".lua",
                        Filter = "Lua scripts (*.lua)|*.lua"
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        try
                        {
                            m_script.DoFile(dialog.FileName);
                        }
                        catch(InternalErrorException e)
                        {
                            MessageBox.Show($"Error running script: {e.Message}");
                        }
                        catch(SyntaxErrorException e)
                        {
                            MessageBox.Show($"Error running script: {e.Message}");
                        }
                        catch(ScriptRuntimeException e)
                        {
                            MessageBox.Show($"Error running script: {e.DecoratedMessage}");
                        }
                    }
                });
            }
        }

        #endregion

        #region Lua functions

        private static int GetStoryCount()
        {
            if (m_instance == null)
                return 0;

            return m_instance.m_stories.Count;
        }

        private static Story? GetStory(int index)
        {
            if (m_instance == null)
                return null;

            return m_instance.m_stories[index];
        }

        private static void Print(string text)
        {
            Trace.WriteLine(text);

            if (m_instance != null)
                m_instance.AddStatusText(text);
        }

        #endregion

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

        private void AddStatusText(string text)
        {
            var updated = new StringBuilder(m_statusText);
            updated.AppendLine(text);
            m_statusText = updated.ToString();
            OnPropertyChanged(nameof(StatusText));
        }

        public void AddStory(Story story)
        {
            var newStoryViewModel = new StoryViewModel(story);
            m_stories.Add(story);

            m_storyViewModels.Add(newStoryViewModel);
            m_modified = true;
            OnPropertyChanged(nameof(ApplicationTitle));
        }

        private void LoadStories(string filename)
        {
            if (m_keySetting == KeySetting.OnLoad || m_keySetting == KeySetting.All)
            {
                if (m_key == null)
                {
                    MessageBox.Show("Key not set", "Story Writer", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (FileStream readFileStream = new(filename, FileMode.Open))
                {
                    using (Aes aes = Aes.Create())
                    {
                        byte[] iv = new byte[aes.IV.Length];
                        int numBytesToRead = aes.IV.Length;
                        int numBytesRead = 0;
                        while (numBytesToRead > 0)
                        {
                            int n = readFileStream.Read(iv, numBytesRead, numBytesToRead);
                            if (n == 0) break;

                            numBytesRead += n;
                            numBytesToRead -= n;
                        }

                        using (CryptoStream cryptoStream = new(
                        readFileStream,
                           aes.CreateDecryptor(m_key, iv),
                           CryptoStreamMode.Read))
                        {
                            using (StreamReader decryptReader = new(cryptoStream))
                            {
                                string text = decryptReader.ReadToEnd();
                                var stories = JsonSerializer.Deserialize<List<Story>>(text);

                                if (stories != null)
                                    m_stories = stories;
                            }
                        }
                    }
                }

            }
            else
            {
                var text = File.ReadAllText(filename);
                var stories = JsonSerializer.Deserialize<List<Story>>(text);

                if (stories != null)
                    m_stories = stories;
            }
        }

        private void SaveStories(string filename)
        {
            if (m_keySetting == KeySetting.OnSave || m_keySetting == KeySetting.All)
            {
                if (m_key == null)
                {
                    MessageBox.Show("Key not set", "Story Writer", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var text = JsonSerializer.Serialize(m_stories, options);

                using (FileStream writeFileStream = new(filename, FileMode.OpenOrCreate))
                {
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = m_key;

                        byte[] iv = aes.IV;
                        writeFileStream.Write(iv, 0, iv.Length);

                        using (CryptoStream cryptoStream = new(writeFileStream,
                            aes.CreateEncryptor(),
                            CryptoStreamMode.Write))
                        {
                            using (StreamWriter encryptWriter = new(cryptoStream))
                            {
                                encryptWriter.Write(text);
                            }
                        }
                    }
                }
            }
            else
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var text = JsonSerializer.Serialize(m_stories, options);
                File.WriteAllText(filename, text);
            }
        }

    }
}
