using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace MongoToRedis
{
    public class ConfigurationLoader
    {
        public static TSettings LoadSettings<TSettings>(string filePath, string sectionName)
            where TSettings : new()
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found at: {Path.GetFullPath(filePath)}");
                throw new FileNotFoundException("Configuration file not found", filePath);
            }

            try
            {
                Console.WriteLine($"File found at: {Path.GetFullPath(filePath)}");
                var contents = File.ReadAllText(filePath);
                var deserializer = new DeserializerBuilder().Build();
                var yamlObject = deserializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(contents);

                if (yamlObject.ContainsKey(sectionName))
                {
                    var settings = new TSettings();
                    var properties = typeof(TSettings).GetProperties();

                    foreach (var prop in properties)
                    {
                        var value = yamlObject[sectionName].GetValueOrDefault(prop.Name);
                        if (value != null)
                        {
                            try
                            {
                                var propertyType = prop.PropertyType;
                                var convertedValue = Convert.ChangeType(value, propertyType);
                                prop.SetValue(settings, convertedValue);
                            }
                            catch (InvalidCastException)
                            {
                                Console.WriteLine($"Unable to convert value of property {prop.Name} to type {prop.PropertyType}");
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine($"Invalid format for property {prop.Name} of type {prop.PropertyType}");
                            }
                        }
                    }

                    return settings;
                }
                else
                {
                    throw new ArgumentException($"{sectionName} section not found in the configuration file.");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                throw;
            }
            catch (YamlException ex)
            {
                Console.WriteLine($"Error deserializing YAML: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
    }
}