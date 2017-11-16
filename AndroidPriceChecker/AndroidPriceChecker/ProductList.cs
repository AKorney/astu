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
using Android.Support.V7.Widget;
using ZXing.Mobile;
using ZXing;

namespace AndroidPriceChecker
{
    [Activity(Label = "ProductList")]
    public class ProductList : Activity
    {
        MobileBarcodeScanner _barcodeScanner;
        private RecyclerView _productList;
        private ImageButton _addButton;
        private ProductDBAdapter _adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            SetContentView(Resource.Layout.ProductList);
            _productList = FindViewById<RecyclerView>(Resource.Id.productListView);
            _addButton = FindViewById<ImageButton>(Resource.Id.AddProductButton);
            _adapter = new ProductDBAdapter(ShowUpdateDialog, ShowDeleteDialog);
            GridLayoutManager manager = new GridLayoutManager(this, 1, GridLayoutManager.Vertical, false);
            _productList.SetLayoutManager(manager);
            _productList.SetAdapter(_adapter);

            _addButton.Click += ShowAddDialog;
            _barcodeScanner = new MobileBarcodeScanner();
        }
        private void ShowUpdateDialog(int ID)
        {
            Product p = ProductDB.GetByID(ID);
            var view = LayoutInflater.Inflate(Resource.Layout.productCard, null);

            var name = view.FindViewById<EditText>(Resource.Id.nameInputCard);
            var desc = view.FindViewById<EditText>(Resource.Id.descriptionInputCard);
            var price = view.FindViewById<EditText>(Resource.Id.priceInputCard);
            var code = view.FindViewById<EditText>(Resource.Id.codeInputCard);
            var scanButton = view.FindViewById<ImageButton>(Resource.Id.scanCodeImgBtn);

            name.Text = p.ProductName;
            price.Text = p.Price.ToString();
            code.Text = p.BarCodeInfo;
            desc.Text = p.Description;

            scanButton.Click += async delegate
            {
                _barcodeScanner.UseCustomOverlay = false;
                //We can customize the top and bottom text of the default overlay
                _barcodeScanner.TopText = "Hold the camera up to the barcode\nAbout 6 inches away";
                _barcodeScanner.BottomText = "Wait for the barcode to automatically scan!";

                //Start scanning
                var result = await _barcodeScanner.Scan();

                HandleScanResult(result, code);
            };

            var dialog =
            new AlertDialog.Builder(this)
            .SetView(view)
            .SetTitle("Edit item")
            .SetPositiveButton("SAVE", (EventHandler<Android.Content.DialogClickEventArgs>)null)
                .SetNegativeButton("Cancel", (s, args) => { }).Create();

            dialog.Show();

            var okButton = dialog.GetButton((int)DialogButtonType.Positive);
            okButton.Click += delegate
            {
                if (string.IsNullOrWhiteSpace(name.Text))
                {
                    RunOnUiThread(() => Toast.MakeText(this, "Fill Name"
                        , ToastLength.Short).Show());
                    name.RequestFocus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(price.Text))
                {
                    RunOnUiThread(() => Toast.MakeText(this, "Fill Valid Price"
                        , ToastLength.Short).Show());
                    price.RequestFocus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(code.Text))
                {
                    RunOnUiThread(() => Toast.MakeText(this, "Fill Valid Barcode"
                        , ToastLength.Short).Show());
                    code.RequestFocus();
                    return;
                }
                if (!string.Equals(p.BarCodeInfo, code.Text.Trim()) 
                    && ProductDB.FindByBarcode(code.Text.Trim()) != null)
                {
                    RunOnUiThread(() => Toast.MakeText(this, "Barcode muast be unique"
                        , ToastLength.Short).Show());
                    code.RequestFocus();
                    return;
                }
                /*
                Product p = new Product
                {
                    ProductName = name.Text,
                    Price = float.Parse(price.Text),
                    Description = desc.Text,
                    BarCodeInfo = code.Text
                };
                ProductDB.Insert(p);
                */
                //_adapter.RefreshData();
                p.ProductName = name.Text.Trim();
                p.Description = desc.Text.Trim();
                p.Price = float.Parse(price.Text.Trim());
                p.BarCodeInfo = code.Text.Trim();

                ProductDB.Update(p);
                _adapter.RefreshData();
                dialog.Dismiss();

            };
        }
        private void ShowDeleteDialog(int ID)
        {
            Product p = ProductDB.GetByID(ID);
            new AlertDialog.Builder(this)
                        .SetView(new TextView(this) { Text = "Are you sure?" })
                        .SetTitle("Delete item")
                        .SetPositiveButton("OK", (s, e) => { ProductDB.Delete(p); _adapter.RefreshData(); })
                        .SetNegativeButton("Cancel", (s, e) => { })
                        .Show();
        }

