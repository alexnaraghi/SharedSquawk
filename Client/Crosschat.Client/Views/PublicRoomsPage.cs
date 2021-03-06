﻿using SharedSquawk.Client.Seedwork;
using SharedSquawk.Client.Seedwork.Controls;
using Xamarin.Forms;

namespace SharedSquawk.Client.Views
{
    public class PublicRoomsPage : MvvmableContentPage
    {
		public PublicRoomsPage(ViewModelBase viewModel) : base(viewModel)
        {
            var listView = new BindableListView
                {
                    ItemTemplate = new DataTemplate(() =>
                        {
						var textCell = new TextCell();
                            textCell.SetBinding(TextCell.TextProperty, new Binding("Name"));
							textCell.TextColor = Styling.BlackText;
                            //textCell.SetBinding(TextCell.DetailProperty, new Binding("Description"));
                            return textCell;
                        }),
					SeparatorVisibility = SeparatorVisibility.None
                };

            listView.SetBinding(ListView.ItemsSourceProperty, new Binding("PublicRooms"));
            listView.SetBinding(BindableListView.ItemClickedCommandProperty, new Binding("SelectRoomCommand"));

			var loadingIndicator = new ActivityIndicator ();
            loadingIndicator.SetBinding(ActivityIndicator.IsRunningProperty, new Binding("IsBusy"));
			loadingIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, new Binding("IsBusy"));

            Content = new StackLayout
            {
                Children =
                        {
                            loadingIndicator,
                            listView
                        }
            };
        }
    }
}