﻿using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Xamarin.Forms;
using ZoomDemo.Interfaces;
using ZoomDemo.iOS.Platform;
using Zoomios;

[assembly: Dependency(typeof(ZoomService))]
namespace ZoomDemo.iOS.Platform
{
    public class ZoomService : IZoomService
    {
        MobileRTC mobileRTC;

        public ZoomService()
        {
            mobileRTC = MobileRTC.SharedRTC;
        }

        public void InitZoomLib(string appKey, string appSecret)
        {
            mobileRTC.Initialize(new MobileRTCSDKInitContext
            {
                EnableLog = true,
                Domain = "zoom.us"
            });

            var authService = mobileRTC.GetAuthService();
            if (authService != null)
            {
                authService.Delegate = new MobileDelegate();
                authService.ClientKey = appKey;
                authService.ClientSecret = appSecret;

                authService.SdkAuth();

                //Only if you are using screen sharing (Broadcast Upload Extension)

                mobileRTC.SetAppGroupsName("group.com.syndew.zoomdemo");
            }
        }

        public bool IsInitialized()
        {
            return mobileRTC?.IsRTCAuthorized() ?? false;
        }

        public void JoinMeeting(string meetingID, string meetingPassword, string displayName = "Zoom Demo")
        {
            if (IsInitialized())
            {
                var meetingService = mobileRTC.GetMeetingService();
                meetingService.Delegate = new ZoomMeetingEventHandler();
                var meetingParamDict = new Dictionary<string, string>
                {
                    { Constants.kMeetingParam_Username, displayName },
                    { Constants.kMeetingParam_MeetingNumber, meetingID },
                    { Constants.kMeetingParam_MeetingPassword, meetingPassword },

                };

                var nsMeetingParam = NSDictionary.FromObjectsAndKeys(meetingParamDict.Values.ToArray(), meetingParamDict.Keys.ToArray());

                var meetingJoinResponse = meetingService.JoinMeetingWithDictionary(nsMeetingParam);

                Console.WriteLine($"Meeting Joining Response {meetingJoinResponse}");
            }
        }

        //void listMeeting()
        //{
        //    if (IsInitialized())
        //    {
        //        var premeetingService = mobileRTC.GetPreMeetingService();
        //        premeetingService.Delegate = new MobilePreDelegate();

        //        var listSuccess = premeetingService.ListMeeting;
        //    }
        //}

        public void LeaveMeeting(bool endMeeting = false)
        {
            if (IsInitialized())
            {
                var meetingService = mobileRTC.GetMeetingService();
                meetingService.LeaveMeetingWithCmd(endMeeting ? LeaveMeetingCmd.End : LeaveMeetingCmd.Leave);
            }
        }

        class MobileDelegate : MobileRTCAuthDelegate
        {
            public override void OnMobileRTCAuthReturn(MobileRTCAuthError returnValue)
            {
                Console.WriteLine($"Another Log from our iOS counterpart: {returnValue}");
            }
        }


        //class MobilePreDelegate : MobileRTCPremeetingDelegate
        //{
        //    public override void SinkDeleteMeeting(PreMeetingError result)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public override void SinkEditMeeting(PreMeetingError result, ulong uniquedID)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public override void SinkListMeeting(PreMeetingError result, NSObject[] array)
        //    {
        //        //result == 
        //        //var newList 
        //    }

        //    public override void SinkSchedultMeeting(PreMeetingError result, ulong uniquedID)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
    }
}