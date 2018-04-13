using System;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Monika
{
    public class Manifest
    {
        public string Name { get; set; }
        public Dictionary<string, string> Files { get; set; }
        public List<string> Responses { get; set; }

        public Manifest()
        {
            Name = String.Empty;
            Files = new Dictionary<string, string>();
            Responses = new List<string>();
        }
    }
}
