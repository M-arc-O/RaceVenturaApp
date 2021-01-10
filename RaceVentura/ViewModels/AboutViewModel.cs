﻿using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RaceVentura.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://raceventura.nl/#/appsupport"));
        }

        public ICommand OpenWebCommand { get; }
    }
}