using System;
using System.Collections.Generic;
using RaceVentura.Models;
using RaceVentura.ViewModels;
using Xamarin.Forms;

namespace RaceVentura.Views
{
    public partial class RaceDetailPage : ContentPage
    {
        RaceDetailViewModel viewModel;

        public RaceDetailPage(RaceDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }
    }
}