        private void ShowAddDialog(object sender, EventArgs e)
        {
            var view = LayoutInflater.Inflate(Resource.Layout.productCard, null);

            EditText name = view.FindViewById<EditText>(Resource.Id.nameInputCard);
            EditText desc = view.FindViewById<EditText>(Resource.Id.descriptionInputCard);
            EditText price = view.FindViewById<EditText>(Resource.Id.priceInputCard);
            EditText code = view.FindViewById<EditText>(Resource.Id.codeInputCard);
            var scanButton = view.FindViewById<ImageButton>(Resource.Id.scanCodeImgBtn);

            scanButton.Click += async delegate
            {
                _barcodeScanner.UseCustomOverlay = false;
                //We can customize the top and bottom text of the default overlay
                _barcodeScanner.TopText = "Hold the camera up to the barcode\nAbout 6 inches away";
                _barcodeScanner.BottomText = "Wait for the barcode to automatically scan!";

                //Start scanning
                var result = await _barcodeScanner.Scan();

                HandleScanResult(result, code);
            };

            var dialog =
            new AlertDialog.Builder(this)
            .SetView(view)
            .SetTitle("New item")
            .SetPositiveButton("ADD", (EventHandler<Android.Content.DialogClickEventArgs>)null)
                .SetNegativeButton("Cancel", (s, args) => { }).Create();

            dialog.Show();

            var okButton = dialog.GetButton((int)DialogButtonType.Positive);
            okButton.Click += delegate
            {
                if (string.IsNullOrWhiteSpace(name.Text))
                {
                    RunOnUiThread(() => Toast.MakeText(this, "Fill Name"
                        , ToastLength.Short).Show());
                    name.RequestFocus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(price.Text))
                {
                    RunOnUiThread(() => Toast.MakeText(this, "Fill Valid Price"
                        , ToastLength.Short).Show());
                    price.RequestFocus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(code.Text))
                {
                    RunOnUiThread(() => Toast.MakeText(this, "Fill Valid Barcode"
                        , ToastLength.Short).Show());
                    code.RequestFocus();
                    return;
                }
                if (ProductDB.FindByBarcode(code.Text.Trim()) != null)
                {
                    RunOnUiThread(() => Toast.MakeText(this, "Barcode muast be unique"
                        , ToastLength.Short).Show());
                    code.RequestFocus();
                    return;
                }
                Product p = new Product
                {
                    ProductName = name.Text.Trim(),
                    Price = float.Parse(price.Text.Trim()),
                    Description = desc.Text.Trim(),
                    BarCodeInfo = code.Text.Trim()
                };
                ProductDB.Insert(p);
                _adapter.RefreshData();
                dialog.Dismiss();

            };

        }

        private void HandleScanResult(ZXing.Result result, EditText code)
        {
            string msg = "";

            if (result != null && !string.IsNullOrEmpty(result.Text))
            {
                code.Text = result.Text;
            }

            else
                msg = "Scanning Canceled!";

            this.RunOnUiThread(() => Toast.MakeText(this, msg, ToastLength.Short).Show());
        }

        
    }
}