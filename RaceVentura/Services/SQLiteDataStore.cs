using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RaceVentura.Extensions;
using RaceVentura.Models;
using SQLite;

namespace RaceVentura.Services
{
    public class SQLiteDataStore: IDataStore<Race>
    {
        private const string _databaseFilename = "RaceVenturaSQLite.db3";

        public const SQLiteOpenFlags _flags =            
            SQLite.SQLiteOpenFlags.ReadWrite |          // open the database in read/write mode                                               
            SQLite.SQLiteOpenFlags.Create |             // create the database if it doesn't exist                                            
            SQLite.SQLiteOpenFlags.SharedCache;         // enable multi-threaded database access

        private static string _databasePath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, _databaseFilename);
            }
        }

        private static readonly Lazy<SQLiteAsyncConnection> _lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(_databasePath, _flags);
        });

        private static SQLiteAsyncConnection _database => _lazyInitializer.Value;
        private static bool _initialized = false;

        public SQLiteDataStore()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        public Task<int> AddItemAsync(Race item)
        {
            return _database.InsertAsync(item);
        }

        public async Task<int> DeleteItemAsync(Race item)
        {
            return await _database.DeleteAsync(item);
        }

        public Task<Race> GetItemAsync(Guid id)
        {
            return _database.Table<Race>().Where(race => race.RaceId == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Race>> GetItemsAsync(bool forceRefresh = false)
        {
            return await _database.Table<Race>().ToListAsync();
        }

        public Task<int> UpdateItemAsync(Race item)
        {
            return _database.UpdateAsync(item);
        }

        private async Task InitializeAsync()
        {
            if (!_initialized)
            {
                if (!_database.TableMappings.Any(m => m.MappedType.Name == typeof(Race).Name))
                {
                    await _database.CreateTablesAsync(CreateFlags.None, typeof(Race)).ConfigureAwait(false);
                }
                _initialized = true;
            }
        }
    }
}
