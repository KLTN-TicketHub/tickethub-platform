using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Catalog.Domain.Entities
{
    public class EventCategory : SoftDeleteEntity, IAggregateRoot
    {
        public string CategoryCode { get; set; }

        public string CategoryName { get; set; }

        public string Slug { get; set; }

        public string? Description { get; set; }

        public CatalogStatus Status { get; set; }

        public byte[] RowVersion { get; set; } = default!;

        private readonly List<Event> _events = new List<Event>();
        public IReadOnlyCollection<Event> Events => _events.AsReadOnly();

        public EventCategory(string categoryName, string? description)
        {
            CategoryName = categoryName;
            Description = description;
        }

        public static string NormalizeCategoryCode(string name, int maxLen = 40)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;

            string normalized = name.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (var ch in normalized)
            {
                UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (cat != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            string withoutDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

            string replaced = Regex.Replace(withoutDiacritics, @"[^A-Za-z0-9]+", "-");

            string trimmed = replaced.Trim('-').ToUpperInvariant();
            trimmed = Regex.Replace(trimmed, @"[^A-Z0-9\-]", string.Empty);

            if (trimmed.Length > maxLen) trimmed = trimmed.Substring(0, maxLen).Trim('-');

            return trimmed;
        }

        public static string GenerateSlugFromCategoryCode(string categoryCode)
        {
            if (string.IsNullOrWhiteSpace(categoryCode))
                return string.Empty;

            return categoryCode.Trim().ToLowerInvariant();
        }

        public void SetCategoryCode(string categoryCode)
        {
            CategoryCode = categoryCode;
            Slug = GenerateSlugFromCategoryCode(categoryCode);
        }
    }
}
