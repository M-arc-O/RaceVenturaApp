using RaceVentura.Models;

namespace RaceVentura.ViewModels
{
    public class RaceDetailViewModel : BaseViewModel
    {
        public Race Item { get; set; }
        public RaceDetailViewModel(Race item = null)
        {
            Title = item?.Name;
            Item = item;
        }
    }
}
