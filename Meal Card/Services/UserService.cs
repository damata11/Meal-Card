using Meal_Card.Models;
using SQLite;

namespace Meal_Card.Services
{
    public class UserService
    {

        private readonly SQLiteAsyncConnection _database;

        public UserService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "user.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Models.Utilizador>().Wait();
        }

        public async Task<Utilizador> ReadAsync(int id)
        {
            try
            {
                return await _database.Table<Utilizador>().Where(u => u.Id_utilizador == id).FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($" Não foi possivel obter os dados{ex.Message}");
                throw;
            }
        }

        public async Task CreateAsync(Utilizador user)
        {
            try
            {
                await _database.InsertAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Não foi possivel inserir os dados do utilizador a base de dados{ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(Utilizador user)
        {
            try
            {
                await _database.DeleteAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Não foi possivel inserir os dados do utilizador a base de dados{ex.Message}");
                throw;
            }
        }
    }
}
