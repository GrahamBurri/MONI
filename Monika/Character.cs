using System;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Monika
{
    public class ManifestNextGen
    {
        public string Name { get; set; }
        public Dictionary<string, string> Files { get; set; }
        public List<string> Responses { get; set; }

        public ManifestNextGen()
        {
            Name = String.Empty;
            Files = new Dictionary<string, string>();
            Responses = new List<string>();
        }
    }
}
