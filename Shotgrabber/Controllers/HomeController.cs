using System;
using System.Threading;
using System.Web.Mvc;
using VideoOS.Mobile.Portable.MetaChannel;
using VideoOS.Mobile.Portable.Utilities;
using VideoOS.Mobile.Portable.VideoChannel.Params;
using VideoOS.Mobile.SDK.Portable.Server.Base.CommandResults;
using VideoOS.Mobile.SDK.Portable.Server.Base.Connection;

namespace Shotgrabber.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Uri uri = new Uri("http://50.79.232.137:8081");
            // Initialize the Mobile SDK
            VideoOS.Mobile.SDK.Environment.Instance.Initialize();
            var channelType = 0 == string.Compare(uri.Scheme, "http", StringComparison.InvariantCultureIgnoreCase)
                ? ChannelTypes.HTTP
                : ChannelTypes.HTTPSecure;
            var Connection = new Connection(channelType, uri.Host, (uint)uri.Port)
            {
                CommandsQueueing = CommandsQueueing.SingleThread
            };

            var connectResponse = Connection.Connect(null, TimeSpan.FromSeconds(15));
            if (connectResponse.ErrorCode != ErrorCodes.Ok)
                throw new Exception("Not connected to surveillance server");


            var loginResponse = Connection.LogIn("vic.uzumeri@outlook.com", "JVstacks303!", ClientTypes.MobileClient, TimeSpan.FromSeconds(15));
            if (loginResponse.ErrorCode != ErrorCodes.Ok)
                throw new Exception("Not loged in to the surveillance server");

            var cameras = Connection.Views.GetAllViewsAndCameras(TimeSpan.FromSeconds(30));

            var videoParams = new VideoParams()
            {
                CameraId = new Guid("{eea4f88b-dee6-42ad-ae03-ba9d358ab7ac}"),
                DestWidth = 640,
                DestHeight = 480,
                CompressionLvl = 100
            };
            var thumbnail = Connection.Thumbnail.GetThumbnail(videoParams, TimeSpan.FromSeconds(30));

            ViewBag.Title = "Shot";

            ViewData["Thumbnail"] = thumbnail.Thumbnail;

            return View();
        }
    }
}
