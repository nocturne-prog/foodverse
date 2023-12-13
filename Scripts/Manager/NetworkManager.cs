using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using BestHTTP;
using DC = Marvrus.Data.DataContainer;
using Marvrus.Util;

namespace Marvrus
{

    public class NetworkManager : Singleton<NetworkManager>
    {
        #region Baase
        public bool isThreadRunning { get; private set; } = false;
        Queue<IEnumerator> queue = new Queue<IEnumerator>();

        public void Send_Immediately<T>(string _uri, string _data, HTTPMethods _method, Action<T> _success = null, Action<string> _fail = null, bool _isIncludeToken = false, bool _isRetry = false, bool _isAuth = false)
        {
            StartCoroutine(Co_Send(_uri, _data, _method, _success, _fail, _isIncludeToken, _isRetry, _isAuth));
        }

        public void Send(string _uri, HTTPMethods _method, Action<string> _success = null, Action<string> _fail = null, bool _isIncludeToken = false, bool _isRetry = false, bool _isAuth = false)
        {
            Send<string>(_uri, string.Empty, _method, _success, _fail, _isIncludeToken, _isRetry, _isAuth);
        }

        public void Send<T>(string _uri, string _data, HTTPMethods _method, Action<T> _success = null, Action<string> _fail = null, bool _isIncludeToken = false, bool _isRetry = false, bool _isAuth = false)
        {
            queue.Enqueue(Co_Send(_uri, _data, _method, _success, _fail, _isIncludeToken, _isRetry, _isAuth));

            if (isThreadRunning is false)
            {
                StartCoroutine(ThreadQueue());
            }
        }

        public void SendForm_Send_Immediately<T>(string _uri, Dictionary<string, string> _fieldData, Dictionary<string, List<byte[]>> _fileData, HTTPMethods _method, Action<T> _success = null, Action<string> _fail = null)
        {
            StartCoroutine(Co_SendForm(_uri, _fieldData, _fileData, _method, _success, _fail));
        }

        public void SendForm<T>(string _uri, Dictionary<string, string> _fieldData, Dictionary<string, List<byte[]>> _fileData, HTTPMethods _method, Action<T> _success = null, Action<string> _fail = null)
        {
            queue.Enqueue(Co_SendForm(_uri, _fieldData, _fileData, _method, _success, _fail));

            if (isThreadRunning is false)
            {
                StartCoroutine(ThreadQueue());
            }
        }

        private IEnumerator ThreadQueue()
        {
            isThreadRunning = true;

            while (queue.Count > 0)
            {
                yield return StartCoroutine(queue.Dequeue());
            }

            isThreadRunning = false;
        }

        private IEnumerator Co_Send<T>(string _uri, string _data, HTTPMethods _method, Action<T> _success = null, Action<string> _fail = null, bool _isIncludeToken = false, bool _isRetry = false, bool _isAuth = false)
        {

            string targetServeruri = _isAuth ? mainAuthUrl : mainUrl;

            Uri uri = new Uri($"{targetServeruri}/{_uri}");
            HTTPRequest req = new HTTPRequest(uri, _method);

            req.AddHeader("Content-Type", "application/json");

            Debug.Log($"Send :: {uri}");

            if (_isIncludeToken is true)
            {
                req.AddHeader("Authorization", $"Bearer {DC.Token}");
            }

            if (string.IsNullOrEmpty(_data) is false)
            {
                req.RawData = Encoding.UTF8.GetBytes(_data);
                Debug.Log($"Data :: {_data}");
            }

            yield return req.Send();

            ResponseResult(req, _success, _fail, _isRetry, () => StartCoroutine(Co_Send(_uri, _data, _method, _success, _fail)));

            req.Dispose();
            yield break;
        }

