using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Marvrus.Util
{

    public class FileCache : Singleton<FileCache>
    {
        private string localPath;
        private bool isThreadRunning = false;
        private Queue<IEnumerator> queue = new();

        protected override void Awake()
        {
            base.Awake();
            localPath = $"{Application.persistentDataPath}/imageCache";
            Debug.Log($"{localPath}");
        }

        public void GetImage(string _url, Vector2 _size, Action<Texture2D> _success, Action<string> _error = null)
        {
            if (string.IsNullOrEmpty(_url))
            {
                Debug.LogError($"FileCache.GetImage url is null");
                return;
            }

            string[] splitUrl = _url.Split("/");
            string fileName = splitUrl[splitUrl.Length - 1];
            string fullPath = $"{localPath}/{fileName}";

            if (File.Exists(fullPath))
            {
                LoadImage(fullPath, _size, (texture) =>
                {
                    _success(texture);
                }, _error);
            }
            else
            {
                DownloadImage(_url, fileName, (path) =>
                {
                    LoadImage(fullPath, _size, (texture) =>
                    {
                        _success(texture);
                    }, _error);
                }, (error) =>
                {
                    if (_error is null)
                        Debug.LogError($"FileCache.GetImage Error :: {error}");
                    else
                        _error(error);
                });
            }
        }

        //public Texture2D LoadImage(string _path, int _width, int _height)
        //{
        //    byte[] bytes = File.ReadAllBytes(_path);
        //    Texture2D texture = new Texture2D(_width, _height);
        //    texture.LoadImage(bytes);
        //    return texture;
        //}

        public void LoadImage(string _path, Vector2 _size, Action<Texture2D> _success, Action<string> _error = null)
        {
            _ = LoadImageAync(_path, _size, (texture) =>
            {
                _success(texture);
            }, (error) =>
            {
                _error?.Invoke(error);
            });
        }

        private async Task LoadImageAync(string _path, Vector2 _size, Action<Texture2D> _callback, Action<string> _error)
        {
            try
            {
                byte[] bytes = await File.ReadAllBytesAsync(_path);

                Texture2D texture = new Texture2D((int)_size.x, (int)_size.y);
                texture.LoadImage(bytes);

                _callback(texture);
            }
            catch (Exception e)
            {
                if (_error is null)
                {
                    Debug.LogError($"LoadIamge Error !! {_path} :: {e}");
                }
                else
                {
                    _error(e.ToString());
                }
            }
        }

        public void DownloadImage(string _url, string _fileName, Action<string> _success, Action<string> _error)
        {
            queue.Enqueue(Co_DownloadImage(_url, _fileName, _success, _error));

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

        private IEnumerator Co_DownloadImage(string _url, string _fileName, Action<string> _success, Action<string> _error)
        {
            UnityWebRequest req = UnityWebRequest.Get(_url);
            Debug.Log($"download image :: {_url}");
            yield return req.SendWebRequest();

            if (req.result is UnityWebRequest.Result.Success)
            {
                if (Directory.Exists(localPath) is false)
                    Directory.CreateDirectory(localPath);

                string path = $"{localPath}/{_fileName}";

                _ = WriteImageAyncAsync(path, req.downloadHandler.data, () =>
                {
                    _success?.Invoke(path);
                });
                //File.WriteAllBytes(path, req.downloadHandler.data);
            }
            else
            {
                _error?.Invoke(req.error);
            }

            req.Dispose();
        }

        //private void WriteImage(string _path, byte[] _data)
        //{
        //    File.WriteAllBytes(_path, _data);
        //}

        public void WriteImage(string _path, byte[] _data, Action<string> _callback)
        {
            _ = WriteImageAyncAsync(_path, _data, () =>
            {
                _callback(_path);
            });
        }

        private async Task WriteImageAyncAsync(string _path, byte[] _data, Action _callback)
        {
            await File.WriteAllBytesAsync(_path, _data);
            _callback();
        }


        public string GetImagePath(string _url)
        {
            string[] splitUrl = _url.Split("/");
            string fileName = splitUrl[splitUrl.Length - 1];
            return $"{localPath}/{fileName}";
        }
    }
}