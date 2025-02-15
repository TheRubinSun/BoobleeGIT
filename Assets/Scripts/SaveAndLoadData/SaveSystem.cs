using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using UnityEngine;

public class SaveSystem
{
    private static string savePath = Application.persistentDataPath;

    public static async Task SaveDataAsync <T>(T data, string fileName)
    {
        string fullPath = Path.Combine(savePath, fileName);
        try
        {
            // Добавляем TypeNameHandling для сохранения информации о типах
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(data, settings);
            await File.WriteAllTextAsync(fullPath, json);
            Debug.Log($"Данные сохранены: {fileName}");
        }
        catch(Exception e)
        {
            Debug.LogError($"Ошибка сохранения {fileName}: {e.Message}");
        }
    }
    public static async Task <T> LoadDataAsync<T>(string fileName) where T : new()
    {
        string fullPath = Path.Combine(savePath,fileName);
        if(File.Exists(fullPath))
        {
            try
            {
                string json = await File.ReadAllTextAsync(fullPath);
                // Добавляем TypeNameHandling для восстановления информации о типах
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };
                
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch(Exception e)
            {
                Debug.LogError($"Ошибка загрузки {fileName}: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"Файл {fileName} отсутствует. Создаём новый.");
        }
        return new T();  // Если файла нет, возвращаем новый объект
    }


}
