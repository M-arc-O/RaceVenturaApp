using RaceVentura.Models;
using Xamarin.Forms;

namespace RaceVentura.ViewModels
{
    public class RaceDetailViewModel : BaseViewModel
    {
        public bool RaceActive
        {
            get { return Item.RaceActive; }
            set
            {
                Item.RaceActive = value;
                DataStore.UpdateItemAsync(Item);
                OnPropertyChanged();
            }
        }

        private bool _notProcessing;
        public bool NotProcessing
        {
            get { return _notProcessing; }
            set
            {
                _notProcessing = value;
                OnPropertyChanged();
            }
        }

        public Race Item { get; set; }
        public RaceDetailViewModel(Race item = null)
        {
            NotProcessing = true;
            Title = item?.Name;
            Item = item;
        }
    }
}
