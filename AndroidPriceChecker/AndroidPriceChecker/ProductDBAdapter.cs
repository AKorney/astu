using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;

namespace AndroidPriceChecker
{
    class ProductDBAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ProductDBAdapterClickEventArgs> ItemClick;
        public event EventHandler<ProductDBAdapterClickEventArgs> ButtonClick;

        List<Product> items;

        public ProductDBAdapter(Action<int> updateAction, Action<int> deleteAction)
        {
            items = ProductDB.Select();
            ItemClick += (s, args) => { updateAction(items[args.Position].ID); };
            ButtonClick += (s, args) => { deleteAction(items[args.Position].ID); };
        }

        

        public void RefreshData()
        {
            items = ProductDB.Select();
            NotifyDataSetChanged();
        }
        
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ProductItem, null);

            var vh = new ProductDBAdapterViewHolder(itemView, OnClick, OnButtonClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            var holder = viewHolder as ProductDBAdapterViewHolder;
            holder.BindItem(item);
        }

        public override int ItemCount => items.Count;

        void OnClick(ProductDBAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnButtonClick(ProductDBAdapterClickEventArgs args) => ButtonClick?.Invoke(this, args);

    }

    public class ProductDBAdapterViewHolder : RecyclerView.ViewHolder
    {
        private Product _product;
        private TextView _nameTextView, _priceTextView, _barcodeTextView;
        private ImageButton _delButton;

        public void BindItem(Product product)
        {
            _nameTextView.Text = product.ProductName;
            _priceTextView.Text = product.Price.ToString();
            _barcodeTextView.Text = product.BarCodeInfo;
            _product = product;
        }

        public ProductDBAdapterViewHolder(View itemView, Action<ProductDBAdapterClickEventArgs> clickListener,
            Action<ProductDBAdapterClickEventArgs> deleteListener) : base(itemView)
        {
            _nameTextView = itemView.FindViewById<TextView>(Resource.Id.nameText);
            _priceTextView = itemView.FindViewById<TextView>(Resource.Id.priceText);
            _barcodeTextView = itemView.FindViewById<TextView>(Resource.Id.codeText);
            _delButton = itemView.FindViewById<ImageButton>(Resource.Id.deleteButton);
            _delButton.Click += (sender, e) => deleteListener(new ProductDBAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.Click += (sender, e) => clickListener(new ProductDBAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
          
        }
    }

    public class ProductDBAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}