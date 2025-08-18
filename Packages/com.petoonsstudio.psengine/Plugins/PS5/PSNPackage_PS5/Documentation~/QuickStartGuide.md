
# Quick Start Quide

This will show how to setup your own project to use the PSN API.

## PS5 Title Configuration

You will need to use Sony's __UDS Management Tool__ and __Package/Disc Management Tool (GEMS)__ on DevNet to configure your titles Events, Activities, Trophies and other game releated data. 

Once the Universal Data system is configured or changes have been made use the __Package/Disc Management Tool (GEMS)__ to download the titles __npconfig.zip__ and reference this in Unity project publishing settings.

Some systems may also need configuring in the titles __param.json__ file. 

For example if the title uses __GameIntent__ you may need to config the types of intent the title supports. e.g. this may need to be added to the file.

```json
"gameIntent": {
    "permittedIntents": [
      {
        "intentType": "joinSession"
      },
      {
        "intentType": "launchActivity"
      },
      {
        "intentType": "launchMultiplayerActivity"
      }
    ]
  },
```

Please refer to the Sony documention on how to configure your title.

## Unity Initialisation

The system will first need to be initialised. Do this from a MonoBehaviour or script code as it *must* be called on the main Unity thread.

```CSharp
InitResult initResult;

try
{
    initResult = Main.Initialize();

    if (initResult.Initialized == true)
    {
        // Initialization succeeded
    }
    else
    {
        // Initialization failed
    }
}
catch (PSNException e)
{
    // Exception - See e.ExtendedMessage for more info
}
```

View [`Main`](../api/Unity.PSN.PS5.Main.html) documention for details on initialization.

## Updating

Each frame the PSN system needs an update call to handle various internal PSN callback API's. This can be done in a monobehaviour update. It *must* be called on the main Unity thread.

```CSharp
Main.Update();
```

## Registering a user

Whenever a user logs into the PS5 system or the app wants to use a User Id then user needs to be first registered with the PSN system. 

This is an asynchronous operation which will take time to complete. Don't call other methods until the user has been registered.

```csharp
// Create the request data
UserSystem.AddUserRequest request = new UserSystem.AddUserRequest() { UserId = id };

// Create the async operation and response once completed
var requestOp = new AsyncRequest<UserSystem.AddUserRequest>(request).ContinueWith((antecedent) =>
{
    // Operation completed
    // User has been registered (antecedent.Request.UserId)
});

// Schedule the request operation
UserSystem.Schedule(requestOp);
```

View [`AddUserRequest`](../api/Unity.PSN.PS5.Users.UserSystem.AddUserRequest.html) documention for details on users.

## Unregistering a user

When a user logs off from the PS5 system use [`RemoveUserRequest`](../api/Unity.PSN.PS5.Users.UserSystem.RemoveUserRequest.html) operation to unregister the user from the system. 

## Basic Usage Examples

### Unlocking a Trophy

The [`Universal Data System`](../api/Unity.PSN.PS5.UDS.html) is used to unlock trophies. To retrieve infomation about trophies use the [`Trophy System`](../api/Unity.PSN.PS5.Trophies.html)

```csharp
UniversalDataSystem.UnlockTrophyRequest request = new UniversalDataSystem.UnlockTrophyRequest()
{
    TrophyId = trophyId,
    UserId = userId
};

var getTrophyOp = new AsyncRequest<UniversalDataSystem.UnlockTrophyRequest>(request).ContinueWith((antecedent) =>
{
    // Trophy unlocked for the given user (antecedent.Request.TrophyId)
});

UniversalDataSystem.Schedule(getTrophyOp);
```

### Retrieving Friends

This example will retrieve the first 95 friends for a user (online or offline) and sorted into online status order.

```csharp
UInt32 limit = 95;

UserSystem.GetFriendsRequest request = new UserSystem.GetFriendsRequest()
{
    UserId = id,
    Offset = 0,
    Limit = limit,
    Filter = UserSystem.GetFriendsRequest.Filters.NotSet,
    SortOrder = UserSystem.GetFriendsRequest.Order.OnlineId,
    RetrievedAccountIds = new System.Collections.Generic.List<UInt64>((int)limit)
};

var requestOp = new AsyncRequest<UserSystem.GetFriendsRequest>(request).ContinueWith((antecedent) =>
{
    if (antecedent.Request.Result.apiResult == APIResultTypes.Success)
    {
        var accountIds = antecedent.Request.RetrievedAccountIds;

        for (int i = 0; i < accountIds.Count; i++)
        {
            // Process friends list
        }
    }
    else
    {
        // Error condition
    }
});

UserSystem.Schedule(requestOp);
```

