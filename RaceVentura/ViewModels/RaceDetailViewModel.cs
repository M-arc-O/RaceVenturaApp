using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RaceVentura.Models;

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

        public async Task AddPoint(Models.Point point)
        {
            if (Item.Points == null)
            {
                Item.Points = new List<Models.Point>();
            }

            var existingPoint = Item.Points.FirstOrDefault(p => p.PointId == point.PointId);

            if (existingPoint == null)
            {
                Item.Points.Add(point);
            }

            await DataStore.UpdateItemAsync(Item);
        }
    }
}
