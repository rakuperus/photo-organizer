using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.Storage;

namespace photo_organizer.Model
{
    public class PhotoMover : INotifyPropertyChanged
    {
        private const string SINKHOLE_FOLDER_NAME = "\\unknown";

        public event PropertyChangedEventHandler PropertyChanged;

        #region Observable properties

        public StorageFile LastImageLocation = null;

        private string _currentImage;
        public string CurrentImage
        {
            get { return _currentImage; }
            set
            {
                _currentImage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentImage"));
            }
        }

        private int _photoCount;
        public int PhotoCount
        {
            get { return _photoCount; }
            set
            {
                _photoCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PhotoCount"));
            }
        }

        private int _photoCountLM;
        public int PhotoCountLM
        {
            get { return _photoCountLM; }
            set
            {
                _photoCountLM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PhotoCountLM"));
            }
        }

        private int _folderCount;
        public int FolderCount
        {
            get { return _folderCount; }
            set
            {
                _folderCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FolderCount"));
            }
        }

        private string _currentFolderName;
        public string CurrentFolderName
        {
            get { return _currentFolderName; }
            set
            {
                _currentFolderName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentFolderName"));
            }
        }

        private string _destinationFolderName;
        public string DestinationFolderName
        {
            get { return _destinationFolderName; }
            set
            {
                _destinationFolderName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DestinationFolderName"));
            }
        }

        private string _progressStream;
        public string ProgressStream
        {
            get { return _progressStream; }
            set
            {
                _progressStream = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProgressStream"));
            }
        }

        private int _duplicates;
        public int Duplicates
        {
            get { return _duplicates; }
            set
            {
                _duplicates = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Duplicates"));
            }
        }

        private int _errorCount;
        public int ErrorCount
        {
            get { return _errorCount; }
            set
            {
                _errorCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ErrorCount"));
            }
        }

        #endregion

        public bool CancelExecute { get; set; }

        public PhotoMover()
        {
        }

        private void AddToStream (string message)
        {

        }

        async public Task<bool> Execute(StorageFolder sourceFolder, StorageFolder destinationFolder, bool moveFiles)
        {
            if (null == sourceFolder) throw new ArgumentNullException("sourceFolder");
            if (null == destinationFolder) throw new ArgumentNullException("destinationFolder");

            return await ProcessFolder(sourceFolder, destinationFolder, moveFiles);
        }

        async private Task<bool> ProcessFolder(StorageFolder sourceFolder, StorageFolder destinationFolder, bool moveFiles)
        {
            DestinationFolderName = destinationFolder.Path;

            var subfolders = await sourceFolder.GetFoldersAsync();
            foreach (var sub in subfolders)
            {
                AddToStream($"Processing folder {sub.Name}");
                if (!await ProcessFolder(sub, destinationFolder, moveFiles)) return false;

                FolderCount++;
                if (CancelExecute) return false;
            }

            CurrentFolderName = sourceFolder.Name;

            await ProcessFiles(sourceFolder, destinationFolder, moveFiles);

            return true;
        }

        async private Task<bool> ProcessFiles(StorageFolder sourceFolder, StorageFolder destinationRootFolder, bool moveFiles)
        {
            AddToStream($"Retrieving files from folder {sourceFolder.Name}");
            var files = await sourceFolder.GetFilesAsync();
            foreach (var file in files)
            {
                CurrentImage = file.DisplayName;
                AddToStream($"Processing file {file.Name} of type {file.ContentType}");

                if (string.Compare(file.ContentType, "image/jpeg", true) == 0)
                {
                    await ProcessImageFile(file, destinationRootFolder, moveFiles);
                }
                else if (string.Compare(file.ContentType, "video/avi", true) == 0 || string.Compare(file.ContentType, "video/mp4", true) == 0 || string.Compare(file.ContentType, "video/quicktime", true) == 0 || file.ContentType.StartsWith("video/"))
                {
                    await ProcessVideoFile(file, sourceFolder, destinationRootFolder, moveFiles);
                }

                PhotoCount++;

                if (CancelExecute) return false;
            }

            return true;
        }

