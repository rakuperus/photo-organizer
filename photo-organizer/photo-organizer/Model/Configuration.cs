using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace photo_organizer.Model
{
    public class Configuration
    {
        public StorageFolder SourceFolder { get; set; }
        public StorageFolder DestinationFolder { get; set; }
        public bool MoveFilesToDestination { get; set; }
        public string DestinationPathPattern { get; set; }
    }
}
