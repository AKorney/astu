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
        private static string _folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        private static string DB_FILENAME = "products.db";
        private static string _dbPath = Path.Combine(_folderPath, DB_FILENAME);
        public ProductDB()
        {
            try
            {
                using (var db = new SQLiteConnection(_dbPath))
                {

                    const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=?";
                    var cmd = db.CreateCommand(cmdText, typeof(Product).Name);
                    if (cmd.ExecuteScalar<string>() == null)
                    {
                        db.CreateTable<Product>();
                    }
                }
            }
            catch (SQLiteException e)
            {
                Log.Info("DB_CRASH",e.StackTrace);
                throw e;
            }            
        }

        public void Insert(Product p)
        {
            try
            {
                using (var db = new SQLiteConnection(_dbPath))
                {
                    db.Insert(p);                   
                }
            }
            catch (SQLiteException e)
            {
                Log.Info("DB_CRASH", e.Message);
                throw e;
            }
        }

        public List<Product> Select()
        {
            try
            {
                using (var db = new SQLiteConnection(_dbPath))
                {
                    return db.Table<Product>().ToList();
                }
            }
            catch (SQLiteException e)
            {
                Log.Info("DB_CRASH", e.Message);
                throw e;
            }
        }

        public void Update(Product p)
        {
            try
            {
                using (var db = new SQLiteConnection(_dbPath))
                {
                    db.Query<Product>(@"UPDATE Product set ProductName=?, Description=?, Price=?, BarCodeInfo=? WHERE ID=?",
                        p.ProductName, p.Description, p.Price, p.BarCodeInfo, p.ID);
                }
            }
            catch (SQLiteException e)
            {
                Log.Info("DB_CRASH", e.Message);
                throw e;
            }
        }


        public void Delete(Product p)
        {
            try
            {
                using (var db = new SQLiteConnection(_dbPath))
                {
                    db.Delete(p);
                }
            }
            catch (SQLiteException e)
            {
                Log.Info("DB_CRASH", e.Message);
                throw e;
            }
        }


        public Product FindByBarcode(string barcodeInfo)
        {
            using (var db = new SQLiteConnection(_dbPath))
            {
                var prod = from p in db.Table<Product>()
                           where p.BarCodeInfo.Equals(barcodeInfo)
                           select p;
                return prod.FirstOrDefault();
            }
        }
    }
}