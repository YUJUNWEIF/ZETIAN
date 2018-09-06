using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;

public class ZipFileWriter
{
    void ZipGen(ZipOutputStream zipOutput, string entryName, byte[] bytes)
    {
        Crc32 crc = new Crc32();
        crc.Reset();
        crc.Update(bytes);

        ZipEntry entry = new ZipEntry(entryName);
        entry.DateTime = DateTime.Now;
        entry.Size = bytes.Length;
        //entry.Crc = crc.Value;
        zipOutput.PutNextEntry(entry);
        zipOutput.Write(bytes, 0, (int)bytes.Length);
        zipOutput.CloseEntry();
        zipOutput.Flush();
    }
    void OldSave(string zipFileName, ZipOutputStream zipOutput, Predicate<string> pred)
    {
        if (!File.Exists(zipFileName)) { return; }

        byte[] bytes = File.ReadAllBytes(zipFileName);
        var ms = new MemoryStream(bytes);
        using (var zip = new ZipInputStream(ms))
        {
            ZipEntry theEntry = null;
            byte[] buffer = new byte[2048];
            while ((theEntry = zip.GetNextEntry()) != null)
            {
                if (pred(theEntry.Name))
                {
                    var fdata = new byte[theEntry.Size];
                    Util.DependencyUtil.ZipEntryUncompress(zip, theEntry, fdata);
                    ZipGen(zipOutput, theEntry.Name, fdata);
                }
            }
        }
    }
    public void Save(string zipFileName, List<KeyValuePair<string, byte[]>> datas, Predicate<string> pred)
    {
        var output = new MemoryStream();
        using (var zipOutput = new ZipOutputStream(output))
        {
            OldSave(zipFileName, zipOutput, pred);
            for (int index = 0; index < datas.Count; ++index)
            {
                ZipGen(zipOutput, datas[index].Key, datas[index].Value);
            }
        }
        output.Flush();
        if (File.Exists(zipFileName))
        {
            File.Delete(zipFileName);
        }
        File.WriteAllBytes(zipFileName, output.ToArray());
        output.Dispose();
    }
}