        private IEnumerator Co_SendForm<T>(string _uri, Dictionary<string, string> _fieldData, Dictionary<string, List<byte[]>> _fileData, HTTPMethods _method, Action<T> _success = null, Action<string> _fail = null)
        {
            Uri uri = new Uri($"{mainUrl}/{_uri}");
            HTTPRequest req = new HTTPRequest(uri, _method);
            req.FormUsage = BestHTTP.Forms.HTTPFormUsage.Multipart;
            req.AddHeader("Authorization", $"Bearer {DC.Token}");

            if (_fieldData != null)
            {
                foreach (var v in _fieldData)
                {
                    req.AddField(v.Key, v.Value);
                }
            }

            if (_fileData != null)
            {
                foreach (var v in _fileData)
                {
                    for (int i = 0; i < v.Value.Count; i++)
                    {
                        req.AddBinaryData(v.Key, v.Value[i]);
                    }
                }
            }

            Debug.Log($"Send :: {uri}");
            Debug.Log($"Data :: {JsonConvert.SerializeObject(_fieldData)}");
            Debug.Log($"Media Data :: {JsonConvert.SerializeObject(_fileData)}");

            yield return req.Send();

            ResponseResult(req, _success, _fail);

            req.Dispose();
        }

        private void ResponseResult<T>(HTTPRequest _req, Action<T> _success = null, Action<string> _fail = null, bool _isRetry = false, Action _retry = null)
        {
            switch (_req.State)
            {
                case HTTPRequestStates.Finished:
                    if (_req.Response.IsSuccess)
                    {
                        Debug.Log($"Receive :: {_req.Response.DataAsText}");
                        T resultData = JsonConvert.DeserializeObject<T>(_req.Response.DataAsText);
                        _success?.Invoke(resultData);
                    }
                    else
                    {
                        Debug.LogError(string.Format("{3} :: Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                        _req.Response.StatusCode, _req.Response.Message, _req.Response.DataAsText, _req.Uri));
                        if (_req.Response.DataAsText.Contains("NOT_FOUND"))    // SNS Login : Token NOT_FOUND
                        {
                            _fail?.Invoke(_req.Response.DataAsText);
                        }
                        else
                        {
                            _fail?.Invoke(_req.Response.DataAsText);
                        }
                    }
                    break;

                case HTTPRequestStates.Aborted:
                case HTTPRequestStates.ConnectionTimedOut:
                case HTTPRequestStates.TimedOut:
                case HTTPRequestStates.Error:
                    if (_fail is null)
                    {
                        Debug.LogError($"{_req.Uri} || {_req.State} || Request Finished with Error! " + (_req.Exception != null ? (_req.Exception.Message + "\n" + _req.Exception.StackTrace) : "No Exception"));
                    }
                    else
                    {
                        if (_isRetry is true)
                        {
                            Debug.LogError($"{_req.Uri} is faile... retry..");
                            _retry?.Invoke();
                            //StartCoroutine(Co_Send(_uri, _data, _method, _success, _fail));
                        }
                        else
                        {
                            UIManager.s.Dim = true;
                            UIManager.s.OpenOneButton("", Const.NETWORK_ERROR, () =>
                            {
                                UIManager.s.Dim = false;
                            }, Const.OK);

                            if (_req.Exception is null)
                            {
                                Debug.LogError($"{_req.State} || Unknown Error..");
                            }
                            else
                            {
                                _fail.Invoke(_req.Exception.Message);
                            }
                        }
                    }
                    break;

                default:
                    Debug.LogError($"{_req.State} || Request Finished with Error! " + (_req.Exception != null ? (_req.Exception.Message + "\n" + _req.Exception.StackTrace) : "No Exception"));
                    break;
            }
        }
        #endregion

        #region Login.
        public void Login(string _id, string _password, string _clientId, Action<Protocol.Response.Login> _success, Action<string> _error)
        {
            var uri = "*****";
            var data = new Protocol.Request.Login(_id, RSAModule.RSAEncrypt(_password), _clientId);
            var jsonData = JsonConvert.SerializeObject(data);
            Send<Protocol.Response.Login>(uri, jsonData, HTTPMethods.Post, (result) =>
            {
                DC.GetLoginInfo(result);
                _success(result);
            }, (error) =>
            {
                _error(error);
            },
            _isIncludeToken: false,
            _isRetry: false,
            _isAuth: true);
        }

        public void AutoLogin(Protocol.Request.AutoLogin _data, Action<Protocol.Response.Login> _success, Action<string> _error)
        {
            var uri = string.Empty;

            if (Define.platform == Define.Platform.UnityEditor)
            {
                uri = "*****";
            }
            else
            {
                uri = "*****";
            }

            var data = JsonConvert.SerializeObject(_data);
            Send<Protocol.Response.Login>(uri, data, HTTPMethods.Post, (result) =>
            {
                DC.GetLoginInfo(result);
                _success(result);
            }, (error) =>
            {
                _error(error);
            },
            _isIncludeToken: false,
            _isAuth: true);
        }

        public void GuestLogin(Protocol.Request.GuestLogin _data, Action<Protocol.Response.Login> _success, Action<string> _error)
        {
            var uri = "*****";
            var data = JsonConvert.SerializeObject(_data);
            Send<Protocol.Response.Login>(uri, data, HTTPMethods.Post, (result) =>
            {
                DC.GetLoginInfo(result);
                _success(result);
            }, (error) =>
            {
                _error(error);
            },
            _isIncludeToken: false,
            _isAuth: true);
        }

        public void SocialLogin(Protocol.Request.SocialLogin _data, Action<Protocol.Response.Login> _success, Action<string> _error)
        {
            var uri = "*****";
            var data = JsonConvert.SerializeObject(_data);
            Send<Protocol.Response.Login>(uri, data, HTTPMethods.Post, (result) =>
            {
                DC.GetLoginInfo(result);
                _success(result);
            }, (error) =>
            {
                _error(error);
            },
            _isIncludeToken: false,
            _isAuth: true);
        }

        public void Logout(Action<Protocol.Response.MeemzDefault> _success, Action<string> _error)
        {
            var uri = "*****";
            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                { "refreshToken", DC.data_login.data.refreshToken }
            };

            var data = JsonConvert.SerializeObject(dic);
            Send<Protocol.Response.MeemzDefault>(uri, data, HTTPMethods.Post, (result) =>
            {
                _success(result);
            }, (error) =>
            {
                _error(error);
            },
            _isIncludeToken: true,
            _isAuth: true);
        }

        public void Withdrawal(string _reason, string _reasonDetail, Action<Protocol.Response.MeemzDefault> _success, Action<string> _error)
        {
            var uri = "*****";

            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                { "reason", _reason },
                { "reasonDetail", _reasonDetail }
            };

            var data = JsonConvert.SerializeObject(dic);

            Send<Protocol.Response.MeemzDefault>(uri, data, HTTPMethods.Delete, (result) =>
            {
                _success(result);
            }, (error) =>
            {
                _error(error);
            },
            _isIncludeToken: true,
            _isAuth: true);
        }

