using Qayimli.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Qayimli.Repository.Data
{
    public static class StoreContextSeed
    {
        public async static Task SeedAsync(StoreContext _dbContext)
        {
            if (_dbContext.ReviewCategories.Count() == 0)
            {
                var reviewCategoryData = File.ReadAllText("../Qayimli.Repository/Data/DataSeed/reviewcategories.json");
                var reviewCategories = JsonSerializer.Deserialize<List<ReviewCategory>>(reviewCategoryData);

                if (reviewCategories?.Count() > 0)
                {
                    foreach (var category in reviewCategories)
                    {
                        _dbContext.Set<ReviewCategory>().Add(category);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
