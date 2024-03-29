﻿using System.Windows;

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
    }
}
