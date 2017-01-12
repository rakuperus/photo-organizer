using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace photo_organizer.Model
{
    public class PhotoMover : INotifyPropertyChanged
    {

        #region Observable properties

        public event PropertyChangedEventHandler PropertyChanged;

        private StorageFile lastImageLocation = null;

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

        private string _duration;
        public string Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Duration"));
            }
        }

        private string _pps;
        public string PPS
        {
            get { return _pps; }
            set
            {
                _pps = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PPS"));
            }
        }

        private string _ppslm;
        public string PPSLM
        {
            get { return _ppslm; }
            set
            {
                _ppslm = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PPSLM"));
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

            // await ProcessFiles(sourceFolder, destinationFolder, moveFiles);

            return true;
        }
    }
}
