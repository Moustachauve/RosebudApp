using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using FragmentSupport = Android.Support.V4.App.Fragment;
using MyTransitCore.Utils;
using Android.Views.Animations;
using System.Threading.Tasks;
using MyTransitCore.DataAccessor;
using Android.Support.Design.Widget;

namespace MyTransitAndroid.Fragments
{
    public class NetworkStatusFragment : FragmentSupport
    {
        private TextView lblStatus;
        private LinearLayout statusContainer;
        private Animation slideUpAnimation;
        private Animation slideDownAnimation;

        private NetworkState currentState;

        public EventHandler RetryLastRequest;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            slideUpAnimation = AnimationUtils.LoadAnimation(Application.Context, Resource.Animation.slide_up);
            slideDownAnimation = AnimationUtils.LoadAnimation(Application.Context, Resource.Animation.slide_down);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.network_status, container, false);

            statusContainer = v.FindViewById<LinearLayout>(Resource.Id.status_container);
            lblStatus = v.FindViewById<TextView>(Resource.Id.lbl_status);

            return v;
        }

        public override async void OnResume()
        {
            NetworkState newState = Dependency.NetworkStatusMonitor.State;
            Dependency.NetworkStatusMonitor.StateChanged += OnStateChanged;
            HttpHelper.ServerErrorOccured += OnServerErrorOccured;
           
            base.OnResume();
            await SetState(newState);
        }

        public override void OnPause()
        {
            Dependency.NetworkStatusMonitor.StateChanged -= OnStateChanged;
            HttpHelper.ServerErrorOccured -= OnServerErrorOccured;

            base.OnPause();
        }

        public void UpdateState()
        {
            Dependency.NetworkStatusMonitor.UpdateState();
        }

        protected async void OnStateChanged(object sender, NetworkState newState)
        {
            await SetState(newState);
        }

        private async Task SetState(NetworkState newState)
        {
            if(newState == currentState)
            {
                return;
            }

            if(currentState != NetworkState.ConnectedData && currentState != NetworkState.ConnectedWifi)
            {
                await SlideUp();
            }
            if (newState == NetworkState.ConnectedData || newState == NetworkState.ConnectedWifi)
            {
                return;
            }

            lblStatus.Text = Resources.GetText(Resource.String.network_no_connection);

            currentState = newState;
            await SlideDown();
        }

        private async Task SlideUp()
        {
            statusContainer.StartAnimation(slideUpAnimation);
            await WaitForAnimationComplete();
            statusContainer.Visibility = ViewStates.Gone;
        }

        private async Task SlideDown()
        {
            statusContainer.StartAnimation(slideDownAnimation);
            statusContainer.Visibility = ViewStates.Visible;
            await WaitForAnimationComplete();
        }

        protected async Task WaitForAnimationComplete()
        {
            await Task.Run(() =>
            {
                do
                {
                    System.Threading.Thread.Sleep(350); // arbitrary sleep
                }
                while (statusContainer.Animation != null && !statusContainer.Animation.HasEnded);
            });
        }

        private void OnServerErrorOccured()
        {
            ViewGroup parentView = (ViewGroup)((ViewGroup)Activity.FindViewById(Android.Resource.Id.Content)).GetChildAt(0);

            var snack = Snackbar.Make(parentView, Resource.String.network_server_error, Snackbar.LengthIndefinite);

            if (RetryLastRequest != null)
            {
                snack.SetAction(Resource.String.network_retry, (View v) =>
                {
                    snack.Dismiss();
                    RetryLastRequest?.Invoke(this, EventArgs.Empty);
                });
            }

            snack.Show();
        }

    }
}