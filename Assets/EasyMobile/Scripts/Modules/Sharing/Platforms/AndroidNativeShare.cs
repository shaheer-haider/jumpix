#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using EasyMobile.Internal;

namespace EasyMobile.SharingInternal
{
    internal static class AndroidNativeShare
    {
        private static readonly string ANDROID_JAVA_UTILITY_CLASS = "com.sglib.easymobile.androidnative.EMUtility";

        internal static void ShareTextOrURL(string textOrURL, string subject = "")
        {
            AndroidUtil.CallJavaStaticMethod(ANDROID_JAVA_UTILITY_CLASS, "ShareTextOrURL", textOrURL, subject);
        }

        internal static void ShareImage(string imagePath, string message, string subject = "")
        {    
            AndroidUtil.CallJavaStaticMethod(ANDROID_JAVA_UTILITY_CLASS, "ShareImage", imagePath, message, subject);           
        }
    }
}
#endif
