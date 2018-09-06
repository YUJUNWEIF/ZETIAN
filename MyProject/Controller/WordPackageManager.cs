//using System;
//using System.Collections.Generic;
//using System.IO;

//namespace geniusbaby
//{
//    public class _WordPackageManager : Singleton<_WordPackageManager>
//    {
//        IFileReader m_reader;
//        Dictionary<int, List<cfg.wordClass>> m_packages = new Dictionary<int, List<cfg.wordClass>>();
//        public _WordPackageManager(IFileReader reader)
//        {
//            m_reader = reader;
//        }
//        public void ParseAll()
//        {
//            FindZip(GlobalDefine.wordPackage, (file, bytes) =>
//            {
//                var index = file.IndexOf('.');
//                if (index < 0) { return; }

//                var subfix = file.Substring(0, index);
//                var packageSubCfg = tab.wordPackageSub.Inst().Find(it => it.file == subfix);
//                if (packageSubCfg != null)
//                {
//                    var encodeMs = new MemoryStream(bytes);
//                    var os = new BinaryReader(encodeMs);
//                    var array = DeBinary.Deserializer.Deserialize<List<cfg.wordClass>>(os);
//                    m_packages.Add(packageSubCfg.packageId, array);
//                }
//            });
//        }
//        public void Parse(string file)
//        {

//            //wordpackCfg.file
//        }
//        public void __Pa()
//        {
//            //var ms = new MemoryStream(m_reader.Load(GlobalDefine.wordPackage));
//            //using (var zip = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(ms))
//            //{
//            //    ICSharpCode.SharpZipLib.Zip.ZipEntry theEntry = null;
//            //    byte[] buffer = new byte[2048 * 4];
//            //    while ((theEntry = zip.GetNextEntry()) != null)
//            //    {
//            //        var index = theEntry.Name.IndexOf('.');
//            //        if (index < 0) { return; }

//            //        var subfix = theEntry.Name.Substring(0, index);
//            //        var packageSubCfg = tab.wordPackageSub.Inst().Find(it => it.file == subfix);
//            //        if (packageSubCfg != null)
//            //        {
//            //            var encodeMs = new MemoryStream(bytes);
//            //            var os = new BinaryReader(encodeMs);
//            //            var array = DeBinary.Deserializer.Deserialize<List<cfg.wordClass>>(os);
//            //            m_packages.Add(packageSubCfg.packageId, array);
//            //        }

//            //        var fdata = new byte[theEntry.Size];
//            //        Util.DependencyUtil.ZipEntryUncompress(zip, theEntry, fdata);
//            //        parser(theEntry.Name, fdata);
//            //    }
//            //}
//        }
//        public void Parse(int packageId)
//        {
//            //m_packages.Clear();
//            if (m_packages.ContainsKey(packageId)) { return; }
            
//            var packageSubCfg = tab.wordPackageSub.Inst().Find(packageId);
//            FindZip(GlobalDefine.wordPackage, packageSubCfg.file + ".xls", bytes =>
//            {
//                var encodeMs = new MemoryStream(bytes);
//                var os = new BinaryReader(encodeMs);
//                var array = DeBinary.Deserializer.Deserialize<List<cfg.wordClass>>(os);
//                m_packages.Add(packageSubCfg.packageId, array);
//            });
//        }
//        public List<cfg.wordClass> FindClasses(int packageId)
//        {
//            List<cfg.wordClass> pack;
//            m_packages.TryGetValue(packageId, out pack);
//            return pack;
//        }
//        public List<cfg.word> MakeKnowledge(int packageId, Util.FastRandom rand, int count)
//        {
//            List<cfg.wordClass> classCfgs;
//            m_packages.TryGetValue(packageId, out classCfgs);
//            int tryCount = 0;
//            var wordCfgs = new List<cfg.word>();
//            while (wordCfgs.Count < count)
//            {
//                int classIndex = rand.Next(classCfgs.Count);
//                var classCfg = classCfgs[classIndex];
//                var wordIndex = rand.Next(classCfg.words.Count);
//                var wordCfg = classCfg.words[wordIndex];
//                if (cfg.word.Valid(wordCfg.english) && !wordCfgs.Contains(wordCfg))
//                {
//                    wordCfgs.Add(wordCfg);
//                }
//                ++tryCount;
//                if (tryCount >= count * 3) { break; }
//            }
//            return wordCfgs;
//        }
//        public cfg.wordClass FindClass(int packageId, int classId)
//        {
//            var package = FindClasses(packageId);
//            return package.Find(it => it.id == classId);
//        }
//        public cfg.wordClass FindClassesIfMissing(int packageId, int classId)
//        {
//            var package = FindClasses(packageId);
//            if (package == null)
//            {
//                Parse(packageId);
//                package = FindClasses(packageId);
//            }
//            return package.Find(it => it.id == classId);
//        }
//        void FindZip(string zipFile, string file, Action<byte[]> parser)
//        {
//            var ms = new MemoryStream(m_reader.Load(zipFile));
//            using (var zip = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(ms))
//            {
//                ICSharpCode.SharpZipLib.Zip.ZipEntry theEntry = null;
//                while ((theEntry = zip.GetNextEntry()) != null)
//                {
//                    if (theEntry.Name != file) { continue; }
//                    var fdata = new byte[theEntry.Size];
//                    Util.DependencyUtil.ZipEntryUncompress(zip, theEntry, fdata);
//                    parser(fdata);
//                    break;
//                }
//            }
//        }
//        void FindZip(string zipFile, Action<string, byte[]> parser)
//        {
//            var ms = new MemoryStream(m_reader.Load(zipFile));
//            using (var zip = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(ms))
//            {
//                ICSharpCode.SharpZipLib.Zip.ZipEntry theEntry = null;
//                byte[] buffer = new byte[2048 * 4];
//                while ((theEntry = zip.GetNextEntry()) != null)
//                {
//                    var fdata = new byte[theEntry.Size];
//                    Util.DependencyUtil.ZipEntryUncompress(zip, theEntry, fdata);
//                    parser(theEntry.Name, fdata);
//                }
//            }
//        }
//    }
//}
