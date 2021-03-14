using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using RaceVentura.Models;
using RaceVentura.Views;

namespace RaceVentura.ViewModels
{
    public class RacesViewModel : BaseViewModel
    {
        public ObservableCollection<Race> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public RacesViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Race>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewRacePage, Race>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Race;
                Items.Add(newItem);
                await DataStore.AddItemAsync(newItem);
            });

            MessagingCenter.Subscribe<RaceDetailPage, Race>(this, "RemoveItem", async (obj, item) =>
            {
                var removeItem = item as Race;
                Items.Remove(removeItem);
                await DataStore.DeleteItemAsync(removeItem);
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            { 
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async void DeleteItems()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    await DataStore.DeleteItemAsync(item);
                }
                items = await DataStore.GetItemsAsync(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}