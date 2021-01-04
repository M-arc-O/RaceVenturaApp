using System;
using System.Collections.Generic;
using RaceVentura.Models;
using RaceVentura.ViewModels;
using Xamarin.Forms;

namespace RaceVentura.Views
{
    public partial class RacesPage : ContentPage
    {
        RacesViewModel viewModel;

        public RacesPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new RacesViewModel();
        }

        private async void OnItemSelected(object sender, EventArgs args)
        {
            var layout = (BindableObject)sender;
            var item = (Race)layout.BindingContext;
            await Navigation.PushAsync(new RaceDetailPage(new RaceDetailViewModel(item)));
        }

        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewRacePage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.IsBusy = true;
        }
    }
}
