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

namespace AndroidPriceChecker
{
    [Activity(Label = "CardActivity")]
    public class CardActivity : Activity
    {
        public const string Key_ProductID = "Product.ID";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.productCard);

            int id = Intent.GetIntExtra(Key_ProductID, 0);
            Product product = ProductDB.GetByID(id);

            EditText name = FindViewById<EditText>(Resource.Id.nameInputCard);
            EditText desc = FindViewById<EditText>(Resource.Id.descriptionInputCard);
            EditText price = FindViewById<EditText>(Resource.Id.priceInputCard);
            EditText code = FindViewById<EditText>(Resource.Id.codeInputCard);

            Button scan = FindViewById<Button>(Resource.Id.scanCodeImgBtn);
            name.Text = product.ProductName;
            desc.Text = product.Description;
            price.Text = product.Price.ToString();
            code.Text = product.BarCodeInfo;
            // Create your application here
        }

        
    }
}