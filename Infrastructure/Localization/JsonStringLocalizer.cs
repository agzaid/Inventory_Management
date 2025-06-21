using Microsoft.Extensions.Localization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Localization
{

    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly string _filePath;
        private readonly ConcurrentDictionary<string, Dictionary<string, string>> _localizations;

        public JsonStringLocalizer(string filePath)
        {
            _filePath = filePath;
            _localizations = new ConcurrentDictionary<string, Dictionary<string, string>>();
            LoadJson();
        }

        private void LoadJson()
        {
            if (!File.Exists(_filePath)) return;

            var json = File.ReadAllText(_filePath);
            var allData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);

            if (allData != null)
            {
                foreach (var cultureEntry in allData)
                {
                    _localizations[cultureEntry.Key] = cultureEntry.Value;
                }
            }
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name, CultureInfo.CurrentUICulture.Name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments] =>
            new(name, string.Format(this[name].Value, arguments));

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var culture = CultureInfo.CurrentUICulture.Name;
            if (_localizations.TryGetValue(culture, out var dict))
            {
                return dict.Select(kv => new LocalizedString(kv.Key, kv.Value, false));
            }
            return Enumerable.Empty<LocalizedString>();
        }

        private string? GetString(string name, string culture)
        {
            if (_localizations.TryGetValue(culture, out var dict) && dict.TryGetValue(name, out var value))
                return value;

            return null;
        }
    }
}
