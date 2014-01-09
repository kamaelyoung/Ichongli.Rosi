using Caliburn.Micro;
using Ichongli.Rosi.Interfaces;
using Ichongli.Rosi.Services;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Parse;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Ichongli.Rosi
{
    public partial class AppBootstrapper : PhoneBootstrapper
    {
        enum SessionType
        {
            None,
            Home,
            DeepLink
        }

        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        // Set to Home when the app is launched from Primary tile.
        // Set to DeepLink when the app is launched from Deep Link.
        private SessionType sessionType = SessionType.None;

        // Set to true when the page navigation is being reset 
        bool wasRelaunched = false;

        // set to true when 5 min passed since the app was relaunched
        bool mustClearPagestack = false;

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.           
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Monitor deep link launching 
            RootFrame.Navigating += RootFrame_Navigating;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Event handler for the Navigating event of the root frame. Use this handler to modify
        // the default navigation behavior.
        void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {

            // If the session type is None or New, check the navigation Uri to determine if the
            // navigation is a deep link or if it points to the app's main page.
            if (sessionType == SessionType.None && e.NavigationMode == NavigationMode.New)
            {
                // This block will run if the current navigation is part of the app's intial launch


                // Keep track of Session Type 
                if (e.Uri.ToString().Contains("DeepLink=true"))
                {
                    sessionType = SessionType.DeepLink;
                }
                else if (e.Uri.ToString().Contains("/MainPage.xaml"))
                {
                    sessionType = SessionType.Home;
                }
            }


            if (e.NavigationMode == NavigationMode.Reset)
            {
                // This block will execute if the current navigation is a relaunch.
                // If so, another navigation will be coming, so this records that a relaunch just happened
                // so that the next navigation can use this info.
                wasRelaunched = true;
            }
            else if (e.NavigationMode == NavigationMode.New && wasRelaunched)
            {
                // This block will run if the previous navigation was a relaunch
                wasRelaunched = false;

                if (e.Uri.ToString().Contains("DeepLink=true"))
                {
                    // This block will run if the launch Uri contains "DeepLink=true" which
                    // was specified when the secondary tile was created in MainPage.xaml.cs

                    sessionType = SessionType.DeepLink;
                    // The app was relaunched via a Deep Link.
                    // The page stack will be cleared.
                }
                else if (e.Uri.ToString().Contains("/MainPage.xaml"))
                {
                    // This block will run if the navigation Uri is the main page
                    if (sessionType == SessionType.DeepLink)
                    {
                        // When the app was previously launched via Deep Link and relaunched via Main Tile, we need to clear the page stack. 
                        sessionType = SessionType.Home;
                    }
                    else
                    {
                        if (!mustClearPagestack)
                        {
                            //The app was previously launched via Main Tile and relaunched via Main Tile. Cancel the navigation to resume.
                            e.Cancel = true;
                            RootFrame.Navigated -= ClearBackStackAfterReset;
                        }
                    }
                }

                mustClearPagestack = false;
            }
        }
        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        protected override void OnActivate(object sender, ActivatedEventArgs e)
        {
            // If some interval has passed since the app was deactivated (30 seconds in this example),
            // then remember to clear the back stack of pages
            mustClearPagestack = CheckDeactivationTimeStamp();


            // If IsApplicationInstancePreserved is not true, then set the session type to the value
            // saved in isolated storage. This will make sure the session type is correct for an
            // app that is being resumed after being tombstoned.
            if (!e.IsApplicationInstancePreserved)
            {
                RestoreSessionType();
            }
        }

        protected override void OnClose(object sender, ClosingEventArgs e)
        {
            // When the application closes, delete any deactivation settings from isolated storage
            RemoveCurrentDeactivationSettings();
        }

        protected override void OnDeactivate(object sender, DeactivatedEventArgs e)
        {
            // When the applicaiton is deactivated, save the current deactivation settings to isolated storage
            SaveCurrentDeactivationSettings();
        }

        protected async override void OnLaunch(object sender, LaunchingEventArgs e)
        {
            await ParseAnalytics.TrackAppOpenedAsync();
            // When a new instance of the app is launched, clear all deactivation settings
            RemoveCurrentDeactivationSettings();
        }

        protected async override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            InitializePhoneApplication();
            ParseAnalytics.TrackAppOpens(RootFrame);

            // By convention, the empty string is considered a "Broadcast" channel
            // Note that we had to add "async" to the OnLaunched definition to use the await keyword
            await ParsePush.SubscribeAsync("");

            base.OnStartup(sender, e);
        }

        protected override void OnUnhandledException(object sender, System.Windows.ApplicationUnhandledExceptionEventArgs e)
        {

            //写入log信息到IsoFile中
            var sbLogInfo = new StringBuilder();

            sbLogInfo.AppendLine("ExceptionType: " + e.ExceptionObject.Message);
            sbLogInfo.AppendLine("ExceptionInfo: " + e.ExceptionObject);

            string logInfo = sbLogInfo.ToString();

            IChongliHelper.SubmitRespond(DateTime.Now + " :" + logInfo);

            if (Debugger.IsAttached)
            {
                // 出现未处理的异常；强行进入调试器
                Debugger.Break();
            }
        }

        // Helper method for adding or updating a key/value pair in isolated storage
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        // Helper method for removing a key/value pair from isolated storage
        public void RemoveValue(string Key)
        {
            // If the key exists
            if (settings.Contains(Key))
            {
                settings.Remove(Key);
            }
        }

        // Called when the app is deactivating. Saves the time of the deactivation and the 
        // session type of the app instance to isolated storage.
        public void SaveCurrentDeactivationSettings()
        {
            if (AddOrUpdateValue("DeactivateTime", DateTimeOffset.Now))
            {
                settings.Save();
            }

            if (AddOrUpdateValue("SessionType", sessionType))
            {
                settings.Save();
            }

        }

        // Called when the app is launched or closed. Removes all deactivation settings from
        // isolated storage
        public void RemoveCurrentDeactivationSettings()
        {
            RemoveValue("DeactivateTime");
            RemoveValue("SessionType");
            settings.Save();
        }

        // Helper method to determine if the interval since the app was deactivated is
        // greater than 30 seconds
        bool CheckDeactivationTimeStamp()
        {
            DateTimeOffset lastDeactivated;

            if (settings.Contains("DeactivateTime"))
            {
                lastDeactivated = (DateTimeOffset)settings["DeactivateTime"];
            }

            var currentDuration = DateTimeOffset.Now.Subtract(lastDeactivated);

            return TimeSpan.FromSeconds(currentDuration.TotalSeconds) > TimeSpan.FromSeconds(30);
        }

        // Helper method to restore the session type from isolated storage.
        void RestoreSessionType()
        {
            if (settings.Contains("SessionType"))
            {
                sessionType = (SessionType)settings["SessionType"];
            }
        }
    }
}
