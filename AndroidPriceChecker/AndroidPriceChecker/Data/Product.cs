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

namespace AndroidPriceChecker
{
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [Unique, NotNull]
        public string ProductName { get; set; }

        public string Description { get; set; }

        public float Price { get; set; }
        [Unique, NotNull]
        public string BarCodeInfo { get; set; }
    }
}