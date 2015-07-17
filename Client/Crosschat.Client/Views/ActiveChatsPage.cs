﻿using SharedSquawk.Client.Seedwork;
using SharedSquawk.Client.Seedwork.Controls;
using Xamarin.Forms;
using SharedSquawk.Client.Views.ValueConverters;

namespace SharedSquawk.Client.Views
{
    public class ActiveChatsPage : MvvmableContentPage
    {
		public ActiveChatsPage(ViewModelBase viewModel) : base(viewModel)
		{
			var listView = new BindableListView
			{
				ItemTemplate = new DataTemplate(() =>
					{
						var textCell = new TextCell();
						textCell.SetBinding(TextCell.TextProperty, new Binding("Name"));
						textCell.SetBinding(TextCell.DetailProperty, new Binding("DescriptionText"));
						return textCell;
					}),
				SeparatorVisibility = SeparatorVisibility.None
			};

			listView.SetBinding(ListView.ItemsSourceProperty, new Binding("ActiveChats"));
			listView.SetBinding(BindableListView.ItemClickedCommandProperty, new Binding("SelectActiveChatCommand"));
			listView.SetBinding(ListView.IsVisibleProperty, new Binding("HasConversations", BindingMode.OneWay));
				
			var noItemsLabel = new Label {
				Text = "Start a conversation or open a room!",
				HorizontalOptions = LayoutOptions.Center,
				FontSize = 16,
				TextColor = Color.Gray
			};
			var noItemsLayout = new StackLayout
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = 
				{
					new BoxView{HeightRequest = 20},
					noItemsLabel
				}
			};
			noItemsLayout.SetBinding(StackLayout.IsVisibleProperty, new Binding("HasConversations", BindingMode.OneWay, converter: new InverterConverter()));
			

			var contactsLoadingIndicator = new ActivityIndicator ();
			contactsLoadingIndicator.SetBinding(ActivityIndicator.IsRunningProperty, new Binding("IsBusy"));
			contactsLoadingIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, new Binding("IsBusy"));

			Content = new StackLayout
			{
				Children =
				{
					contactsLoadingIndicator,
					listView,
					noItemsLayout
				}
			};
		}
    }
}