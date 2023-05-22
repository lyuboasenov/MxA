using System;
using System.Collections.Generic;
using System.Text;

namespace PortableLoadCell.Models
{
    public class Training {
        public string Id { get; set; }
        public string Name { get; set; }

        public uint PrepTime { get; set; }
        public uint WorkTime { get; set; }
        public uint RestTime { get; set; }
        public uint RestBwSetsTime { get; set; }
        public uint CooldownTime { get; set; }
        
        public uint Reps { get; set; }
        public uint Sets { get; set; }
    }
}
