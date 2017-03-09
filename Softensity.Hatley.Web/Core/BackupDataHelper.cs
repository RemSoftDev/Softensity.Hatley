using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CsvHelper;
using Ionic.Zip;

namespace Softensity.Hatley.Web.Core
{
    public class BackupDataHelper
    {
        private ConcurrentDictionary<string, byte[]> files = new ConcurrentDictionary<string, byte[]>();

        public void Add(string fileName, IEnumerable records)
        {
            byte[] byteArray;
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.WriteRecords(records);
                    }
                }
                byteArray = memoryStream.ToArray();
            }
            files.AddOrUpdate(fileName, byteArray, (s, bytes) => bytes);
        }

        public byte[] Save()
        {
            byte[] byteArray;
            using (ZipFile zip = new ZipFile())
            {
                foreach (var file in files)
                {
                    zip.AddEntry(file.Key, file.Value);
                }
                using (var memoryStream = new MemoryStream())
                {
                    zip.Save(memoryStream);
                    byteArray = memoryStream.ToArray();
                }
            }
            return byteArray;
        }
    }
}