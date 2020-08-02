using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using MusicOrder.Infrastructure;
using MusicOrder.Extensions;
using MusicOrder.Models;
using MusicOrder.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MusicOrder.Managers;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicOrder
{
    public class MainViewModel : ViewModelBase, IDropTarget
    {
        #region Fields 
        private MediaPlayer _mediaPlayer;
        private MediaFileManager _mediaFileManager;
        private HashSet<string> _hashFileNames;
        #endregion 

        #region Properties 
        public ObservableCollection<FileViewModel> FileViewCollection { get; set; }

        public ICommand MenuLoadClick { get; private set; }
        public ICommand MenuSaveToClick { get; private set; }
        public ICommand PlayClick { get; private set; }
        public ICommand StopClick { get; private set; }
        public ICommand ShuffleClick { get; private set; }
        public ICommand RemoveClick { get; private set; }
        public ICommand RemoveAllClick { get; private set; }
        public ICommand FileViewItemDoubleClick { get; private set; }
        
        private FileViewModel _fileViewItem;
        public FileViewModel FileViewItem
        {
            get { return _fileViewItem; }
            set
            {
                _fileViewItem = value;
                OnPropertyChanged("FileViewItem");
            }
        }

        private bool _addOrderBeforeNameChecked;
        public bool AddOrderBeforeNameChecked
        {
            get { return _addOrderBeforeNameChecked; }
            set
            {
                _addOrderBeforeNameChecked = value;
                OnPropertyChanged("AddOrderBeforeNameChecked");
            }
        }

        private string _currentPath;
        public string CurrentPath
        {
            get { return _currentPath; }
            set
            {
                _currentPath = value;
                OnPropertyChanged("CurrentPath");
            }
        }

        private bool _showLoader;
        public bool ShowLoader
        {
            get { return _showLoader; }
            set
            {
                _showLoader = value;
                OnPropertyChanged("ShowLoader");
            }
        }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            _mediaPlayer = new MediaPlayer();
            _mediaFileManager = new MediaFileManager();
            _hashFileNames = new HashSet<string>();

            _addOrderBeforeNameChecked = true;

            FileViewCollection = new ObservableCollection<FileViewModel>();
            ShuffleClick = new DelegateCommand(new Action<object>(ShuffleClickHandler));
            PlayClick = new DelegateCommand(new Action<object>(PlayClickHandler));
            StopClick = new DelegateCommand(new Action<object>(StopClickHandler));
            RemoveClick = new DelegateCommand(new Action<object>(RemoveClickHandler));
            RemoveAllClick = new DelegateCommand(new Action<object>(RemoveAllClickHandler));
            MenuLoadClick = new DelegateCommand(new Action<object>(MenuLoadClickHandler));
            MenuSaveToClick = new DelegateCommand(new Action<object>(MenuSaveToClickHandler));
            FileViewItemDoubleClick = new DelegateCommand(new Action<object>(FileViewItemDoubleClickHandler));
        }
        #endregion
        
        #region Methods 
        public void MenuLoadClickHandler(object param)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3";
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentPath = Path.GetDirectoryName(openFileDialog.FileName);
                foreach (string fileName in openFileDialog.SafeFileNames)
                {
                    if (_hashFileNames.Contains(fileName)) continue;

                    FileViewCollection.Add(new FileViewModel(
                        name: fileName));
                    _hashFileNames.Add(fileName);
                }
            }
        }

        public void MenuSaveToClickHandler(object param)
        {
            var folderDialog = new CommonOpenFileDialog();
            folderDialog.IsFolderPicker = true;
            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ShowLoader = true;

                Task.Run(() =>
                {
                    try
                    {
                        _mediaFileManager.Save(new MediaFileManagerSettings(
                            fileViewCollection: FileViewCollection,
                            currentPath: CurrentPath,
                            newPath: folderDialog.FileName,
                            addOrderBeforeName: AddOrderBeforeNameChecked));
                    }
                    catch (MusicOrderException ex)
                    {
                        ShowErrorMessage(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessage(ex.Message);
                    }
                    finally
                    {
                        Application.Current.Dispatcher.Invoke(() => ShowLoader = false);
                    }
                });
            }
        }

        private void ShowErrorMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ShowLoader = false;
                MessageBox.Show(message);
            });
        }

        public void PlayClickHandler(object param)
        {
            if (FileViewItem == null) return;

            _mediaPlayer.Open(new Uri(Path.Combine(CurrentPath, FileViewItem.Name)));
            _mediaPlayer.Play();
        }

        public void StopClickHandler(object param)
        {
            _mediaPlayer.Stop();
        }

        public void ShuffleClickHandler(object param)
        {
            var random = new Random();
            int count = FileViewCollection.Count;
            while (count > 1)
            {
                int randomNum = random.Next(count--);
                var temp = FileViewCollection[count];
                FileViewCollection[count] = FileViewCollection[randomNum];
                FileViewCollection[randomNum] = temp;
            }
        }
        
        public void RemoveClickHandler(object param)
        {
            if (FileViewItem == null) return;

            _hashFileNames.Remove(FileViewItem.Name);
            FileViewCollection.Remove(FileViewItem);
            if (FileViewCollection.Count > 0)
                CurrentPath = string.Empty;
        }
        
        public void RemoveAllClickHandler(object param)
        {
            _mediaPlayer.Stop();
            _hashFileNames.Clear();
            FileViewCollection.Clear();
            CurrentPath = string.Empty;
        }

        public void FileViewItemDoubleClickHandler(object param)
        {
            PlayClickHandler(param);
        }
        
        #region Drag'n'Drop 
        public void DragOver(IDropInfo dropInfo)
        {
            //if (dropInfo.Data is FileViewModel)
            //{
            //    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            //    dropInfo.Effects = DragDropEffects.Move;
            //}
        }

        public void Drop(IDropInfo dropInfo)
        {
            //var files = (FileViewModel)dropInfo.Data;
            //((IList<FileViewModel>)dropInfo.DragInfo.SourceCollection).Remove(files);
        }
        #endregion

        #endregion
    }
}