        #endregion

        #region Get Feed List.
        public void GetFeedList(long _last_id, Action<Protocol.Response.FeedList> _success, Action<string> _error, int _pageSize = 10)
        {
            var uri = "*****";

            if (_last_id != 0)
            {
                uri = "*****";
            }

            Send<Protocol.Response.FeedList>(uri, null, HTTPMethods.Get, (result) =>
            {
                DC.GetFeedList(result);
                _success(result);

            }, (error) =>
            {
                _error(error);
            },
            _isIncludeToken: true,
            _isRetry: false);
        }

        public void GetMyFeedList(long _last_id, Action<Protocol.Response.FeedList> _success, Action<string> _error, int _pageSize = 18)
        {
            var uri = "*****";

            if (_last_id != 0)
            {
                uri = "*****";
            }

            Send<Protocol.Response.FeedList>(uri, null, HTTPMethods.Get, (result) =>
            {
                DC.GetMyFeedLists(result);
                _success(result);
            }, (error) =>
            {
                _error(error);
            }, _isIncludeToken: true);
        }
        #endregion

        #region Feed Upload.

        public void UploadFeed(Protocol.Request.FeedUpload _data, Action<Feed> _success, Action<string> _error)
        {
            var uri = "*****";

            Dictionary<string, string> field = new Dictionary<string, string>
            {
                {"content", _data.content }
            };

            List<byte[]> meidaData = new List<byte[]>();

            for (int i = 0; i < _data.medias.Length; i++)
            {
                if (string.IsNullOrEmpty(_data.medias[i]) is true)
                    continue;

                meidaData.Add(File.ReadAllBytes(_data.medias[i]));
            }

            Dictionary<string, List<byte[]>> file = new Dictionary<string, List<byte[]>>
            {
                {"medias", meidaData }
            };

            SendForm<Feed>(uri, field, file, HTTPMethods.Post, (result) =>
             {
                 _success(result);
             }, (error) =>
             {
                 _error(error);
             });
        }


