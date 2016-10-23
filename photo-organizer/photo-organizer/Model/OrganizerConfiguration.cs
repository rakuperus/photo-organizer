using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace photo_organizer.Model
{
    public struct OrganizerConfiguration
    {
        public StorageFolder Source { get; set; }
        public StorageFolder Destination { get; set; }
        public bool MoveFiles { get; set; }
    }
}
