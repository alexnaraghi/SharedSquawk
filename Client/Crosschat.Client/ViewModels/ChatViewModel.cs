﻿using System;
using SharedSquawk.Client.Seedwork;
using SharedSquawk.Client.Model.Managers;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using SharedSquawk.Client.Model.Entities.Messages;
using SharedSquawk.Client.Seedwork.Extensions;
using System.Collections.Specialized;
using System.Linq;
using SharedSquawk.Client.Views;
using System.Text;
using SharedSquawk.Client.Model.Entities;
using SharedSquawk.Client.Model.Helpers;
using System.Threading.Tasks;

namespace SharedSquawk.Client.ViewModels
{
	public class ChatViewModel : ViewModelBase
	{
		private readonly ApplicationManager _appManager;
		private ObservableCollection<EventViewModel> _messageEvents;
		private ObservableCollection<TypingEventViewModel> _typingEvents;
		private string _inputText;
		private readonly EventViewModelFactory _eventViewModelFactory;
		private string _typingEventsString;
		private RoomStatus _roomStatus;
		private RoomData _roomData;

		public ChatViewModel (ApplicationManager appManager, RoomData roomData)
		{
			_appManager = appManager;
			_eventViewModelFactory = new EventViewModelFactory();
			_messageEvents = new ObservableCollection<EventViewModel>();
			_typingEvents = new ObservableCollection<TypingEventViewModel>();

			_roomData = roomData;

			//Bind this view model to status changes on the model
			Status = _roomData.Status;
			_roomData.PropertyChanged += ChatModel_PropertyChanged;

			//Yikes, every time we switch rooms, we are creating EventViewModels for potentially hundreds of messages.
			//This could result in some major garbage.  Keep an eye on performance, maybe we will need to drop the view model
			//approach for messages.
			_roomData.TextMessages.SynchronizeWith(MessageEvents, i => _eventViewModelFactory.Get(i, _appManager.AccountManager.CurrentUser.UserId));
			_roomData.TypingEvents.SynchronizeWith(_typingEvents, i => new TypingEventViewModel(i as TypingEvent), s => (s as TypingEvent).UserId, d => d.UserId);
			_typingEvents.CollectionChanged += _typingEvents_CollectionChanged; 
		}

		void ChatModel_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			const string statusString = "Status"; //Upgrade C#, use nameof
			if (e.PropertyName == statusString)
			{
				Status = _roomData.Status;
			}
		}

		public RoomStatus Status
		{
			get{ return _roomStatus; }
			set
			{ 
				SetProperty (ref _roomStatus, value);

				//Notify any bindings that depend on the room status
				Raise ("IsInConnectedMode");
				Raise ("IsInMessageMode");
				Raise ("IsInRequestMode");
				Raise ("StatusText");
			}
		}

		public bool IsInConnectedMode
		{
			get { return _roomStatus == RoomStatus.Connected; }
		}

		public bool IsInMessageMode
		{
			get { return _roomStatus != RoomStatus.Connected && _roomStatus != RoomStatus.AwaitingOurApproval; }
		}

		public bool IsInRequestMode
		{
			get { return _roomStatus == RoomStatus.AwaitingOurApproval; }
		}

		public string StatusText
		{
			get {
				switch (_roomStatus)
				{
				case RoomStatus.Waiting:
					if (_roomData.Room.IsUserChat)
					{
						return "Waiting for the other user to join...";
					}
					else
					{
						return "Joining...";
					}
				case RoomStatus.Connected:
					return string.Empty;
				case RoomStatus.Error:
					return "An error has occurred.";
				case RoomStatus.OtherUserDeclined:
					return "The other user has declined this chat request.";
				case RoomStatus.OtherUserLeft:
					return "The other user has left the chat.";
				default:
					return "Unknown Status";
				}
			}
		}

		void _typingEvents_CollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (_typingEvents.Count == 0)
			{
				TypingEventsString = string.Empty;
				return;
			}
			else if (_typingEvents.Count > 4)
			{
				TypingEventsString = "Many people are typing...";
				return;
			}

			StringBuilder sb = new StringBuilder ();
			if (_typingEvents.Count == 1)
			{
				sb.Append (_typingEvents [0].UserName).Append (" is typing...");
			}
			else if (_typingEvents.Count == 2)
			{
				sb.Append (_typingEvents [0].UserName).Append (" and ");
				sb.Append (_typingEvents [1].UserName).Append (" are typing...");
			}
			else
			{
				for (int i = 0; i < _typingEvents.Count - 1; i++)
				{
					var str = _typingEvents [i].UserName;
					sb.Append (str).Append (", ");
				}

				//The last entry is special
				var lastStr = _typingEvents [_typingEvents.Count - 1].UserName;
				sb.Append (" and ").Append (lastStr);

				sb.Append (" are typing...");
			}

