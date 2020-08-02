using MusicOrder.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MusicOrder.Models
{
    public class MediaFileManagerSettings
    {
        public MediaFileManagerSettings(
            ObservableCollection<FileViewModel> fileViewCollection, 
            string currentPath, 
            string newPath,
            bool addOrderBeforeName)
        {
            FileViewCollection = fileViewCollection ?? new ObservableCollection<FileViewModel>();
            CurrentPath = currentPath;
            NewPath = newPath;
            AddOrderBeforeName = addOrderBeforeName;
        }

        /// <summary>
        /// Loaded file descriptions from view model
        /// </summary>
        public ObservableCollection<FileViewModel> FileViewCollection { get; private set; }
        /// <summary>
        /// Loaded files path
        /// </summary>
        public string CurrentPath { get; private set; }
        /// <summary>
        /// Path for save files
        /// </summary>
        public string NewPath { get; private set; }
        /// <summary>
        /// Add order number before file name
        /// </summary>
        public bool AddOrderBeforeName { get; private set; }
    }
}
