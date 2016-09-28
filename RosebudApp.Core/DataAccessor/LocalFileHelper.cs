using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RosebudAppCore.DataAccessor
{
    public static class LocalFileHelper
    {
        public static async Task<T> GetFromFile<T>(string filePath)
        {
            if (!File.Exists(filePath))
                return default(T);

            string json;
            using (StreamReader reader = File.OpenText(filePath))
            {
                json = await reader.ReadToEndAsync();
            }

            T item = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(json));
            
            return item;
        }

        public static async Task SaveToFile(string filePath, object item)
        {
            string json = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(item));
            using (StreamWriter writer = File.CreateText(filePath))
            {
                await writer.WriteAsync(json);
            }
        }
    }
}
