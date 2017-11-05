using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SQLite;
using Android.Util;
using System.IO;

namespace AndroidPriceChecker
{
    public class ProductDB
    {
        private string _folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        private SQLiteConnection _currentConnection;
        private const string DB_FILENAME = "products.db"; 
        public ProductDB()
        {
            try
            {
                string _dbPath = Path.Combine(_folderPath, DB_FILENAME);
                _currentConnection = new SQLiteConnection(_dbPath);

                const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=?";
                var cmd = _currentConnection.CreateCommand(cmdText, typeof(Product).Name);
                if(cmd.ExecuteScalar<string>()==null)
                {
                    _currentConnection.CreateTable<Product>();
                }

            }
            catch (SQLiteException e)
            {
                Log.Info("DB_CRASH",e.StackTrace);
                throw e;
            }
            
        }
    }
}