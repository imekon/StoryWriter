using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace StoryWriter
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private string m_filename;
        private Story? m_story;
        private ObservableCollection<Story> m_stories;

        public MainWindowViewModel()
        {
            m_filename = "";
            m_story = null;
            m_stories = new ObservableCollection<Story>();
        }

        public Story? Story
        {
            get => m_story;
            set
            {
                m_story = value;
                OnPropertyChanged(nameof(Story));
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(Text));
            }
        }

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
                    m_story.Title = value;
                    OnPropertyChanged(nameof(Title));
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
                    m_story.Text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        public ObservableCollection<Story> Stories => m_stories;

        public ICommand NewCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    m_story = null;
                    m_stories = new ObservableCollection<Story>();

                    OnPropertyChanged(nameof(Title));
                    OnPropertyChanged(nameof(Text));
                    OnPropertyChanged(nameof(Stories));
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
                        m_filename = dialog.FileName;

                        var text = File.ReadAllText(dialog.FileName);
                        var stories = JsonSerializer.Deserialize<List<Story>>(text);

                        if (stories != null)
                            m_stories = new ObservableCollection<Story>(stories);

                        m_story = m_stories[0];

                        OnPropertyChanged(nameof(Title));
                        OnPropertyChanged(nameof(Text));
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
                    Application.Current.Shutdown(0);
                });
            }
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        internal void SetStory(Story story)
        {
            Story = story;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
