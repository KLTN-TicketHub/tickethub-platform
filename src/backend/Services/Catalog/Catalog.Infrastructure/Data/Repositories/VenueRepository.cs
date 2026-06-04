using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class VenueRepository : BaseRepository<Venue, CatalogDbContext>, IVenueRepository
    {
        public VenueRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<string> GenerateNextVenueCodeAsync(string venueName, CancellationToken cancellation = default)
        {
            string baseCode = Venue.NormalizeVenueCode(venueName);
            if (string.IsNullOrWhiteSpace(baseCode))
                throw new ArgumentException("Venue name is invalid.", nameof(venueName));

            List<string> candidates = await _dbContext.Set<Venue>()
                .IgnoreQueryFilters()
                .Where(v => EF.Functions.Like(v.VenueCode, baseCode + "%"))
                .Select(v => v.VenueCode)
                .ToListAsync(cancellation);

            int maxSuffix = -1;
            int maxDigits = 0;

            foreach (var code in candidates)
            {
                if (string.Equals(code, baseCode, StringComparison.OrdinalIgnoreCase))
                {
                    maxSuffix = Math.Max(maxSuffix, 0);
                    continue;
                }

                if (code.Length <= baseCode.Length + 1)
                    continue;

                if (code[baseCode.Length] != '-')
                    continue;

                string suffixPart = code.Substring(baseCode.Length + 1);
                if (suffixPart.Length == 0 || !Regex.IsMatch(suffixPart, @"^[0-9]+$"))
                    continue;

                if (!int.TryParse(suffixPart, out int suffix))
                    continue;

                maxSuffix = Math.Max(maxSuffix, suffix);
                maxDigits = Math.Max(maxDigits, suffixPart.Length);
            }

            if (maxSuffix == -1)
                return baseCode;

            int nextSuffix = maxSuffix + 1;

            if (maxSuffix == 0 && maxDigits == 0)
                return $"{baseCode}-{nextSuffix}";

            string paddedSuffix = maxDigits > 0
                ? nextSuffix.ToString().PadLeft(maxDigits, '0')
                : nextSuffix.ToString();

            return $"{baseCode}-{paddedSuffix}";
        }
    }
}
