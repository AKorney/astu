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
    public static class ProductDB
    {
        private static string _folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        private static string DB_FILENAME = "products.db";
        private static string _dbPath = Path.Combine(_folderPath, DB_FILENAME);
        public static void Init()
        {
            try
            {
                using (var db = new SQLiteConnection(_dbPath))
                {
                    db.CreateTable<Product>();

                    if (db.Table<Product>().Count() == 0)
                    {
                        Product p = new Product
                        {
                            BarCodeInfo = @"4601498006824",
                            ProductName = @"Rom",
                            Price = 4.5f
                        };
                        Insert(p);
                    }
                }
            }
            catch (SQLiteException e)
            {
                Log.Info("DB_CRASH",e.Message);
                throw e;
            }            
        }

        public static void Insert(Product p)
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

        public static List<Product> Select()
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

        public static void Update(Product p)
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


        public static void Delete(Product p)
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

        public static Product GetByID(int id)
        {
            using (var db = new SQLiteConnection(_dbPath))
            {
                var prod = from p in db.Table<Product>()
                           where p.ID.Equals(id)
                           select p;
                return prod.FirstOrDefault();
            }
        }

        public static Product FindByBarcode(string barcodeInfo)
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