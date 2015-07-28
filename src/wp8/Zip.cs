using System;
using System.IO;
using System.IO.IsolatedStorage;
using SharpGIS;
using WPCordovaClassLib.Cordova.JSON;

namespace WPCordovaClassLib.Cordova.Commands
{
    public class Zip : BaseCommand
    {
        public void unzip(string jsonArgs)
        {
            Console.WriteLine("unzip");
            var options = JsonHelper.Deserialize<string[]>(jsonArgs);
            string zipPath = options[0];
            string extractPath = options[1];

            // open isolated storage
            using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {             
                // open filestream for the zip-file
                using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(zipPath, FileMode.Open, FileAccess.ReadWrite, isoStorage))
                {
                    // let's creat unzipper
                    using (UnZipper zipStream = new UnZipper(fileStream))
                    {
                        foreach (string dir in zipStream.DirectoriesInZip)
                        {
                            isoStorage.CreateDirectory(dir);
                        }

                        // let's run through all files within the zip-file
                        foreach (string file in zipStream.FileNamesInZip) {                             
                            using (var streamWriter = new BinaryWriter(new IsolatedStorageFileStream(file, FileMode.Create, FileAccess.Write, FileShare.Write, isoStorage)))
                            {
                                Stream zipFileStream = zipStream.GetFileStream(file);

                                var buffer = new byte[2048];
                                int size;
                                while ((size = zipFileStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    streamWriter.Write(buffer, 0, size);
                                }
                            }
                        }

                        DispatchCommandResult(new PluginResult(PluginResult.Status.OK, true));
                    }
                }
            }
        }
    }
}