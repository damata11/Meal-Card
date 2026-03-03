using Meal_Card.Models;
using SQLite;

namespace Meal_Card.Services
{
    public class FavoritosService
    {
        private readonly SQLiteAsyncConnection _database;

        public FavoritosService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "favoritos.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Favorito>().Wait();
            ;
        }

        public async Task<Favorito> ReadAsync(int id, int id_utilizador)
        {
            try
            {
                return await _database.Table<Favorito>().Where(p => p.Id_produto == id && p.Id_utilizador == id_utilizador).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Não foi possivel obter os dados{ex.Message}");
                throw;
            }
        }

        public async Task<List<Favorito>> ReadAllAsync(int id_utilizador)
        {
            try
            {
                return await _database.Table<Favorito>().Where(p => p.Id_utilizador == id_utilizador).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Não foi possivel obter os dados{ex.Message}");
                throw;
            }
        }

        public async Task CreateAsync(Favorito favoritos)
        {

            try
            {
                await _database.InsertAsync(favoritos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Não foi possivel adicionar o produto as favoritos{ex.Message}");
                throw;
            }

        }

        public async Task DeleteAsync(Favorito favoritos)
        {
            try
            {
                await _database.DeleteAsync(favoritos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Não foi possivel remover o produto{ex.Message}");
                throw;
            }
        }
    }
}
