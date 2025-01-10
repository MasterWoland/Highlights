using System;
using Sirenix.OdinInspector;

namespace Aion.Highlights.Data
{
    [Serializable]
    public class DataFileInfo
    {
        private readonly string _filePath;
        [ShowInInspector] public string FilePath { get => _filePath; }
        private readonly string _fileName;
        [ShowInInspector] public string FileName { get => _fileName; }

        public DataFileInfo(string filePath, string fileName)
        {
            _filePath = filePath;
            _fileName = fileName;
        }
    }
}