        #endregion

        #region Feed Edit/Delete
        public void EditFeed(long _feed_id, string _contents, Action<Feed> _success, Action<string> _error = null)
        {
            var uri = "*****";

            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                { "content", _contents }
            };

            var data = JsonConvert.SerializeObject(dic);

            Send<Feed>(uri, data, HTTPMethods.Patch, (result) =>
            {
                _success(result);
            }, (error) =>
            {
                _error?.Invoke(error);
            },
            _isIncludeToken: true);
        }

        public void DeleteFeed(long _feed_id, Action _success, Action<string> _error = null)
        {
            var uri = "*****";
            Send(uri, HTTPMethods.Delete, (result) =>
            {
                _success();
            }, (error) =>
            {
                _error?.Invoke(error);
            },
            _isIncludeToken: true);
        }
        #endregion

        #region like.
        public void Like(bool _like, long _id, Action<Protocol.Response.LikeCount> _success = null, Action _fail = null)
        {
            var uri = "*****";

            Send<Protocol.Response.LikeCount>(uri, null, _like ? HTTPMethods.Post : HTTPMethods.Delete, (result) =>
            {
                _success?.Invoke(result);
            },
            _isIncludeToken: true);
        }
        public void GetLikeUserList(long _id, Action<Protocol.Response.LikeUserList> _success = null)
        {
            var uri = "*****";

            Send<Protocol.Response.LikeUserList>(uri, null, HTTPMethods.Get, (result) =>
            {
                DC.GetUserList(result.likes);
                _success?.Invoke(result);
            },
            _isIncludeToken: true);
        }
        #endregion

        #region Commnet.
        public void GetCommentUserList(long _feed_id, Action<Protocol.Response.CommentUserList> _success)
        {
            var uri = "*****";

            Send<Protocol.Response.CommentUserList>(uri, null, HTTPMethods.Get, (result) =>
            {
                DC.GetUserList(result.users);
                _success?.Invoke(result);
            },
            _isIncludeToken: true);
        }

        public void GetCommentList(long _feed_id, long _last_id, Action<Protocol.Response.CommentList> _success, Action<string> _error, int _pageSize = 20)
        {
            var uri = "*****";

            if (_last_id != 0)
            {
                uri = "*****";
            }

            Send<Protocol.Response.CommentList>(uri, null, HTTPMethods.Get, (result) =>
            {
                DC.GetCommentList(result);
                _success(result);
            }, (error) =>
            {
                _error(error);
            }, _isIncludeToken: true);
        }

        public void WriteComment(long _feed_id, string _content, Action<Comment> _success, Action<string> _error)
        {
            var uri = "*****";

            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                { "content", _content }
            };

