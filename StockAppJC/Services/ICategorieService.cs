using StockAppJC.General;
using StockAppJC.Models;

namespace StockAppJC.Services
{
    public interface ICategorieService
    {
        Task<IEnumerable<Categoria>> GetCategories();
        Task<Categoria> GetCategory(int id);
        Task<answer> CreateCategory(CategoryViewModel category);
        Task<answer> UpdateCategory(int id, CategoryViewModel category);
        Task<answer> DeleteCategory(int id);
    }
}
