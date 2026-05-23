using GastosApp.Client.Models;
using Supabase.Postgrest;

namespace GastosApp.Client.Services;

public class CategoryService
{
    private readonly SupabaseService _supabase;

    public CategoryService(SupabaseService supabase)
    {
        _supabase = supabase;
    }

    public async Task<List<Category>> GetByAccountAsync(Guid accountId)
    {
        var response = await _supabase.Client
            .From<Category>()
            .Where(c => c.AccountId == accountId)
            .Order(c => c.Name, Constants.Ordering.Ascending)
            .Get();

        return response.Models;
    }

    public async Task<Category> CreateAsync(Guid accountId, string name, string block)
    {
        var newCategory = new Category
        {
            AccountId = accountId,
            Name = name,
            Block = block
        };

        var response = await _supabase.Client.From<Category>().Insert(newCategory);
        return response.Models.First();
    }

    public async Task<Category> UpdateAsync(Guid id, string name, string block)
    {
        var existing = await _supabase.Client
            .From<Category>()
            .Where(c => c.Id == id)
            .Single();

        if (existing is null)
            throw new InvalidOperationException("Categoría no encontrada.");

        existing.Name = name;
        existing.Block = block;

        var response = await existing.Update<Category>();
        return response.Models.First();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabase.Client
            .From<Category>()
            .Where(c => c.Id == id)
            .Delete();
    }
}
