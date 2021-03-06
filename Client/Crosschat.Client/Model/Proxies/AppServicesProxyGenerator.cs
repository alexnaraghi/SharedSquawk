﻿using System.Threading.Tasks;
using SharedSquawk.Server.Application.DataTransferObjects.Requests;


namespace SharedSquawk.Client.Model.Proxies
{

	
    /// <summary>
    /// Autogenerated proxy for AppController
    /// </summary>
	public partial class AuthenticationServiceProxy : ServiceProxyBase
	{
        public AuthenticationServiceProxy(ConnectionManager connectionManager) : base(connectionManager)
        {
        }


		public Task<AuthenticationResponse> Authenticate(AuthenticationRequest request)
		{
			return ConnectionManager.SendRequestAndWaitResponse<AuthenticationRequest,AuthenticationResponse>(request);
		}
		
	}
	
    /// <summary>
    /// Autogenerated proxy for AppController
    /// </summary>
	/*
	public partial class ChatServiceProxy : ServiceProxyBase, IChatServiceProxy
	{
		public Task<ConnectedMembersResponse> GetConnectedMembers (ConnectedMembersRequest request)
		{
			throw new System.NotImplementedException ();
		}

        public ChatServiceProxy(ConnectionManager connectionManager) : base(connectionManager)
        {
        }


		public void PublicMessage(PublicMessageRequest request)
		{
			//ConnectionManager.SendRequest(request);
		}

		public Task<SendImageResponse> SendImage(SendImageRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<SendImageResponse>(request);
		}

		public Task<GrantModershipResponse> GrantModership(GrantModershipRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<GrantModershipResponse>(request);
		}

		public Task<RemoveModershipResponse> RemoveModership(RemoveModershipRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<RemoveModershipResponse>(request);
		}

		public Task<DevoiceResponse> Devoice(DevoiceRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<DevoiceResponse>(request);
		}

		public Task<BanResponse> Ban(BanRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<BanResponse>(request);
		}

		public Task<ResetPhotoResponse> ResetPhoto(ResetPhotoRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<ResetPhotoResponse>(request);
		}

		public Task<LastMessageResponse> GetLastMessages(LastMessageRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<LastMessageResponse>(request);
		}

		public Task<GetOnlineUsersResponse> GetOnlineUsers(GetOnlineUsersRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<GetOnlineUsersResponse>(request);
		}
		
	}
	*/


	
    /// <summary>
    /// Autogenerated proxy for AppController
    /// </summary>
	public partial class FriendsServiceProxy : ServiceProxyBase
	{
        public FriendsServiceProxy(ConnectionManager connectionManager) : base(connectionManager)
        {
        }


		public Task<UserFriendsResponse> GetFriends(UserFriendsRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<UserFriendsResponse>(request);
		}

		public Task<AddToFriendsResponse> AddToFriends(AddToFriendsRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<AddToFriendsResponse>(request);
		}

		public Task<RemoveFromFriendsResponse> RemoveFromFriends(RemoveFromFriendsRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<RemoveFromFriendsResponse>(request);
		}
		
	}


	
    /// <summary>
    /// Autogenerated proxy for AppController
    /// </summary>
	public partial class ProfileServiceProxy : ServiceProxyBase
	{
        public ProfileServiceProxy(ConnectionManager connectionManager) : base(connectionManager)
        {
        }


		public Task<ChangePhotoResponse> ChangePhoto(ChangePhotoRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<ChangePhotoResponse>(request);
		}
		
	}


	
    /// <summary>
    /// Autogenerated proxy for AppController
    /// </summary>
	public partial class RegistrationServiceProxy : ServiceProxyBase
	{
        public RegistrationServiceProxy(ConnectionManager connectionManager) : base(connectionManager)
        {
        }


		public Task<RegistrationResponse> RegisterNewUser(RegistrationRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<RegistrationResponse>(request);
		}

		public Task<ResponseBase> Deactivate(DeactivationRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<ResponseBase>(request);
		}
		
	}


	
    /// <summary>
    /// Autogenerated proxy for AppController
    /// </summary>
	public partial class UsersSearchServiceProxy : ServiceProxyBase
	{
        public UsersSearchServiceProxy(ConnectionManager connectionManager) : base(connectionManager)
        {
        }


		public Task<UsersSearchResponse> SearchUser(UsersSearchRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<UsersSearchResponse>(request);
		}

		public Task<GetUserDetailsResponse> GetUserDetails(GetUserDetailsRequest request)
		{
			return null;
			//return ConnectionManager.SendRequestAndWaitResponse<GetUserDetailsResponse>(request);
		}
		
	}

}