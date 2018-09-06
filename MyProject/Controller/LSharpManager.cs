using System;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;

namespace geniusbaby
{
    public class LSharpManager : Singleton<LSharpManager>, IGameEvent
    {
        List<Code> codes = new List<Code>();
        public void Regist(byte[] bytes)
        {
            var ms = new MemoryStream(bytes);
            using (var zip = new ZipInputStream(ms))
            {
                ZipEntry theEntry = null;
                while ((theEntry = zip.GetNextEntry()) != null)
                {
                    try
                    {
                        var fdata = new byte[theEntry.Size];
                        Util.DependencyUtil.ZipEntryUncompress(zip, theEntry, fdata);
                        codes.Add(new Code() { dll = fdata, pdb = null });
                    }
                    catch (Exception e)
                    {
                        Util.Logger.Instance.Error(theEntry.Name, e);
                    }
                }
            }
        }
        public void OnStartGame()
        {
            LSharpInterface.StartUp(codes);
            codes.Clear();
        }
        public void OnStopGame() { }
    }
}
