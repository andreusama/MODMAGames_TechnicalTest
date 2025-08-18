using System;
using System.IO;

namespace PetoonsStudio.PSEngine.Multiplatform
{
#if UNITY_PS4
    public class WriteFilesRequest : Sony.PS4.SaveData.FileOps.FileOperationRequest
    {
        public byte[] Data = new byte[1024 * 1024 * 2];
        public string FileName;

        public override void DoFileOperations(Sony.PS4.SaveData.Mounting.MountPoint mp, Sony.PS4.SaveData.FileOps.FileOperationResponse response)
        {
            WriteFilesResponse fileResponse = response as WriteFilesResponse;

            string outpath = mp.PathName.Data + "/" + FileName;

            File.WriteAllBytes(outpath, Data);

            FileInfo info = new FileInfo(outpath);
            fileResponse.lastWriteTime = info.LastWriteTime;
            fileResponse.totalFileSizeWritten += info.Length;
        }
    }

    public class WriteFilesResponse : Sony.PS4.SaveData.FileOps.FileOperationResponse
    {
        public DateTime lastWriteTime;
        public long totalFileSizeWritten;
    }

    public class ReadFilesRequest : Sony.PS4.SaveData.FileOps.FileOperationRequest
    {
        public string FileName;

        public override void DoFileOperations(Sony.PS4.SaveData.Mounting.MountPoint mp, Sony.PS4.SaveData.FileOps.FileOperationResponse response)
        {
            ReadFilesResponse fileResponse = response as ReadFilesResponse;

            string outpath = mp.PathName.Data + "/" + FileName;

            FileInfo info = new FileInfo(outpath);

            fileResponse.Data = File.ReadAllBytes(outpath);
        }
    }

    public class ReadFilesResponse : Sony.PS4.SaveData.FileOps.FileOperationResponse
    {
        public byte[] Data;
    }
#endif
}

