using System;
using UnityEngine.Android;
using UnityEngine;
using PPM = Marvrus.Util.PlayerPrefsManager;

namespace Marvrus.Util
{
    public enum PermissionType
    {
        Read = 0,
        Write,
        Camera,
        Microphone,
        FineLocation,
        CoarseLocation,
    }

    public enum PermissionCallbackType
    {
        PermissionGranted = 0,
        PermissionDenied,
        PermissionDeniedAndDontAskAgain
    }

    public static class PermissionCheck
    {
        public static bool CheckPermission(PermissionType _type)
        {
            string permission = ParseString(_type);

            if (string.IsNullOrEmpty(permission) is true)
                return false;

            return Permission.HasUserAuthorizedPermission(permission);
        }

        public static void RequestPermission(PermissionType _type, Action<PermissionCallbackType> _callback)
        {
            RequestPermission(_type,
                (granted) => { _callback(PermissionCallbackType.PermissionGranted); },
                (denied) => { _callback(PermissionCallbackType.PermissionDenied); },
                (dontAsk) => { _callback(PermissionCallbackType.PermissionDenied); });
        }

        public static void RequestPermissions(PermissionType[] _type, Action<PermissionCallbackType> _callback)
        {
            RequestPermissions(_type,
                (granted) => { _callback(PermissionCallbackType.PermissionGranted); },
                (denied) => { _callback(PermissionCallbackType.PermissionDenied); },
                (dontAsk) => { _callback(PermissionCallbackType.PermissionDenied); });
        }

        private static string ParseString(PermissionType _type)
        {
            switch (_type)
            {
                case PermissionType.Read: return Permission.ExternalStorageRead;
                case PermissionType.Write: return Permission.ExternalStorageWrite;
                case PermissionType.Camera: return Permission.Camera;
                case PermissionType.Microphone: return Permission.Microphone;
                case PermissionType.FineLocation: return Permission.FineLocation;
                case PermissionType.CoarseLocation: return Permission.CoarseLocation;

                default: return string.Empty;
            }
        }

        private static void RequestPermission(PermissionType _type, Action<string> _granted, Action<string> _denied, Action<string> _dontAsk)
        {
            PermissionCallbacks callback = new PermissionCallbacks();
            callback.PermissionGranted += _granted;
            callback.PermissionDenied += _denied;
            callback.PermissionDeniedAndDontAskAgain += _dontAsk;

            Permission.RequestUserPermission(ParseString(_type), callback);
        }

        private static void RequestPermissions(PermissionType[] _type, Action<string> _granted, Action<string> _denied, Action<string> _dontAsk)
        {
            PermissionCallbacks callback = new PermissionCallbacks();
            callback.PermissionGranted += _granted;
            callback.PermissionDenied += _denied;
            callback.PermissionDeniedAndDontAskAgain += _dontAsk;

            string[] type = new string[_type.Length];

            for (int i = 0; i < type.Length; i++)
            {
                type[i] = ParseString(_type[i]);
            }

            Permission.RequestUserPermissions(type, callback);
        }

        public static int ParseNativePluginPermissionValue(PermissionCallbackType _type)
        {
            switch (_type)
            {
                case PermissionCallbackType.PermissionGranted:
                    return 1;

                case PermissionCallbackType.PermissionDenied:
                    return 0;

                case PermissionCallbackType.PermissionDeniedAndDontAskAgain:
                    return 2;
            }

            return -1;
        }

        public static void RequestGallery(Action _grantedCallback)
        {
            NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Read);

            switch (permission)
            {
                case NativeGallery.Permission.Granted:
                    _grantedCallback();
                    break;

                case NativeGallery.Permission.Denied:
                case NativeGallery.Permission.ShouldAsk:

                    if (PPM.Ask_NativeGallery_Permission is true)
                    {
                        UIManager.s.OpenTwoButton(
                        _title: Const.POPUP_PERMISSION_GALLERY_DENIED_REQUEST_TITLE,
                        _desc: Const.POPUP_PERMISSION_GALLERY_DENIED_REQUEST_DESC,
                        _left: null,
                        _leftText: Const.CLOSE,
                        _right: () => NativeGallery.OpenSettings(),
                        _rightText: Const.POPUP_PERMISSION_GO_SETTING);
                    }
                    else
                    {
                        RequestPermission(PermissionType.Read, (result) =>
                        {
                            PPM.NativeGallery = ParseNativePluginPermissionValue(result);
                            PPM.Ask_NativeGallery_Permission = true;

                            switch (result)
                            {
                                case PermissionCallbackType.PermissionGranted:
                                    _grantedCallback();
                                    break;

                                case PermissionCallbackType.PermissionDenied:
                                case PermissionCallbackType.PermissionDeniedAndDontAskAgain:

                                    UIManager.s.OpenTwoButton(
                                   _title: "",
                                   _desc: Const.POPUP_PERMISSION_GALLERY_DENIED,
                                   _left: null,
                                   _leftText: Const.CLOSE,
                                   _right: () => NativeGallery.OpenSettings(),
                                   _rightText: Const.POPUP_PERMISSION_GO_SETTING);

                                    break;
                            }
                        });
                    }
                    break;
            }
        }

        public static void RequestCamera(Action _grantedCallback)
        {
            NativeCamera.Permission permission = NativeCamera.CheckPermission();

            switch (permission)
            {
                case NativeCamera.Permission.Granted:
                    _grantedCallback();
                    break;

                case NativeCamera.Permission.Denied:
                case NativeCamera.Permission.ShouldAsk:

                    if (PPM.Ask_NativeCamera_Permission is true)
                    {
                        UIManager.s.OpenTwoButton(
                        _title: Const.POPUP_PERMISSION_CAMERA_DENIED_REQUEST_TITLE,
                        _desc: Const.POPUP_PERMISSION_CAMERA_DENIED_REQUEST_DESC,
                        _left: null,
                        _leftText: Const.CLOSE,
                        _right: () => NativeCamera.OpenSettings(),
                        _rightText: Const.POPUP_PERMISSION_GO_SETTING);
                    }
                    else
                    {
                        RequestPermission(PermissionType.Read, (result) =>
                        {
                            PPM.NativeCamera = ParseNativePluginPermissionValue(result);
                            PPM.Ask_NativeCamera_Permission = true;

                            switch (result)
                            {
                                case PermissionCallbackType.PermissionGranted:
                                    _grantedCallback();
                                    break;

                                case PermissionCallbackType.PermissionDenied:
                                case PermissionCallbackType.PermissionDeniedAndDontAskAgain:

                                    UIManager.s.OpenTwoButton(
                                   _title: "",
                                   _desc: Const.POPUP_PERMISSION_CAMERA_DENIED,
                                   _left: null,
                                   _leftText: Const.CLOSE,
                                   _right: () => NativeCamera.OpenSettings(),
                                   _rightText: Const.POPUP_PERMISSION_GO_SETTING);

                                    break;
                            }
                        });
                    }
                    break;
            }
        }
    }
}