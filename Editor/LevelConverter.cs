using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;

namespace geniusbaby
{
    public static class LevelConverter
    {
        static string configPath = Application.dataPath + "/../Excel/binMap.zip";        
        static S2D SVPConvert(GameObject go)
        {
            S2D s2d = new S2D();
            //var gm = GameObject.Find("GameManager");
            //var cam3d = gm.transform.Find("camera3d").GetComponent<Camera>();
            //var cam2d = gm.transform.Find("camera2d").GetComponent<Camera>();
            //var desktop = gm.transform.Find("desktop").GetComponent<RectTransform>();
            //var vp = cam3d.projectionMatrix * cam3d.worldToCameraMatrix;
            //var mat2d = Matrix4x4.identity;
            //mat2d.m00 = 0.5f * Screen.width;
            //mat2d.m11 = 0.5f * Screen.height;
            //s2d.eye = cam3d.transform.position.V3();
            //s2d.svp = (mat2d * vp).MF();

            //var single = desktop.GetComponentInChildren<ui.FightSingleFrame>(true);
            //var multi = desktop.GetComponentInChildren<ui.FightMultiFrame>(true);
            //var singleGun = single.playerPanel.gunRoot;
            //var multiGun1 = multi.aPlayerPanel.gunRoot;
            //var multiGun2 = multi.bPlayerPanel.gunRoot;
            //Vector2 pos;
            //C2DWorldPosRectangleLocal(cam2d, singleGun.transform.position, out pos, desktop);
            //s2d.singleGunPos = pos.V2();
            //C2DWorldPosRectangleLocal(cam2d, multiGun1.transform.position, out pos, desktop);
            //s2d.multiGunPos1 = pos.V2();
            //C2DWorldPosRectangleLocal(cam2d, multiGun2.transform.position, out pos, desktop);
            //s2d.multiGunPos2 = pos.V2();

            var a = go.transform.Find("dynamic/a");
            var b = go.transform.Find("dynamic/b");
            s2d.multiGunPos1 = new FEVector3D(Mathf.RoundToInt(a.position.x), 0f, Mathf.RoundToInt(a.position.z));
            s2d.multiGunPos2 = new FEVector3D(Mathf.RoundToInt(b.position.x), 0f, Mathf.RoundToInt(b.position.z));            
            return s2d;
        }
        static FELevelSpline LevelSplineConvert(GameObject go)
        {
            var lspline = new FELevelSpline();
            var bs = go.GetComponentsInChildren<Spline>(false);
            lspline.name = go.name;
            lspline.splines = new List<ISpline>();

            TypeSplinesConvert(go.transform, lspline.splines);

            TypeCamConvert(1, go.transform.Find(@"Dynamic/Defender/cam"), lspline);
            TypeCamConvert(2, go.transform.Find(@"Dynamic/Attacker/cam"), lspline);

            TypeSlotConvert(1, go.transform.Find(@"Dynamic/Defender/slot/pet"), lspline);
            TypeSlotConvert(2, go.transform.Find(@"Dynamic/Attacker/slot/pet"), lspline);
            TypeSlotConvert(3, go.transform.Find(@"Dynamic/Left/slot/pet"), lspline);
            TypeSlotConvert(4, go.transform.Find(@"Dynamic/Right/slot/pet"), lspline);

            return lspline;
        }
        static void TypeSplinesConvert(Transform trans, List<ISpline> paths)
        {
            var bs = trans.GetComponentsInChildren<Spline>(false);
            for (int splineIndex = 0; splineIndex < bs.Length; ++splineIndex)
            {
                Spline spline = bs[splineIndex];
                spline.cacheImpl.Id = int.Parse(spline.name);
                if (!paths.Exists(it => it.Id == spline.cacheImpl.Id))
                {
                    paths.Add(spline.cacheImpl);                    
                }
                else
                {
                    Debug.LogError("duplicate name");
                }
            }
        }
        static void TypeCamConvert(int type, Transform trans, FELevelSpline lspline)
        {
            //if (trans != null)
            //{
            //    var cam = trans.GetComponent<Camera>();
            //    lspline.cams.Add(new FELevelSpline.Cam()
            //    {
            //        index = type,
            //        location = trans.position.V3(),
            //        rotation = trans.rotation.QF(),
            //        svp = (cam.projectionMatrix * cam.worldToCameraMatrix).MF()
            //    });
            //}
        }
        static void TypeSlotConvert(int type, Transform trans, FELevelSpline lspline)
        {
            //if (trans != null)
            //{
            //    lspline.slots.Add(new FELevelSpline.Slot()
            //    {
            //        index = type,
            //        location = trans.position.V3(),
            //        rotation = trans.rotation.QF(),
            //    });
            //}
        }
        static bool C2DWorldPosRectangleLocal(Camera cam2d, Vector3 worldPoint, out Vector2 localPos, RectTransform rect)
        {
            var screenPos = RectTransformUtility.WorldToScreenPoint(cam2d, worldPoint);
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, cam2d, out localPos);
        }

        [MenuItem("Custom/Level/SVPConverter")]
        static void ProcessSVP()
        {
            GameObject go = Selection.activeObject as GameObject;
            if (go == null) { return; }
            var svp = SVPConvert(go);
            SaveXml(go.name + ".map", S2D.SaveToJson(svp));
        }
        [MenuItem("Custom/Level/LevelSplineConverter")]
        static void ProcessSplines()
        {
            GameObject go = Selection.activeObject as GameObject;
            if (go == null) { return; }
            var spline = LevelSplineConvert(go);
            SaveXml(spline.name + @".spline", FELevelSpline.SaveToZipBinData(spline));
        }
        static void SaveXml(string xml, byte[] buffer)
        {
            var zipWriter = new ZipFileWriter();
            var datas = new List<KeyValuePair<string, byte[]>>();
            datas.Add(new KeyValuePair<string, byte[]>(xml, buffer));
            zipWriter.Save(configPath, datas, name => name != xml);
        }
    }
}