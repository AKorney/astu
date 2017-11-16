using Android.App;
using Android.Widget;
using Android.OS;
using ZXing;
using ZXing.Mobile;
using Android.Content;

namespace AndroidPriceChecker
{
    [Activity(Label = "AndroidPriceChecker", MainLauncher = true)]
    public class MainActivity : Activity
    {
      
        MobileBarcodeScanner _barcodeScanner;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ProductDB.Init();
            SetContentView(Resource.Layout.Main);
            MobileBarcodeScanner.Initialize(Application);
            _barcodeScanner = new MobileBarcodeScanner();

            Button scanButton = this.FindViewById<Button>(Resource.Id.scanButton);

            scanButton.Click += async delegate
            {
                _barcodeScanner.UseCustomOverlay = false;
                //We can customize the top and bottom text of the default overlay
                _barcodeScanner.TopText = "Hold the camera up to the barcode\nAbout 6 inches away";
                _barcodeScanner.BottomText = "Wait for the barcode to automatically scan!";

                //Start scanning
                var result = await _barcodeScanner.Scan();

                HandleScanResult(result);
            };

            Button listButton = FindViewById<Button>(Resource.Id.listButton);
            listButton.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ProductList));
                StartActivity(intent);
            };
        }
        void HandleScanResult(ZXing.Result result)
        {
            string msg = "";

            if (result != null && !string.IsNullOrEmpty(result.Text))
            {
                msg = "Found Barcode: " + result.Text;
                var product = ProductDB.FindByBarcode(result.Text);


                if(product!=null)
                {
                    var view = LayoutInflater.Inflate(Resource.Layout.productCard, null);

                    var name = view.FindViewById<EditText>(Resource.Id.nameInputCard);
                    var desc = view.FindViewById<EditText>(Resource.Id.descriptionInputCard);
                    var price = view.FindViewById<EditText>(Resource.Id.priceInputCard);
                    var code = view.FindViewById<EditText>(Resource.Id.codeInputCard);

                    name.Text = product.ProductName;
                    desc.Text = product.Description;
                    price.Text = product.Price.ToString();
                    code.Text = product.BarCodeInfo;
                    code.Enabled = price.Enabled = desc.Enabled = name.Enabled = false;

                    var dialog = new AlertDialog.Builder(this)
                        .SetView(view)
                        .SetTitle("Search result:")
                        .Create();
                        dialog.SetButton("OK", (s,e) => { dialog.Dismiss(); });
                    dialog.Show();
                }
                else
                {
                    new AlertDialog.Builder(this)
                        .SetView(new TextView(this) { Text = "Sorry, nothing to show! Run web search?" })
                        .SetTitle("SearchResult")
                        .SetPositiveButton("OK", (s, e) =>
                        {
                            var intent = new Intent(Intent.ActionWebSearch);
                            intent.PutExtra(SearchManager.Query, result.Text);
                            StartActivity(intent);
                        })
                        .SetNegativeButton("Cancel", (s, e) => { })
                        .Show();
                }
            }

            else
                msg = "Scanning Canceled!";

            this.RunOnUiThread(() => Toast.MakeText(this, msg, ToastLength.Short).Show());
        }
    }
}

