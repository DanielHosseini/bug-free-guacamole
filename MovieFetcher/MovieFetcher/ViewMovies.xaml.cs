﻿using FFImageLoading.Forms;
using MovieFetcher.Core;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieFetcher
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewMovies : ContentPage
    {
        private const string YIFYURL = "https://yts.ag/api/v2/list_movies.json?sort_by=year&order_by=desc&limit=50";
        private int movieNumber = 0;
        private JSONHandler jsonHandler = new JSONHandler();
        private bool _userTapped;


        public ViewMovies()
        {
            InitializeComponent();
            Title = "MovieFetcher";
            FetchMovies();
        }

        private async void FetchMovies()
        {
            try
            {
                loadingIndictor.IsRunning = true;
                var YIFYMoviesResponse = await jsonHandler.ParseJsonAsync(YIFYURL);
                PopulateUiGridView(YIFYMoviesResponse);
                loadingIndictor.IsRunning = false;


            }
            catch (Exception)
            {
                //Throw error to UI
                throw new InvalidOperationException("Problem with Parsing JSON file");
            }

        }


        public void PopulateUiGridView(YIFYMovies yifyMovies)
        {
            var TotalAmountOfMovies = yifyMovies.data.limit;
            IList<Movy> movieObjects = yifyMovies.data.movies;
            var tapGestureRecognizer = new TapGestureRecognizer();

            var scroll = new ScrollView
            {
                Orientation = ScrollOrientation.Vertical,
            };
            var grid = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0
            };

            for (int row = 0; row < TotalAmountOfMovies / 2; row++)
            {
                for (int column = 0; column < 2; column++)
                {
                    var image = new CachedImage() { Aspect = Aspect.Fill, Source = movieObjects[movieNumber].large_cover_image };
                    image.GestureRecognizers.Add(tapGestureRecognizer);
                    grid.Children.Add(image, column, row);

                    movieNumber++;
                }
            }

            Content = scroll;
            scroll.Content = grid;
            tapGestureRecognizer.Tapped += TappedSpecificVideo;


            async void TappedSpecificVideo(object sender, EventArgs e)
            {
                if (_userTapped)
                    return;

                _userTapped = true;
                var tempImage = (CachedImage)sender;
                tempImage.Opacity = .5;
                var movieIDTapped = grid.Children.IndexOf(tempImage);
                var specificMovieObject = movieObjects[movieIDTapped];
                await Navigation.PushAsync(new SpecificView(specificMovieObject, movieIDTapped));
                tempImage.Opacity = 1;
                _userTapped = false;

            }

        }

      


    }
}