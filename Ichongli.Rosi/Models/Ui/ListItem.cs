using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ichongli.Rosi.Models.Ui
{

    public class ItemWithUrl : Item
    {

        private ItemImage _itemImage;
        public ItemImage ItemImage
        {
            get { return this._itemImage; }
            set
            {
                if (_itemImage != value)
                {
                    _itemImage = value;
                    NotifyPropertyChanged();
                }
            }
        }

    }

    public class ItemImage : ModelBase
    {
        private string _thumbnail;
        public string Thumbnail
        {
            get
            {
                return this._thumbnail;
            }
            set
            {
                if (this._thumbnail != value)
                {
                    this._thumbnail = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        private string _medium;
        public string Medium
        {
            get
            { return this._medium; }
            set
            {
                if (this._medium != value)
                {
                    this._medium = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        private string _large;
        public string Large
        {
            get
            { return this._large; }
            set
            {
                if (this._large != value)
                {
                    this._large = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
    }

    public class Item : ModelBase
    {

        private string _ItemId;
        public string ItemId
        {
            get { return this._ItemId; }
            set
            {
                if (_ItemId != value)
                {
                    _ItemId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _Title;
        public string Title
        {
            get { return this._Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    NotifyPropertyChanged();
                }
            }
        }

    }
}
