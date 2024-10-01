using Microsoft.EntityFrameworkCore;
using StockAppJC.DbContext;
using StockAppJC.General;
using StockAppJC.Models;

namespace StockAppJC.Services
{
    public class CategorieService : ICategorieService
    {
        private readonly ApplicationDbContext _context;

        public CategorieService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categoria>> GetCategories()
        {
            try
            {
                var categories = _context.Categorias.ToList();
                List<Categoria> listCategories = new List<Categoria>();

                if (categories.Count != 0) 
                {
                    foreach (var item in categories)
                    {
                        Categoria category = new Categoria
                        {
                            id = item.id,
                            nombre = item.nombre
                        };

                        listCategories.Add(category);
                    }
                }
                else
                {
                    throw new KeyNotFoundException("Categories is Empty");
                }

                return listCategories;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Internal Server Error: {ex.Message}");
            }
        }

        public async Task<Categoria> GetCategory(int id)
        {
            try
            {
                var category = await _context.Categorias.FirstOrDefaultAsync(x => x.id == id);
                if (category == null)
                {
                    throw new KeyNotFoundException("Category Not Found");
                }

                Categoria categoryObj = new Categoria
                {
                    id = category.id,
                    nombre = category.nombre
                };

                return categoryObj;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Internal Server Error: {ex.Message}");
            }
        }

        public async Task<answer> CreateCategory(Categoria category)
        {
            answer response = new answer();
            try
            {
                var newCategory = new Categoria
                {
                    nombre = category.nombre
                };

                if (newCategory == null)
                {
                    response.code = 400;
                    response.description = "Category Not Created Because Category is Null";
                }
                else 
                {
                    _context.Categorias.Add(newCategory);
                    await _context.SaveChangesAsync();
                    response.code = 200;
                    response.description = "Category Created";
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Internal Server Error: {ex.Message}");
            }
            return response;
        }

        public async Task<answer> UpdateCategory(int id, Categoria category)
        {
            answer response = new answer();
            try
            {
                var categoryObj = _context.Categorias.FirstOrDefault(x => x.id == id);
                if (categoryObj == null)
                {
                    response.code = 404;
                    response.description = "Category Not Found";
                }
                else 
                {
                    var categoryUpdate = new Categoria
                    {
                        id = category.id,
                        nombre = category.nombre
                    };
                    _context.Categorias.Update(categoryUpdate);
                    await _context.SaveChangesAsync();
                    response.code = 200;
                    response.description = "Category Updated";
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Internal Server Error: {ex.Message}");
            }
            return response;
        }

        public async Task<answer> DeleteCategory(int id) 
        {
            answer response = new answer();
            try 
            {
                var categorySearch = _context.Categorias.FirstOrDefault(x => x.id == id);
                if (categorySearch == null)
                {
                    response.code = 400;
                    response.description = "Category Not Found";
                }
                else
                {
                    _context.Categorias.Remove(categorySearch);
                    await _context.SaveChangesAsync();
                    response.code = 200;
                    response.description = "Category Deleted";
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Internal Server Error: {ex.Message}");
            }
            return response;
        }
    }
}
