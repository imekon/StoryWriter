using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryWriter
{
    internal class ProcessWindowViewModel : ViewModelBase
    {
        private string processPath;

        public ProcessWindowViewModel()
        {
            processPath = @"F:\stories";
        }

        public string ProcessPath
        {
            get => processPath;
            set
            {
                processPath = value;
                OnPropertyChanged(nameof(ProcessPath));
            }
        }
    }
}
