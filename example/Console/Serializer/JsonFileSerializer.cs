using System.Text.Json;

namespace XboxAuthNetConsole.Serializer
{
    public class JsonFileSerializer<T> : ISerializer<T>
    {
        private readonly string _filePath;

        public JsonFileSerializer(string filePath)
        {
            this._filePath = filePath;
        }

        public async Task<T?> Load()
        {
            Console.WriteLine(Path.GetFullPath(_filePath));
            if (File.Exists(_filePath))
            {
                await using var fs = File.OpenRead(_filePath);
                return await JsonSerializer.DeserializeAsync<T>(fs, JsonHelper.JsonSerializerOptions);
            }
            else
            {
                return default(T);
            }
        }

        public async Task Save(T? obj)
        {
            var dirPath = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dirPath))
                Directory.CreateDirectory(dirPath);
                
            await using var fs = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(fs, obj, JsonHelper.JsonSerializerOptions);
        }
    }
}