        async private Task ProcessVideoFile(StorageFile file, StorageFolder sourceFolder, StorageFolder destinationRootFolder, bool moveFiles)
        {
            StringBuilder sb = new StringBuilder();
            if (file.Properties != null)
            {

                var fileProperties = file.Properties;
                var videoProperties = await fileProperties.GetVideoPropertiesAsync();
                if (null != videoProperties)
                {
                    if (videoProperties.Year > 0)
                    {
                        sb.Append($"\\{ videoProperties.Year }\\Video");
                    }
                    else
                    {
                        VideoPathNameAppend(sourceFolder, sb);
                    }

                    if (null != videoProperties.Latitude && null != videoProperties.Longitude)
                    {
                        Geopoint p = new Geopoint(new BasicGeoposition
                        {
                            Latitude = videoProperties.Latitude.Value,
                            Longitude = videoProperties.Longitude.Value
                        });

                        MapLocationFinderResult location = await MapLocationFinder.FindLocationsAtAsync(p);
                        var l = location.Locations.FirstOrDefault<MapLocation>();
                        if (null != l)
                        {
                            if (!string.IsNullOrWhiteSpace(l.DisplayName))
                            {
                                sb.Append($"\\{l.DisplayName}");
                            }
                            else
                            {
                                if (null != l.Address && null != l.Address.Town)
                                {
                                    sb.Append($"\\{l.Address.Town}");
                                }
                            }
                        }
                    }

                    MoveOrCopyFileTo(file, destinationRootFolder, sb.ToString(), moveFiles);
                    return;
                }
            }

            // missing information, we'll use current path information instead
            VideoPathNameAppend(sourceFolder, sb);
            MoveOrCopyFileTo(file, destinationRootFolder, sb.ToString(), moveFiles);
        }

        private static void VideoPathNameAppend(StorageFolder sourceFolder, StringBuilder sb)
        {
            var splitName = sourceFolder.Name.Split('-');
            if (splitName.Length == 2)
            {
                var year = splitName[0];
                var month = splitName[1];

                sb.Append($"\\{year}");
                sb.Append($"\\{month:00}-{year:0000}");
                sb.Append("\\Video");
            }
            else
            {
                sb.Append(sourceFolder.Name.Replace('-', '\\'));
                sb.Append("\\Video");
            }
        }

        async private Task ProcessImageFile(StorageFile file, StorageFolder destinationRootFolder, bool moveFiles)
        {
            if (null != file.Properties)
            {
                var fileProperties = file.Properties;
                var imageProperties = await fileProperties.GetImagePropertiesAsync();
                if (null != imageProperties)
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append($"\\{imageProperties.DateTaken.Year:0000}");
                    sb.Append($"\\{imageProperties.DateTaken.Month:00}-{imageProperties.DateTaken.Year:0000}");

                    if (null != imageProperties.Latitude && null != imageProperties.Longitude)
                    {
                        Geopoint p = new Geopoint(new BasicGeoposition
                        {
                            Latitude = imageProperties.Latitude.Value,
                            Longitude = imageProperties.Longitude.Value
                        });

                        MapLocationFinderResult location = await MapLocationFinder.FindLocationsAtAsync(p);
                        var l = location.Locations.FirstOrDefault<MapLocation>();
                        if (null != l)
                        {
                            if (!string.IsNullOrWhiteSpace(l.DisplayName))
                            {
                                sb.Append($"\\{l.DisplayName}");
                            }
                            else
                            {
                                if (null != l.Address && null != l.Address.Town)
                                {
                                    sb.Append($"\\{l.Address.Town}");
                                }
                            }
                        }
                    }
                    else if ( !string.IsNullOrWhiteSpace(imageProperties.CameraModel) )
                    {
                        sb.Append($"\\{imageProperties.CameraManufacturer} -  {imageProperties.CameraModel}");
                    }

                    MoveOrCopyFileTo(file, destinationRootFolder, sb.ToString(), moveFiles);

                    LastImageLocation = file;
                    return;
                }
            }

            // MoveOrCopyFileTo(file, destinationRootFolder, SINKHOLE_FOLDER_NAME, moveFiles);
        }

        async private Task<StorageFolder> CreateFolderTreeAsync(StorageFolder root, string newPath)
        {
            StorageFolder f = root;
            if (!string.IsNullOrWhiteSpace(newPath))
            {
                var pathElements = newPath.Split(new char[] { '\\' });
                foreach (var e in pathElements)
                {
                    if (!string.IsNullOrWhiteSpace(e))
                        f = await f.CreateFolderAsync(e, CreationCollisionOption.OpenIfExists);
                }
            }

            return f;
        }

        async private void MoveOrCopyFileTo(StorageFile file, StorageFolder folder, string destinationPath, bool moveFiles)
        {
            var destination = await CreateFolderTreeAsync(folder, destinationPath);
            if (!await FileExistsAsync(destination, file.Name))
            {
                try
                {
                    if (moveFiles)
                    {
                        await file.MoveAsync(destination, file.Name, NameCollisionOption.GenerateUniqueName);
                    }
                    else
                    {
                        await file.CopyAsync(destination, file.Name, NameCollisionOption.GenerateUniqueName);
                    }

                    AddToStream(file.DisplayName);
                }
                catch (Exception e)
                {
                    AddToStream($"Error with message {e.Message}");
                    ErrorCount++;
                }
            }
            else
            {
                Duplicates += 1;
            }
        }

        async private Task<bool> FileExistsAsync(StorageFolder destination, string name)
        {
            bool exists = true;
            try
            {
                await destination.GetFileAsync(name);
            }
            catch (System.IO.FileNotFoundException)
            {
                exists = false;
            }
            return exists;
        }
    }
}
