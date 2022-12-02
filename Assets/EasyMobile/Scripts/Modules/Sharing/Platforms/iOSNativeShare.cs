using UnityEngine;
using System.Collections;

#if UNITY_IOS
using System.Runtime.InteropServices;

namespace EasyMobile.SharingInternal
{
    internal static class iOSNativeShare
    {
        private struct ShareData
        {
            public string text;
            public string url;
            public string image;
            public string subject;
        }

        [DllImport("__Internal")]
        private static extern void _Share(ref ShareData data);

        internal static void ShareText(string text, string subject = "")
        {
            ShareData data = new ShareData();
            data.text = text;
            data.url = "";
            data.image = "";
            data.subject = subject;

            _Share(ref data);
        }

        internal static void ShareURL(string url, string subject = "")
        {
            ShareData data = new ShareData();
            data.text = "";
            data.url = url;
            data.image = "";
            data.subject = subject;

            _Share(ref data);
        }

        internal static void ShareImage(string imagePath, string message, string subject = "")
        {
            ShareData data = new ShareData();
            data.text = message; 
            data.url = "";
            data.image = imagePath;
            data.subject = subject;

            _Share(ref data);
        }
    }
}
#endif
