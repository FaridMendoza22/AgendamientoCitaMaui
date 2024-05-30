using Newtonsoft.Json;
using SQLite;

namespace AgendamientoCita
{
    public class LocalDbService
    {
        const string DBName = "LocalDb_Nail";
        readonly SQLiteAsyncConnection connection = null!;

        public LocalDbService()
        {
            try
            {
                var Route = Path.Combine(FileSystem.AppDataDirectory, DBName);
                connection = new SQLiteAsyncConnection(Route);
                connection.CreateTableAsync<CustomerInSession>().Wait();
            }
            catch
            {

            }
        }

        public async Task CreateCustomer(CustomerInSession Item)
        {
            try
            {
                await connection.InsertAsync(Item);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<CustomerInSession?> GetCustomer()
        {
            try
            {
                return await connection.Table<CustomerInSession>().FirstAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default;
            }
        }

        public async Task DeleteCustomer(int Rowid)
        {
            try
            {
                await connection.Table<CustomerInSession>()
                    .Where(x => x.Rowid == Rowid)
                   .DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}