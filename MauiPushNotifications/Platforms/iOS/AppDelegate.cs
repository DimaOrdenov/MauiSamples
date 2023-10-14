// using Firebase.CloudMessaging;
using Foundation;
using Microsoft.Extensions.Logging;
using UIKit;
using UserNotifications;

namespace MauiPushNotifications;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate, IUNUserNotificationCenterDelegate
    // , IMessagingDelegate
{
    private ILogger<AppDelegate> _logger;

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        _logger = LoggerFactory.Create(b => 
            b.SetMinimumLevel(LogLevel.Trace)
                .AddConsole())
            .CreateLogger<AppDelegate>();
        
        // Firebase.Core.App.Configure();
        // Messaging.SharedInstance.Delegate = this;

        RegisterForRemoteNotifications(application);

        return base.FinishedLaunching(application, launchOptions);
    }

    [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
    public void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
    {
        _logger.LogDebug($"Did receive APNS token {deviceToken.DebugDescription}");
    }

    [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
    public void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
    {
        _logger.LogDebug($"Failed register for remote notifications {error.DebugDescription}");
    }

    // [Export("messaging:didReceiveRegistrationToken:")]
    // public void DidReceiveRegistrationToken(Messaging messaging, string token)
    // {
    //     _logger.LogDebug($"Did receive FCM token {token}");
    // }

    [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
    public void WillPresentNotification(UNUserNotificationCenter notificationCenter, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
    {
        _logger.LogDebug($"Will present {notification}");

        completionHandler(UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Sound | UNNotificationPresentationOptions.Badge);
    }

    [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
    public void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
    {
        _logger.LogDebug($"Did receive remote notification {userInfo}");

        completionHandler.Invoke(UIBackgroundFetchResult.NewData);
    }

    private void RegisterForRemoteNotifications(UIApplication application)
    {
        UNUserNotificationCenter.Current.Delegate = this;

        UNUserNotificationCenter.Current.RequestAuthorization(
            UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
            (isSuccess, error) =>
            {
                _logger.LogDebug($"Registration is success: {isSuccess}");

                if (error != null)
                {
                    _logger.LogDebug($"Registration for push notifications error: {error.DebugDescription}");
                }
            });

        application.RegisterForRemoteNotifications();
    }
}