			TypingEventsString = sb.ToString(); 
		}

		protected override void OnShown ()
		{
			base.OnShown ();

			//Scroll to the bottom of the list whenever we get a new message
			//Future idea: scroll to bottom only when the user is already at the bottom, so they can browse messages easier.
			MessageEvents.CollectionChanged += (s, e) =>
			{
				if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
				{
					ScrollToBottom();
				}
			};

			//HACK: Hmmm, one more hack for scrolling.  We need the page to scroll when it first appears, AFTER the cells are created.
			//Can't find any other way to make that happen right now other than wait.
			Task.Run(async delegate
			{
				await Task.Delay(TimeSpan.FromMilliseconds(33));
				ScrollToBottom ();
			}).Start();
		}

		//note this does technically break the mvvm pattern, but I can't think of a less intrusive way to do this.
		//Simple code wins over conformant code here
		public void ScrollToBottom ()
		{
			var chatPage = _currentPage as ChatPage;
			if( chatPage != null && MessageEvents.Count > 0)
			{
				chatPage.OnItemAdded(MessageEvents.Last(), MessageEvents.Count);
			}
		}

		public string RoomName
		{
			get { return _roomData.Room.Name; }
		}

		public string InputText
		{
			get { return _inputText; }
			set 
			{ 
				SetProperty(ref _inputText, value); 
			}
		}

		public ObservableCollection<EventViewModel> MessageEvents {
			get { return _messageEvents; }
			set { SetProperty(ref _messageEvents, value); }
		}

		public string TypingEventsString 
		{
			get 
			{
				return _typingEventsString;
			}
			set 
			{
				SetProperty(ref _typingEventsString, value); 
				Raise ("AreTypingEvents");
			}
		}
		public bool AreTypingEvents {
			get 
			{
				return !string.IsNullOrWhiteSpace (_typingEventsString);
			}
		}

		public ICommand SendMessageCommand
		{
			get { return new Command(OnSendMessage); }
		}

		private async void OnSendMessage()
		{
			if (string.IsNullOrEmpty(InputText))
				return;
			string text = InputText;
			InputText = string.Empty;

			IsBusy = true;
			await _appManager.ChatManager.SendMessage(text, _roomData.Room.RoomId);
			IsBusy = false;
		}

		public ICommand UpdateCommand
		{
			get{ return new Command (OnForceUpdate); }
		}

		private async void OnForceUpdate()
		{
			IsBusy = true;
			await _appManager.ChatManager.GetChatUpdate ();
			IsBusy = false;
		}


		public ICommand AcceptChatCommand
		{
			get{ return new Command (OnAcceptChat); }
		}

		private async void OnAcceptChat()
		{
			if (_roomData.Room.UserId != null)
			{
				IsBusy = true;
				await _appManager.ChatManager.ApproveUserChat (_roomData.Room.UserId.Value);
				IsBusy = false;
			}
		}

		public ICommand DeclineChatCommand
		{
			get{ return new Command (OnDeclineChat); }
		}

		private async void OnDeclineChat()
		{
			if (_roomData.Room.UserId != null)
			{
				IsBusy = true;
				await _appManager.ChatManager.DeclineUserChat (_roomData.Room.UserId.Value);
				IsBusy = false;
			}
			await PopAsync ();
		}

		public ICommand LeaveRoomCommand
		{
			get{ return new Command (OnLeaveRoomCommand); }
		}

		private async void OnLeaveRoomCommand()
		{
			_appManager.ChatManager.LeaveRoom (_roomData.Room.RoomId);
			await PopAsync ();
		}

		private async Task OnLeaveRoom()
		{
			_appManager.ChatManager.LeaveRoom (_roomData.Room.RoomId);
			await PopAsync ();
		}

		private async Task OnViewProfile()
		{
			if (_roomData.Room.UserId != null)
			{
				var fullMember = await _appManager.ChatManager.GetMemberDetails (_roomData.Room.UserId.Value);
				var model = new UserDetailViewModel (_appManager, fullMember)
				{
					HasChatOption = false
				};
				await model.ShowAsync ();
			}
		}

		public ICommand ContextOptionsCommand
		{
			get { return new Command(OnOpenContextOptions); }
		}

		private async void OnOpenContextOptions()
		{
			//TODO: add the missing features
			string[] buttons;

			//Different context options depending if this is a user or group chat
			if (_roomData.Room.IsUserChat)
			{
				buttons = new string[] {
					"View Profile", 
					"Leave Chat"
				};
			}
			else
			{
				buttons = new string[] {
					//"See Members", 
					"Leave Chat"
				};
			}

			var chosen = await Action ("Options", "Cancel", null/*"Block"*/, buttons);
			switch (chosen)
			{
			case "Block":
				await Notify("Error", "We haven't created this feature yet, stay tuned!");
				break;
			case "View Profile":
				await OnViewProfile();
				break;
			case "See Members":
				await Notify("Error", "We haven't created this feature yet, stay tuned!");
				break;
			case "Leave Chat":
				await OnLeaveRoom ();
				break;
			default:
				break;
			}
		}
	}
}

