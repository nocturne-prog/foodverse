using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Marvrus.Util
{
    public static class PlayerPrefsManager
    {

        public enum Key
        {
            RefreshToken,
            Id,
            Client_Id,
            Auto_Login,
            Guest_Login,
            SNS_Login,
            Reception_Notice,
            Reception_Marketing,
            Reception_Night,

        }


        /// <summary>
        /// PlayerPrefs.Clear 할 때, 예외되는 키 값.
        /// </summary>
        public enum Key_Not_Deleted
        {
            PermissionCheck,
            Onboarding,
            Onboarding_Write,
            NativeGalleryPermission,
            Ask_NativeGalleryPermission,
            NativeCameraPermission,
            Ask_NativeCameraPermission,
            KeyboardHeight
        }

        #region Base.
        public static string GetPlayerPrefs(Key _key, string _defaultValue = null)
        {
            return GetPlayerPrefs(_key.ToString(), _defaultValue);
        }

        public static string GetPlayerPrefs(Key_Not_Deleted _key, string _defaultValue = null)
        {
            return GetPlayerPrefs(_key.ToString(), _defaultValue);
        }

        public static int GetPlayerPrefs(Key _key, int _defaultValue = 0)
        {
            return GetPlayerPrefs(_key.ToString(), _defaultValue);
        }

        public static int GetPlayerPrefs(Key_Not_Deleted _key, int _defaultValue = 0)
        {
            return GetPlayerPrefs(_key.ToString(), _defaultValue);
        }

        public static float GetPlayerPrefs(Key _key, float _defaultValue = 0)
        {
            return GetPlayerPrefs(_key.ToString(), _defaultValue);
        }

        public static float GetPlayerPrefs(Key_Not_Deleted _key, float _defaultValue = 0)
        {
            return GetPlayerPrefs(_key.ToString(), _defaultValue);
        }

        public static string GetPlayerPrefs(string _key, string _defaultValue = null)
        {
            if (string.IsNullOrEmpty(_defaultValue))
                return PlayerPrefs.GetString(_key);

            return PlayerPrefs.GetString(_key, _defaultValue);
        }

        public static int GetPlayerPrefs(string _key, int _defaultValue = 0)
        {
            if (_defaultValue == 0)
                return PlayerPrefs.GetInt(_key);

            return PlayerPrefs.GetInt(_key, _defaultValue);
        }

        public static float GetPlayerPrefs(string _key, float _defaultValue = 0)
        {
            if (_defaultValue == 0)
                return PlayerPrefs.GetFloat(_key);

            return PlayerPrefs.GetFloat(_key, _defaultValue);
        }

        public static void SetPlayerPrefs(Key _key, string _value)
        {
            SetPlayerPrefs(_key.ToString(), _value);
        }

        public static void SetPlayerPrefs(Key_Not_Deleted _key, string _value)
        {
            SetPlayerPrefs(_key.ToString(), _value);
        }

        public static void SetPlayerPrefs(Key _key, int _value)
        {
            SetPlayerPrefs(_key.ToString(), _value);
        }

        public static void SetPlayerPrefs(Key_Not_Deleted _key, int _value)
        {
            SetPlayerPrefs(_key.ToString(), _value);
        }

        public static void SetPlayerPrefs(Key _key, float _value)
        {
            SetPlayerPrefs(_key.ToString(), _value);
        }

        public static void SetPlayerPrefs(Key_Not_Deleted _key, float _value)
        {
            SetPlayerPrefs(_key.ToString(), _value);
        }

        public static void SetPlayerPrefs(string _key, string _value)
        {
            PlayerPrefs.SetString(_key, _value);
            PlayerPrefs.Save();
        }

        public static void SetPlayerPrefs(string _key, int _value)
        {
            PlayerPrefs.SetInt(_key, _value);
            PlayerPrefs.Save();
        }

        public static void SetPlayerPrefs(string _key, float _value)
        {
            PlayerPrefs.SetFloat(_key, _value);
            PlayerPrefs.Save();
        }

        public static bool HasKey(string _key)
        {
            return PlayerPrefs.HasKey(_key);
        }

        public static void DeleteKey(string _key)
        {
            PlayerPrefs.DeleteKey(_key);
        }

        public static void Clear()
        {
            foreach(Key v in Enum.GetValues(typeof(Key)))
            {
                DeleteKey(v.ToString());
            }
        }
        #endregion

        public static string RefreshToken
        {
            get { return GetPlayerPrefs(Key.RefreshToken, ""); }
            set { SetPlayerPrefs(Key.RefreshToken, value); }
        }

        public static string ID
        {
            get { return GetPlayerPrefs(Key.Id, ""); }
            set { SetPlayerPrefs(Key.Id, value); }
        }

        public static string ClientID
        {
            get
            {
                string id = GetPlayerPrefs(Key.Client_Id, "");

                if (string.IsNullOrEmpty(id))
                {
                    id = CreateClientID(30);
                    SetPlayerPrefs(Key.Client_Id, id);
                }

                return id;
            }
        }

        private static string CreateClientID(int _length)
        {
            using (var crypto = new RNGCryptoServiceProvider())
            {
                var bits = (_length * 6);
                var byte_size = ((bits + 7) / 8);
                var bytesarray = new byte[byte_size];
                crypto.GetBytes(bytesarray);

                string random = Convert.ToBase64String(bytesarray);
                Debug.Log($"random clientID : {random}");

                random = random.Replace("=", "");
                return random;
            }
        }

        public static bool AutoLogin
        {
            get { return GetPlayerPrefs(Key.Auto_Login, 0) == 1; }
            set { SetPlayerPrefs(Key.Auto_Login, value ? 1 : 0); }
        }

        public static bool SNS_Login
        {
            get { return GetPlayerPrefs(Key.SNS_Login, 0) == 1; }
            set { SetPlayerPrefs(Key.SNS_Login, value ? 1 : 0); }
        }

        public static bool GuestLogin
        {
            get { return GetPlayerPrefs(Key.Guest_Login, 0) == 1; }
            set { SetPlayerPrefs(Key.Guest_Login, value ? 1 : 0); }
        }

        public static bool FirstCheckPermission
        {
            get { return GetPlayerPrefs(Key_Not_Deleted.PermissionCheck, 0) == 1; }
            set { SetPlayerPrefs(Key_Not_Deleted.PermissionCheck, value ? 1 : 0); }
        }

        public static bool DontLookAgainOnboarding
        {
            get { return GetPlayerPrefs(Key_Not_Deleted.Onboarding, 0) == 1; }
            set { SetPlayerPrefs(Key_Not_Deleted.Onboarding, value ? 1 : 0); }
        }

        public static bool DontLookAgainOnboardingWrite
        {
            get { return GetPlayerPrefs(Key_Not_Deleted.Onboarding_Write, 0) == 1; }
            set { SetPlayerPrefs(Key_Not_Deleted.Onboarding_Write, value ? 1 : 0); }
        }

        public static bool ReceptionNotice
        {
            get { return GetPlayerPrefs(Key.Reception_Notice, 0) == 1; }
            set { SetPlayerPrefs(Key.Reception_Notice, value ? 1 : 0); }
        }

        public static bool ReceptionMarketing
        {
            get { return GetPlayerPrefs(Key.Reception_Marketing, 0) == 1; }
            set { SetPlayerPrefs(Key.Reception_Marketing, value ? 1 : 0); }
        }

        public static bool ReceptionNight
        {
            get { return GetPlayerPrefs(Key.Reception_Night, 0) == 1; }
            set { SetPlayerPrefs(Key.Reception_Night, value ? 1 : 0); }
        }

        public static int NativeCamera
        {
            get { return GetPlayerPrefs(Key_Not_Deleted.NativeCameraPermission, -1); }
            set { SetPlayerPrefs(Key_Not_Deleted.NativeCameraPermission, value); }
        }

        public static int NativeGallery
        {
            get { return GetPlayerPrefs(Key_Not_Deleted.NativeGalleryPermission, -1); }
            set { SetPlayerPrefs(Key_Not_Deleted.NativeGalleryPermission, value); }
        }

        public static bool Ask_NativeCamera_Permission
        {
            get { return GetPlayerPrefs(Key_Not_Deleted.Ask_NativeCameraPermission, 0) == 1; }
            set { SetPlayerPrefs(Key_Not_Deleted.Ask_NativeCameraPermission, value ? 1 : 0); }
        }

        public static bool Ask_NativeGallery_Permission
        {
            get { return GetPlayerPrefs(Key_Not_Deleted.NativeGalleryPermission, 0) == 1; }
            set { SetPlayerPrefs(Key_Not_Deleted.NativeGalleryPermission, value ? 1 : 0); }
        }

        public static int KeyboardHeight
        {
            get
            {
                int value = GetPlayerPrefs(Key_Not_Deleted.KeyboardHeight, 0);

                if (value == 0)
                {
                    int height = KeyboardArea.Height;
                    SetPlayerPrefs(Key_Not_Deleted.KeyboardHeight, height);
                    return height;
                }
                else
                    return value;
            }
        }
    }
}