            var data = JsonConvert.SerializeObject(dic);
            Send<Comment>(uri, data, HTTPMethods.Post, (result) =>
            {
                _success(result);
            }, (error) =>
            {
                _error(error);
            }, _isIncludeToken: true);
        }

        public void EditComment(long _feed_id, long _comment_id, string _content, Action<Comment> _success, Action<string> _error = null)
        {
            var uri = "*****";

            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                { "content", _content }
            };

            var data = JsonConvert.SerializeObject(dic);

            Send<Comment>(uri, data, HTTPMethods.Patch, (result) =>
            {
                _success(result);
            }, (error) =>
            {
                _error(error);
            }, _isIncludeToken: true);
        }

        public void DeleteComment(long _feed_id, long _comment_id, Action<string> _success, Action<string> _error = null)
        {
            var uri = "*****";

            Send(uri, HTTPMethods.Delete, (result) =>
            {
                _success(result);
            }, (error) =>
            {
                _error?.Invoke(error);
            }, _isIncludeToken: true);
        }
        #endregion

        #region Report.
        public void ReportFeed(long _feed_id, Protocol.Request.Report _data, Action<Protocol.Response.ReportResult> _success = null, Action<string> _error = null)
        {
            var uri = "*****";
            var data = JsonConvert.SerializeObject(_data);

            Send<Protocol.Response.ReportResult>(uri, data, HTTPMethods.Post, (result) =>
            {
                _success?.Invoke(result);
            }, (error) =>
            {
                _error?.Invoke(error);
            }, _isIncludeToken: true);
        }

        public void ReportComment(long _feed_id, long _comment_id, Protocol.Request.Report _data, Action<Protocol.Response.ReportResult> _success = null, Action<string> _error = null)
        {
            var uri = "*****";
            var data = JsonConvert.SerializeObject(_data);

            Send<Protocol.Response.ReportResult>(uri, data, HTTPMethods.Post, (result) =>
            {
                _success?.Invoke(result);
            }, (error) =>
            {
                _error?.Invoke(error);
            }, _isIncludeToken: true);
        }
        #endregion

        #region Profile.
        public void CreateMyProfile(Action<UserProfile> _success = null, Action<string> _error = null)
        {
            var uri = "*****";

            //Dictionary<string, string> field = new Dictionary<string, string>
            //{
            //    {"username", DC.data_login.id },
            //    //{"signup_site", _sigup_site},
            //};

            SendForm<UserProfile>(uri, null, null, HTTPMethods.Post, (result) =>
            {
                DC.GetMyProfile(result);
                _success(result);
            }, (error) =>
            {
                _error?.Invoke(error);
            });
        }

        public void GetMyProfile(Action<UserProfile> _success = null, Action<string> _error = null)
        {
            var uri = "*****";

            Send<UserProfile>(uri, null, HTTPMethods.Get, (result) =>
            {
                DC.GetMyProfile(result);
                _success?.Invoke(result);
            }, (error) =>
            {
                if (error.Contains("UNAUTHORIZED"))
                {
                    CreateMyProfile((create_result) =>
                    {
                        _success?.Invoke(create_result);
                    });
                }
                else
                {
                    _error?.Invoke(error);
                }
            }, _isIncludeToken: true);
        }

        public void GetProfile(long _id, Action<UserProfile> _success = null, Action<string> _error = null)
        {
            var uri = "*****";

            Send<UserProfile>(uri, null, HTTPMethods.Get, (result) =>
            {
                _success?.Invoke(result);
            }, (error) =>
            {
                _error?.Invoke(error);
            }, _isIncludeToken: true);
        }

        public void EditProfileCheck(string _username, Action<Protocol.Response.DefautlStatus> _success = null, Action<string> _error = null)
        {
            var uri = "*****";

            Send<Protocol.Response.DefautlStatus>(uri, null, HTTPMethods.Get, (result) =>
            {
                _success?.Invoke(result);
            }, (error) =>
            {
                _error?.Invoke(error);
            });
        }

        public void EditProfile(string _username, string _picture, Action<UserProfile> _success, Action<string> _error)
        {
            var uri = "*****";

            Dictionary<string, string> field = new Dictionary<string, string>
            {
                {"username", _username }
            };

            List<byte[]> meidaData = new List<byte[]>();

            if (string.IsNullOrEmpty(_picture) is false)
            {
                meidaData.Add(File.ReadAllBytes(_picture));
            }

            Dictionary<string, List<byte[]>> file = new Dictionary<string, List<byte[]>>
            {
                {"picture", meidaData }
            };

            SendForm<UserProfile>(uri, field, file, HTTPMethods.Patch, (result) =>
            {
                DC.GetMyProfile(result);
                _success(result);
            }, (error) =>
            {
                _error(error);
            });

        }
        #endregion

        #region Authorization.

        public void CheckPassword(string _password, Action _success, Action<string> _error = null)
        {
            var uri = "*****";

            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                { "password", RSAModule.RSAEncrypt(_password)}
            };

            var data = JsonConvert.SerializeObject(dic);

            Send<Protocol.Response.MeemzDefault>(uri, data, HTTPMethods.Patch, (result) =>
            {
                _success();
            }, (error) =>
            {
                _error?.Invoke(error);
            },
            _isIncludeToken: true,
            _isAuth: true);
        }

        public void CheckOverlapEmail(string _email, Action<bool> _success, Action<string> _error)
        {
            var uri = "*****";

            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                { "email", _email }
            };

            var data = JsonConvert.SerializeObject(dic);

            Send<Protocol.Response.MeemzDefault>(uri, data, HTTPMethods.Post, (result) =>
            {
                string data = result.data;
                _success(bool.Parse(data));
            }, (error) =>
            {
                _error?.Invoke(error);
            }, _isAuth: true);
        }

        public void CheckOverlapID(string _id, Action<bool> _success, Action<string> _error)
        {
            var uri = "*****";

            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                { "id", _id }
            };

            var data = JsonConvert.SerializeObject(dic);

            Send<Protocol.Response.MeemzDefault>(uri, data, HTTPMethods.Post, (result) =>
            {
                string data = result.data;
                _success(bool.Parse(data));
            }, (error) =>
            {
                _error?.Invoke(error);
            }, _isAuth: true);
        }

        public void CheckOverlapNickname(string _nickname, Action<bool> _success, Action<string> _error)
        {
            var uri = "*****";

            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                { "nickname", _nickname }
            };

            var data = JsonConvert.SerializeObject(dic);

            Send<Protocol.Response.MeemzDefault>(uri, data, HTTPMethods.Post, (result) =>
            {
                string data = result.data;
                _success(bool.Parse(data));
            }, (error) =>
            {
                _error?.Invoke(error);
            },
            _isIncludeToken: true,
            _isAuth: true);
        }

        public void EditNickName(string _userId, string _nickname, Action _success, Action<string> _error)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                { "nickname", _nickname }
            };

            var data = JsonConvert.SerializeObject(dic);

            EditUserInfo(_userId, data, _success, _error);
        }

        public void EditUserInfo(string _userId, string _data, Action _success, Action<string> _error)
        {
            var uri = "*****";

            Send<Protocol.Response.MeemzDefault>(uri, _data, HTTPMethods.Patch, (result) =>
            {
                _success();
            }, (error) =>
            {
                _error(error);
            },
            _isIncludeToken: true,
            _isAuth: true);
        }
        #endregion

        #region SignUp.
        public void SignUp(bool _isOver, Protocol.Request.SignUp _data, Action<Protocol.Response.Login> _success, Action<string> _error)
        {
            string uri = null;
            if (_isOver)
                uri = "****";
            else
                uri = "*****";

            var data = JsonConvert.SerializeObject(_data);
            Send<Protocol.Response.Login>(uri, data, HTTPMethods.Post, (result) =>
            {
                _success(result);
            }, (error) =>
            {
                _error(error);
            }, _isAuth: true);
        }

        public void SocialSignUp(Protocol.Request.SocialSignUp _data, Action<Protocol.Response.Login> _success, Action<string> _error)
        {
            var uri = "*****";

            var data = JsonConvert.SerializeObject(_data);
            Send<Protocol.Response.Login>(uri, data, HTTPMethods.Post, (result) =>
            {
                Debug.Log($"SocialSignUp result ID : {result.data.id}");
                DC.GetLoginInfo(result);
                _success(result);
            }, (error) =>
            {
                _error(error);
            }, _isAuth: true);
        }
        #endregion

        #region Coin.
        public void GetCoin(long _feedId, Action<Protocol.Response.CoinAcquired> _success, Action<string> _error)
        {
            var uri = "*****";

            Send<Protocol.Response.CoinAcquired>(uri, null, HTTPMethods.Get, (result) =>
            {
                _success(result);
            }, (error) =>
            {
                _error(error);
            },
            _isIncludeToken: true);
        }

        public void GetCoinStatus(Action<Protocol.Response.CoinStatus> _success, Action<string> _error)
        {
            var uri = "*****";

            Send<Protocol.Response.CoinStatus>(uri, null, HTTPMethods.Get, (result) =>
            {
                _success(result);
            }, (error) =>
            {
                _error(error);
            },
            _isIncludeToken: true);
        }

        public void GetCoinHistory(int _page, Action<Protocol.Response.CoinHistory> _success, Action<string> _error, int _pageSize = 10)
        {
            var uri = "*****";

            Send<Protocol.Response.DataDefault<Protocol.Response.CoinHistory>>(uri, null, HTTPMethods.Get, (result) =>
            {
                DC.GetCoinHistory(result.data);
                _success(result.data);
            }, (error) =>
            {
                _error(error);
            },
            _isIncludeToken: true);

        }
        #endregion

        #region Notification
        public void GetNotificationCount(Action<Protocol.Response.NotificationCount> _success, Action<string> _error)
        {
            var uri = "*****";

            Send<Protocol.Response.NotificationCount>(uri, null, HTTPMethods.Get, (result) =>
            {
                DC.GetNotificationCount(result);
                _success(result);
            }, (error) =>
            {
                _error(error);
            },
            _isIncludeToken: true);
        }

        public void GetNotificationList(long _id, Action<Protocol.Response.NotificationList> _success, Action<string> _error, int _pageSize = 20)
        {
            string uri = string.Empty;

            if (_id == 0)
                uri = "*****";
            else
                uri = "*****";

            Send<Protocol.Response.NotificationList>(uri, null, HTTPMethods.Get, (result) =>
            {
                DC.GetNotificationList(result);
                _success(result);
            }, (error) =>
            {
                _error(error);
            },
            _isIncludeToken: true);
        }

        public void ConfirmNotification(bool delete, long[] _ids, Action<Protocol.Response.NotificationCount> _success, Action<string> _error)
        {
            var uri = "*****";
            string data = null;

            // _ids 가 null 일 경우, 전체를 대상으로 함.
            if (_ids != null)
            {
                Dictionary<string, long[]> dic = new Dictionary<string, long[]>()
                {
                    { "ids", _ids }
                };

                data = JsonConvert.SerializeObject(dic);
            }

            var methodType = delete ? HTTPMethods.Delete : HTTPMethods.Patch;

            Send<Protocol.Response.NotificationCount>(uri, data, methodType, (result) =>
            {
                _success(result);
            }, (error) =>
            {
                _error(error);
            },
            _isIncludeToken: true);
        }
        #endregion

                #region CheckServerStatus.
                //public void CheckServerStatus(string _version, Action<Protocol<CheckServerStatus>> _success = null)
                //{
                //    StartCoroutine(Co_CheckServerStatus(_version, _success));
                //}

                //private IEnumerator Co_CheckServerStatus(string _version, Action<Protocol<CheckServerStatus>> _success = null)
                //{
                //    string uri = "******";

                //    UnityWebRequest req = new UnityWebRequest(uri, "GET");
                //    req.SetRequestHeader("Content-Type", "application/json");
                //    req.downloadHandler = new DownloadHandlerBuffer();

                //    yield return req.SendWebRequest();

                //    if (req.error is null)
                //    {
                //        Protocol<CheckServerStatus> response = JsonConvert.DeserializeObject<Protocol<CheckServerStatus>>(req.downloadHandler.text);
                //        mainuri = response.data.serverProfileInfo.meemzApiServeruri;
                //        _success?.Invoke(response);
                //    }
                //    else
                //    {
                //        Debug.LogError($"{uri} is faile... retry..");
                //        StartCoroutine(Co_CheckServerStatus(_version, _success));
                //    }
                //}
                #endregion
        }
}
