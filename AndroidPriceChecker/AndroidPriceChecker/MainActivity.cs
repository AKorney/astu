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
        MobileBarcodeScanner _barcodeScanner;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            var db = new ProductDB();
            
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
                msg = "Found Barcode: " + result.Text;
            else
                msg = "Scanning Canceled!";

            this.RunOnUiThread(() => Toast.MakeText(this, msg, ToastLength.Short).Show());
        }
    }
}

