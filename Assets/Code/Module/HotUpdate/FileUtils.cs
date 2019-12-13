using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using Framework.HotUpdate;


namespace Framework
{
    /**
		Unity常用目录对应的Android && iOS平台地址
		IOS:
		Application.dataPath :            Application/xxxxx/xxx.app/Data
		Application.streamingAssetsPath : Application/xxxxx/xxx.app/Data/Raw
		Application.persistentDataPath :  Application/xxxxx/Documents
		Application.temporaryCachePath :  Application/xxxxx/Library/Caches
		UserDefaults:                     Application/Library/Preferences


		Android:
		Application.dataPath :            /data/app/xxx.xxx.xxx.apk
		Application.streamingAssetsPath : jar:file:///data/app/xxx.xxx.xxx.apk/!/assets
		Application.persistentDataPath :  /data/data/xxx.xxx.xxx/files  or /storage/emulated/0/Android/data/xxx.xxx.xxx  or /mnt/sdcard/Android/data/xxx.xxx.xxx/files
		Application.temporaryCachePath :  /data/data/xxx.xxx.xxx/cache
		SharedPreferences:                /data/data/<package name>/shared_prefs
     * */

    /// <summary>
    /// 资源存储方式
    /// </summary>
    public enum StorageType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown,
        /// <summary>
        /// 本地资源"Resources/GameAssets/"
        /// </summary>
        Direct,
        /// <summary>
        /// 外部打包资源"Application.dataPath/StreamingAssets/"
        /// </summary>
        Package,
        /// <summary>
        /// 外部缓存打包资源"Application.persistentDataPath/StreamingAssets/"
        /// </summary>
        Local,
        /// <summary>
        /// 远程服务器资源"http://"
        /// </summary>
        Remote
    }

    /// <summary>
    /// 文件工具
    /// </summary>
    public class FileUtils:Singleton<FileUtils>
    {
        public static string StreamingAssetsPath;
        public static string FILE_SYMBOL;
        public static string OUTER_FILE_SYMBOL;

        public static string PackageResRootPath;
        public static string LocalResRootPath;
        public static string LocalTempResRootPath;
        public static string LocalTempResRootPath2;//游戏内下载使用
		public static string RemoteResRootPath;

        public static readonly string DirectRootPath = "Assets/GameAssets/";
        public static readonly string AssetRootInBundle = DirectRootPath.ToLower();
        public static readonly string LuaBundleRootDir = "luascripts";
        public static readonly string LuaBytesRootDir = "luabytes";
        public static readonly string LuaBytesExt = ".bytes";
        public static readonly string ASSET_BUNDLE_EXTENSION = ".data";

        public static readonly string[] Extension_Texture = { ".jpg", ".png", "tga", ".exr" };
        public static readonly string[] Extension_Material = { ".mat" };
        public static readonly string[] Extension_GameObject = { ".fbx", ".prefab" };
		public static readonly string[] Extension_Text = {".txt",".xml"};
		public static readonly string   Extension_Animator = ".controller";
		public static readonly string   Extension_SpineSkeletonData = "_skeletondata.asset";
//		public static readonly string   Extension_SpineAtlasData = "_Atlas.asset";

        public void Init()
        {
#if UNITY_EDITOR
            StreamingAssetsPath = PathManager.RES_LOCAL_ROOT_PATH + "/";
            FILE_SYMBOL = @"file://";
            OUTER_FILE_SYMBOL =  @"file:///";
#elif (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)
            //StreamingAssetsPath = Application.streamingAssetsPath+"/";
            StreamingAssetsPath = PathManager.RES_LOCAL_ROOT_PATH + "/";
            FILE_SYMBOL = @"file://";
            OUTER_FILE_SYMBOL =  @"file:///";
#elif UNITY_IOS
            StreamingAssetsPath = Application.dataPath+"/Raw/";
            FILE_SYMBOL = "file://";
            OUTER_FILE_SYMBOL = "file://";
#elif UNITY_ANDROID
            StreamingAssetsPath = Application.dataPath+"!/assets/";
            FILE_SYMBOL = "jar:file://";
            OUTER_FILE_SYMBOL = "file://";
#endif

            PackageResRootPath = StreamingAssetsPath + PathManager.GetRuntimePlatform() + "/";
            LocalResRootPath = Application.persistentDataPath + "/ClientRes/" + PathManager.GetRuntimePlatform() + "/";
            LocalTempResRootPath = LocalResRootPath + "Temp/";
            LocalTempResRootPath2 = LocalResRootPath + "Temp2/";

            CLogger.Log("######PackageResRootPath=" + PackageResRootPath);
            CLogger.Log("######LocalResRootPath=" + LocalResRootPath);
            CLogger.Log("######LocalTempResRootPath=" + LocalTempResRootPath);
            CLogger.Log("######LocalTempResRootPath2=" + LocalTempResRootPath2);
        }

        private HashSet<string> _localNewFlags = new HashSet<string>();

		public void SetRemoteResRootPath(string remoteResRoot)
		{
			if(!remoteResRoot.EndsWith("/"))
			{
				remoteResRoot += "/";
			}
            RemoteResRootPath = remoteResRoot + PathManager.GetRuntimePlatform() + "/";
        }

        public void SetLocalNew(string resPath)
        {
            _localNewFlags.Add(resPath);
        }

        public bool IsLocalNew(string resPath)
        {
            return _localNewFlags.Contains(resPath);
        }

        public string GetAssetNameInBundle(string resPath)
        {
            return AssetRootInBundle + resPath;
        }

        /// <summary>
        /// 统一路径格式 unifiedPath = unifiedPath.Replace ('\\', '/');
        /// </summary>
        public string UnifyPath(string unifiedPath)
        {
            unifiedPath = unifiedPath.Replace('\\', '/');
            return unifiedPath;
        }

        /// <summary>
        /// 根据文件后缀名获得文件类型
        /// </summary>
        public Type GetAssetTypeByExtension(string file)
        {
            Type type = null;
            string extenstion = string.Empty;
            int index = file.LastIndexOf(".");
            if (index > -1)
            {
                extenstion = file.Substring(index);
                extenstion = extenstion.ToLower();

                foreach (var ext in Extension_GameObject)
                {
                    if (ext == extenstion)
                    {
                        type = typeof(GameObject);
                        break;
                    }
                }
                if (type == null)
                {
                    foreach (var ext in Extension_Material)
                    {
                        if (ext == extenstion)
                        {
                            type = typeof(Material);
                            break;
                        }
                    }
                }
                if (type == null)
                {
                    foreach (var ext in Extension_Texture)
                    {
                        if (ext == extenstion)
                        {
                            type = typeof(Texture2D);
                            break;
                        }
                    }
                }

				if(type == null)
				{
					foreach(var ext in Extension_Text)
					{
						if(ext == extenstion)
						{
							type = typeof(TextAsset);
							break;
						}
					}
				}

				if(type == null)
				{
					if(extenstion == Extension_Animator)
					{
						type = typeof(RuntimeAnimatorController);
					}
				}

				if(type == null)
				{
					if(file.EndsWith(Extension_SpineSkeletonData))
					{
						//type = typeof(Spine.Unity.SkeletonDataAsset);
					}
				}

//				if(type == null)
//				{
//					if(file.EndsWith(Extension_SpineAtlasData))
//					{
//						type = typeof(Spine.Unity.AtlasAsset);
//					}
//				}
            }
            return type;
        }

        /// <summary>
        /// 获取文件后缀名
        /// </summary>
        public string GetFileExtention(string file)
        {
            string extenstion = string.Empty;
            int index = file.LastIndexOf(".");
            if (index > -1)
            {
                extenstion = file.Substring(index);
            }
            return extenstion;
        }

        /// <summary>
        /// 获取文件的完整路径
        /// </summary>
        //public string FullPathForFile(string resPath, int resType = ResourceType.ASSET_BUNDLE)
        //{
        //    string fullPath;

        //    if (ResourceCache.Instance.resLoadMode == ResourceLoadMode.Direct)
        //    {
        //        fullPath = DirectRootPath + resPath;
        //    }
        //    else
        //    {
        //        if (resType == ResourceType.ASSET_BUNDLE)
        //        {
        //            //file += ASSET_BUNDLE_EXTENSION;
        //            resPath = AssetBundleDependency.Instance.GetAssetBundleNameByAssetPath(resPath);
        //        }

        //        if (ResourceCache.Instance.resLoadMode == ResourceLoadMode.AssetBundle)
        //        {
        //            bool isLocalNew = VersionMgr.Instance.IsLocalCacheFile(resPath);
        //            if (isLocalNew)
        //            {
        //                fullPath = OUTER_FILE_SYMBOL + LocalResRootPath + resPath;
        //            }
        //            else
        //            {
        //                fullPath = FILE_SYMBOL + PackageResRootPath + resPath;
        //            }
        //        }
        //        else
        //        {
        //            if (RemoteResRootPath.StartsWith("http"))
        //            {
        //                fullPath = RemoteResRootPath + resPath;
        //            }
        //            else
        //            {
        //                fullPath = OUTER_FILE_SYMBOL + RemoteResRootPath + resPath;
        //            }
        //        }
        //    }
        //    return fullPath;
        //}

        /// <summary>
        /// 检查路径是否存在，不存在则创建它
        /// </summary>
        public void CheckDirExists(string dir)
        {
            DirectoryInfo info = new DirectoryInfo(dir);
            if (!info.Exists)
            {
                info.Create();
            }
        }

        /// <summary>
        /// 检查文件路径是否存在，不存在则创建它
        /// </summary>
        public void CheckDirExistsForFile(string file)
        {
            int index = file.LastIndexOf("/");
            if (index == -1)
                return;
            string dir = file.Substring(0, index);
            DirectoryInfo info = new DirectoryInfo(dir);
            if (!info.Exists)
            {
                info.Create();
            }
        }

		public void DeleteDir(string dir)
		{
			if (Directory.Exists(dir))
			{
				Directory.Delete(dir,true);
			}
		}
		
		public void DeleteFile(string file)
		{
			if (File.Exists (file)) {
				try {
					#if !(UNITY_ANDROID ||  UNITY_IOS)
					File.SetAttributes(file, FileAttributes.Normal);
					#endif
					File.Delete (file);
				} catch (Exception ex) {
					CLogger.LogError (ex.Message);
				}
			}
		}

        /// <summary>
        /// 获取文件长度
        /// </summary>
        public long GetFileLength(string fullPath)
        {
            long len = 0;
            if (File.Exists(fullPath))
            {
                using (FileStream fileStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    len = fileStream.Length;
                }
            }
            return len;
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        public void WriteFile(string str, string fullPath, bool isAppend = false)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            WriteFile(bytes, fullPath, isAppend);
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        public void WriteFile(byte[] bytes, string fullPath, bool isAppend = false)
        {
            string dir = fullPath.Substring(0, fullPath.LastIndexOf('/'));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            FileMode mode = isAppend ? FileMode.Append : FileMode.OpenOrCreate;
            using (FileStream stream = new FileStream(fullPath, mode, FileAccess.Write, FileShare.None))
			{
                try
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                finally
                {
                    stream.Flush();
                    stream.Close();
                }
			}
        }

        /// <summary>
        /// 复制整个目录，包含子目录和文件
        /// </summary>
        public void CopyDirectory(string srcDir, string destDir)
        {
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            DirectoryInfo srcDirInfo = new DirectoryInfo(srcDir);
            FileInfo[] files = srcDirInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                file.CopyTo(Path.Combine(destDir, file.Name));
            }
            DirectoryInfo[] directories = srcDirInfo.GetDirectories();
            foreach (DirectoryInfo dir in directories)
            {
                CopyDirectory(Path.Combine(srcDir, dir.Name), Path.Combine(destDir, dir.Name));
            }
        }

		//可以控制忽略目录中的某些目录不删除，某些文件不删除
		public void DeleteDirectory(string dir, bool deleteDirectory = true, List<string> ignoreDirs = null, List<string> ignoreFiles = null)
		{
			//			if (Directory.Exists (dir))
			//			{
			//				string[] files = Directory.GetFiles (dir);
			//				string[] dirs = Directory.GetDirectories (dir);
			//
			//				foreach (string file in files)
			//				{
			//					File.SetAttributes (file, FileAttributes.Normal);
			//					File.Delete (file);
			//				}
			//
			//				foreach (string subDir in dirs)
			//				{
			//					DeleteDirectory (subDir);
			//				}
			//
			//				Directory.Delete (dir, false);
			//			}

			if (Directory.Exists (dir)) {
				string[] files = Directory.GetFiles (dir, "*.*", SearchOption.AllDirectories);
				foreach (var file in files) {
					try {
						#if !(UNITY_ANDROID ||  UNITY_IOS)
                        File.SetAttributes(file, FileAttributes.Normal);
						#endif
						bool isIgnore = false;
						if (ignoreDirs != null) {
							foreach (var dirs in ignoreDirs) {
								if (file.Contains (dirs)) {
									CLogger.LogWarn ("file is in [ignoreDir], keeps=====" + file);
									isIgnore = true;
									break;
								}
							}
							if (isIgnore)
								continue;
						}
						if (ignoreFiles != null) {
								foreach (var ifile in ignoreFiles) {
									if (ifile == file) {
									CLogger.LogWarn ("file is in [ignoreFiles], keeps=====" + file);
									isIgnore = true;
									break;
								}
							}
							if (isIgnore)
								continue;
						}
						File.Delete (file);
					} catch (Exception ex) {
						CLogger.LogError (ex.Message);
					}
				}
				try {
					if (deleteDirectory) { 
						//删除目录以前会报错，现在一直没有出现，这里暂时加一个try  catch,如果删不掉，就不删了
						Directory.Delete (dir, true);
					}
				} catch (Exception ex) {
					CLogger.LogError (ex.Message);
				}
			}
		}
    }
}