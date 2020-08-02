using MusicOrder.Extensions;
using MusicOrder.Infrastructure;
using MusicOrder.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MusicOrder.Managers
{
    public class MediaFileManager
    {
        public void Save(MediaFileManagerSettings settings)
        {
            if (settings.FileViewCollection.Count == 0) 
                throw new MusicOrderException("Media files haven't chosen");
            if (settings.CurrentPath.IsNothing())
                throw new MusicOrderException("Current directory doesn't exist");

            var savedFiles = SaveFiles(settings);
            SaveDescription(savedFiles);
        }

        private List<SaveFileDescription> SaveFiles(MediaFileManagerSettings settings)
        {
            uint currentIndex = 1;
            var savedFiles = new List<SaveFileDescription>();
            string newPath = settings.NewPath.IsNothing()
                ? settings.CurrentPath : settings.NewPath;
            foreach (var fileView in settings.FileViewCollection)
            {
                string fullPath = Path.Combine(settings.CurrentPath, fileView.Name);
                var bytes = File.ReadAllBytes(fullPath);
                // Set by settings
                string newFileName = settings.AddOrderBeforeName
                    ? $"{currentIndex}. {fileView.Name}" : fileView.Name;
                // Save file to new directory
                string newFullPath = @$"{newPath}\{newFileName}";
                File.WriteAllBytes(newFullPath, bytes);
                savedFiles.Add(new SaveFileDescription(
                    index: currentIndex,
                    name: newFullPath));

                currentIndex++;
            }

            return savedFiles;
        }

        private void SaveDescription(List<SaveFileDescription> savedFiles)
        {
            foreach (var savedFile in savedFiles)
            {
                string textIndex = savedFile.Index.ToString();
                using (var file = TagLib.File.Create(savedFile.Name))
                {
                    // Change tags
                    file.Tag.Track = savedFile.Index;
                    file.Tag.AmazonId = textIndex;
                    file.Tag.MusicIpId = textIndex;

                    file.Save();
                }
            }
        }
    }
}
