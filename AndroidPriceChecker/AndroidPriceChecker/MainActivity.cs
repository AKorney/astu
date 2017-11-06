using Android.App;
using Android.Widget;
using Android.OS;
using ZXing;
using ZXing.Mobile;

namespace AndroidPriceChecker
{
    [Activity(Label = "AndroidPriceChecker", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private ProductDB _db;
        MobileBarcodeScanner _barcodeScanner;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            _db = new ProductDB();


            Product p = new Product();
            
            p.BarCodeInfo = @"4601498006824";
            p.ProductName = @"Rom";
            p.Price = 4.5f;
            _db.Insert(p);
            
            // Set our view from the "main" layout resource
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
        }
        void HandleScanResult(ZXing.Result result)
        {
            string msg = "";

            if (result != null && !string.IsNullOrEmpty(result.Text))
            {
                msg = "Found Barcode: " + result.Text;
                var p = _db.FindByBarcode(result.Text);
            }

            else
                msg = "Scanning Canceled!";

            this.RunOnUiThread(() => Toast.MakeText(this, msg, ToastLength.Short).Show());
        }
    }
}

