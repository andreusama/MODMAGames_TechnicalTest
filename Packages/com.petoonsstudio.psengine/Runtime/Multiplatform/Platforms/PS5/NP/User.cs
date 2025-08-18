using UnityEngine;

#if UNITY_PS5 
using PetoonsStudio.PSEngine.Multiplatform.PS5;
using Unity.PSN.PS5.Aysnc;
using Unity.PSN.PS5.Sessions;
using Unity.PSN.PS5.Users;
using Unity.PSN.PS5.WebApi;
#if UNITY_PS4
using PlatformInput = UnityEngine.PS4.PS4Input;
#elif UNITY_PS5
using PlatformInput = UnityEngine.PS5.PS5Input;
#endif
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.PS5
{
    class User
    {
#if UNITY_PS5
        static User()
        {
            PlatformInput.OnUserServiceEvent += OnUserServiceEvent;
        }

#if UNITY_PS5
        static void OnUserServiceEvent(PlatformInput.UserServiceEventType eventtype, uint userid)
#elif UNITY_PS4
        static void OnUserServiceEvent(uint eventtype, uint userid)
#endif
        {
            Debug.Log("OnUserServiceEvent -> User state changed : " + eventtype);

#if UNITY_PS5
            if (eventtype == PlatformInput.UserServiceEventType.Login)
#elif UNITY_PS4
            if (eventtype == 0) // SCE_USER_SERVICE_EVENT_TYPE_LOGIN
#endif
            {
                UserLoggedIn((int)userid);
            }
#if UNITY_PS5
            else if (eventtype == PlatformInput.UserServiceEventType.Logout)
#elif UNITY_PS4
            else if (eventtype == 1)
#endif
            {
                UserLoggedOut((int)userid);
            }
        }

        public static void UserLoggedIn(int userid)
        {

            Debug.Log("[User_PS5] User logged IN");
            User user = FindUser((int)userid);

            if (user != null)
            {
                Debug.Log("[User_PS5] User login found");

                if (user.registerSequence == RegisterSequences.NotSet)
                {
                    user.registerSequence = RegisterSequences.AddingUser;

                    UserSystem.AddUserRequest request = new UserSystem.AddUserRequest() { UserId = (int)userid };

                    var requestOp = new AsyncRequest<UserSystem.AddUserRequest>(request).ContinueWith((antecedent) =>
                    {
                        if (antecedent != null && antecedent.Request != null)
                        {
                            user.registerSequence = RegisterSequences.UserAdded;
                        }
                    });

                    UserSystem.Schedule(requestOp);

                    Debug.Log("[User_PS5]User being added...");
                }
            }
            else
            {
                Debug.Log("[User_PS5] User login is null");
            }

        }

        public static void UserLoggedOut(int userid)
        {
            Debug.Log("[User_PS5] User logged OUT");

            User user = FindUser((int)userid);

            if (user != null)
            {
                if (user.registerSequence != RegisterSequences.UserLoggingOut)
                {
                    user.registerSequence = RegisterSequences.UserLoggingOut;

                    SessionsManager.UnregisterUserSessionEvent((int)userid);

                    UserSystem.RemoveUserRequest request = new UserSystem.RemoveUserRequest() { UserId = userid };

                    var requestOp = new AsyncRequest<UserSystem.RemoveUserRequest>(request).ContinueWith((antecedent) =>
                    {
                        if (antecedent != null && antecedent.Request != null)
                        {
                            User registeredUser = User.FindUser(antecedent.Request.UserId);

                            if (registeredUser != null)
                            {
                                if (PS5Manager.CheckAysncRequestOK(antecedent))
                                {
                                    Debug.Log("User Removed");

                                    registeredUser.registerSequence = RegisterSequences.NotSet;
                                }
                            }
                        }
                    });

                    UserSystem.Schedule(requestOp);

                    Debug.Log("User being removed...");
                }
            }
        }

        public static int GetActiveUserId
        {
            get
            {
                if (GamePad.activeGamePad == null)
                {
                    Debug.LogError("User.GetActiveUserId : Active Gamepad is null. Must wait until the gamepad system has had time to initialize correctly.");
                }
                return GamePad.activeGamePad.loggedInUser.userId;
            }
        }

        public static User GetActiveUser
        {
            get
            {
                if (GamePad.activeGamePad == null) return null;

                return FindUser(GamePad.activeGamePad.loggedInUser.userId);
            }
        }

        public static bool IsActiveUserAdded
        {
            get
            {
                User user = GetActiveUser;

                if (user != null)
                {
                    return user.registerSequence == RegisterSequences.UserAdded ||
                           user.registerSequence == RegisterSequences.UserRegistered ||
                           user.registerSequence == RegisterSequences.UserAddedButNoOnline;
                }
                return false;
            }
        }

        public static User[] users = new User[4];

        public static User FindUser(int userId)
        {
            for (int i = 0; i < users.Length; i++)
            {
                if (users[i] != null && users[i].gamePad != null)
                {
                    if (users[i].gamePad.loggedInUser.userId != 0 && users[i].gamePad.loggedInUser.userId == userId)
                    {
                        return users[i];
                    }
                }
            }

            return null;
        }

        public static void Initialize(GamePad[] gamePads)
        {
            for (int i = 0; i < gamePads.Length; i++)
            {
                int playerId = gamePads[i].playerId;

                users[playerId] = new User();
                users[playerId].gamePad = gamePads[i];
            }
        }

        public static void CheckRegistration()
        {
            for (int i = 0; i < users.Length; i++)
            {
                if (users[i] != null && users[i].gamePad != null)
                {
                    if (users[i].gamePad.IsConnected == true)
                    {
                        if (users[i].gamePad.loggedInUser.status == 1)
                        {
                            if (users[i].registerSequence == RegisterSequences.NotSet)
                            {
                                UserLoggedIn(users[i].gamePad.loggedInUser.userId);
                            }
                            else if (users[i].registerSequence == RegisterSequences.UserAdded)
                            {
                                if (users[i].gamePad.loggedInUser.onlineStatus == PlatformInput.OnlineStatus.SignedIn)
                                {
                                    SessionsManager.RegisterUserSessionEvent(users[i].gamePad.loggedInUser.userId);
                                    Debug.Log("SessionsManager.RegisterUserSessionEvent");
                                    users[i].registerSequence = RegisterSequences.RegisteringUser;
                                }
                                else
                                {
                                    users[i].registerSequence = RegisterSequences.UserAddedButNoOnline;
                                }
                            }
                            else if (users[i].registerSequence == RegisterSequences.RegisteringUser)
                            {
                                WebApiPushEvent pushEvent = SessionsManager.GetUserSessionPushEvent(users[i].gamePad.loggedInUser.userId);

                                // Check for registraction.
                                if (pushEvent != null )
                                {
                                    users[i].registerSequence = RegisterSequences.UserRegistered;
                                }
                            }
                            else if (users[i].registerSequence == RegisterSequences.UserAddedButNoOnline)
                            {
                                if (users[i].gamePad.loggedInUser.onlineStatus == PlatformInput.OnlineStatus.SignedIn)
                                {
                                    SessionsManager.RegisterUserSessionEvent(users[i].gamePad.loggedInUser.userId);
                                    Debug.Log("SessionsManager.RegisterUserSessionEvent");
                                    users[i].registerSequence = RegisterSequences.RegisteringUser;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (users[i].gamePad.loggedInUser.status == 0)
                        {
                            if (users[i].registerSequence == RegisterSequences.NotSet)
                            {
                                UserLoggedOut(users[i].gamePad.loggedInUser.userId);
                            }
                        }
                    }
                }
            }
        }

        public static string Output()
        {
            string userOutput = "";
            for (int i = 0; i < users.Length; i++)
            {
                if (users[i] != null && users[i].gamePad != null)
                {
                    if (users[i].gamePad.IsConnected == true)
                    {
                        if (users[i].gamePad == GamePad.activeGamePad)
                        {
                            userOutput += "-->";
                        }
                        else
                        {
                            userOutput += "    ";
                        }

                        userOutput += " (0x" + users[i].gamePad.loggedInUser.userId.ToString("X8") + ")    " + users[i].gamePad.loggedInUser.userName + "  " + users[i].gamePad.loggedInUser.onlineStatus + " " + users[i].registerSequence + "\n";
                    }
                }
            }
            return userOutput;
        }

        public enum RegisterSequences
        {
            NotSet,
            AddingUser,
            UserAdded,
            RegisteringUser,
            UserRegistered,
            UserAddedButNoOnline,
            UserLoggingOut,
        }

        public RegisterSequences registerSequence = RegisterSequences.NotSet;

        AsyncAction<AsyncRequest<WebApiNotifications.RegisterPushEventRequest>> currentRegisterRequest;
     
        public GamePad gamePad;
#endif
    }

}
