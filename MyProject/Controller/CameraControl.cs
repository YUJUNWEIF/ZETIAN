using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public struct TransBackUp
{
    public Transform parent;
    public Vector3 localPosition;
    public Vector3 localScale;
    public Quaternion localRotation;
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
    public TransBackUp Save(Transform target)
    {
        parent = target.parent;
        localPosition = target.localPosition;
        localScale = target.localScale;
        localRotation = target.localRotation;
        position = target.position;
        scale = target.localScale;
        rotation = target.rotation;
        return this;
    }
    public TransBackUp Restore(Transform target)
    {
        target.parent = parent;
        target.localPosition = localPosition;
        target.localScale = localScale;
        target.localRotation = localRotation;
        return this;
    }
}

public class ShakeParam
{
    public enum ShakeType
    {
        Vertical = 1,
        Horizontal = 2,
        Random = 3,
    }
    public ShakeType shakeType;
    public float delay;
    public float distance;
    public float intensity;
    public float duration;
}

namespace geniusbaby
{
    public class CameraControl : Singleton<CameraControl>
    {
        public class CamBinder
        {
            Transform cam;
            geniusbaby.IBaseObj target;
            Transform cachedTrans;
            float height;
            Vector3 relativePos;
            void OnTransform()
            {
                cam.position = new Vector3(cachedTrans.position.x, height * 0.5f, cachedTrans.position.z) + relativePos;
            }
            public void Bind(geniusbaby.IBaseObj target, float height)
            {
                this.cam = CameraControl.Inst().camera3d.transform;
                this.target = target;
                this.cachedTrans = target.transform;
                this.height = height;
                this.relativePos = cam.position - new Vector3(cachedTrans.position.x, height * 0.5f, cachedTrans.position.z);
                //target.onTransform.Add(OnTransform);
            }
            public void Unbind()
            {
                if (target)
                {
                    //target.onTransform.Rmv(OnTransform);
                }
            }
        }
        public enum DragDir
        {
            X = 1,
            Z = 2,
            XZ = X | Z,
        }
        public enum Action
        {
            Locked,
            Normal,
        }
        public Camera camera3d { get; private set; }
        float m_idealZoomAmount = 0;
        TerrainParam m_terrain;
        Util.CoroutineHelper m_coroutineHelper = new Util.CoroutineHelper();
        public Action action = Action.Locked;
        public CamBinder binder = new CamBinder();
        public void StartGame()
        {
            camera3d = Framework.Instance.camera3d;
        }
        public void StopGame() { }
        public void Attach(TerrainParam terrain)
        {
            m_terrain = terrain;
            //camera3d.fieldOfView = m_terrain.fov;
            //SetAsFirstCamera(true);
            //Reset(m_terrain.eye);
        }
        public void SetAsFirstCamera(bool first3dCam)
        {
            camera3d.clearFlags = first3dCam ? CameraClearFlags.SolidColor : CameraClearFlags.Nothing;
        }
        public void Detach() { }
        public TransBackUp SaveCamera()
        {
            return new TransBackUp()
            {
                parent = camera3d.transform.parent,
                localPosition = camera3d.transform.localPosition,
                localScale = camera3d.transform.localScale,
                localRotation = camera3d.transform.localRotation,
                position = camera3d.transform.position,
                scale = camera3d.transform.localScale,
                rotation = camera3d.transform.rotation
            };
        }
        public void RestoreCamera(TransBackUp backup)
        {
            camera3d.transform.parent = backup.parent;
            camera3d.transform.localPosition = backup.localPosition;
            camera3d.transform.localScale = backup.localScale;
            camera3d.transform.localRotation = backup.localRotation;
        }
        public void Pinch(float pinchDistance)
        {
            var distance = m_terrain.zoomSpeed * pinchDistance;
            ZoomInOut(distance);
            m_idealZoomAmount = distance;
        }
        IEnumerator ZoomImpulse()
        {
            while (true)
            {
                if (Mathf.Abs(m_idealZoomAmount) > 0.01f)
                {
                    ZoomInOut(m_idealZoomAmount);
                    m_idealZoomAmount *= 0.9f;
                    yield return null;
                }
                else
                {
                    break;
                }
            }
        }
        void ZoomInOut(float distance)
        {
            camera3d.transform.position += camera3d.transform.forward * distance;
        }
        public void Reset()
        {
            Reset(m_terrain.eye.transform);
        }
        public void Reset(Transform eye)
        {
            StopTweener();
            Util.UnityHelper.ShowCopyFrom(camera3d, eye);
        }
        public T GetImageEffect<T>() where T : MonoBehaviour
        {
            return camera3d.GetComponent<T>();
        }
        public void AddAndPlayAnimClip(AnimationClip animClip)
        {
            if (animClip)
            {
                var anim = camera3d.GetComponent<Animation>();
                if (!anim) { camera3d.gameObject.AddComponent<Animation>(); }
                animClip.legacy = true;
                anim.AddClip(animClip, animClip.name);
                anim.clip = animClip;
                anim.Play(animClip.name);
            }
        }
        public void StopAndRmvAnimClip(AnimationClip animClip)
        {
            if (animClip)
            {
                var anim = camera3d.GetComponent<Animation>();
                if (!anim)
                {
                    anim.Stop(animClip.name);
                    anim.RemoveClip(animClip);
                }
            }
        }
        public void Display(Vector3 position, Vector3 forward)
        {
            camera3d.transform.position = position;
            camera3d.transform.forward = forward;
        }
        public void RotateRoundPlayer(Vector3 center, float rotAngle)
        {
            camera3d.transform.RotateAround(center, Vector3.up, rotAngle);
        }
        public void Pitch(Vector3 center, float angle)
        {
            camera3d.transform.RotateAround(center, camera3d.transform.right, angle);
        }
        public void Billboard(Transform trans, bool reverse, bool up)
        {
            if (reverse)
            {
                trans.LookAt(trans.position * 2f - camera3d.transform.position, up ? camera3d.transform.up : Vector3.up);
            }
            else
            {
                trans.LookAt(trans.position * 2f - camera3d.transform.position, up ? camera3d.transform.up : Vector3.up);
            }
        }
        static FEVector3D ToFEVec3(Vector3 nowUp)
        {
            return new FEVector3D(nowUp.x, nowUp.y, nowUp.z);
        }
        static Vector3 ToVec3(FEVector3D nowUp)
        {
            return new Vector3((float)nowUp.x, (float)nowUp.y, (float)nowUp.z);
        }
        public void SetPosition(Vector3 pos) { camera3d.transform.position = pos; }
        public Vector3 GetPosition() { return camera3d.transform.position; }
        public Vector3 GetDir(MoveDirection unknownRepresentForward = MoveDirection.None)
        {
            switch (unknownRepresentForward)
            {
                case MoveDirection.Left: return -camera3d.transform.right;
                case MoveDirection.Right: return camera3d.transform.right;
                case MoveDirection.Up: return camera3d.transform.up;
                case MoveDirection.Down: return -camera3d.transform.up;
                default: return camera3d.transform.forward;
            }
        }
        public Ray ScreenPointToRay(Vector2 point) { return camera3d.ScreenPointToRay(point); }

        public void ResetPitch(Vector3 position)
        {
            var cam = CameraControl.Instance.camera3d;
            var shouldForward = position - cam.transform.position;
            var shouldUp = Vector3.Cross(shouldForward, cam.transform.right).normalized;
            var nowUp = cam.transform.up;
            var rot = ToFEVec3(nowUp).GetRotationTo(ToFEVec3(shouldUp), ToFEVec3(cam.transform.right));
            var forward = rot * ToFEVec3(cam.transform.forward);
            cam.transform.forward = ToVec3(forward);
        }
        public void AdjustCam(Vector3 center, float magnitude)
        {
            var cam = CameraControl.Instance.camera3d;
            cam.transform.LookAt(center);
            var distance = cam.transform.position - center;
            var direction = distance.normalized;
            RaycastHit hitInfo;
            if (Physics.Raycast(center, direction, out hitInfo, magnitude, 1 << Util.TagLayers.Terrain))
            {
                cam.transform.position = hitInfo.point;
            }
        }
        public IEnumerator GotoWithTime(Vector3 position, float time, IEnumerator after)
        {
            if (after != null) { yield return after; }
            action = Action.Locked;
            var start = camera3d.transform.position;
            var timeStart = Time.time;
            while (Time.time < timeStart + time)
            {
                var percent = (Time.time - timeStart) / time;
                camera3d.transform.position = start * (1 - percent) + position * percent;
                yield return null;
            }
            camera3d.transform.position = position;
            action = Action.Normal;
        }
        public void PlayCameraAnim(string clipName)
        {
            var component = camera3d.GetComponent<Animation>();
            if (component != null && !component.IsPlaying(clipName))
            {
                component.Play(clipName);
            }
        }
        public void StopCameraAnim(string clipName)
        {
            var component = camera3d.GetComponent<Animation>();
            if (component != null && component.IsPlaying(clipName))
            {
                component.Stop(clipName);
            }
        }
        public bool IsCameraAnimStop(string clipName)
        {
            var component = camera3d.GetComponent<Animation>();
            return component != null && component.IsPlaying(clipName);
        }
        public void PlayTweener(Transform to, float duration, AnimationCurve curve = null, bool ignoreTimeScale = true)
        {
            var go = camera3d.gameObject;
            var comp = go.GetComponent<UITweener>();
            if (comp == null) { comp = go.AddComponent<UITweener>(); }
            SaveCamera();
            Vector3 tmpPos = go.transform.position;
            Quaternion tmpRot = go.transform.rotation;
            Vector3 tmpScale = go.transform.localScale;
            comp.messageProcessed = (factor) =>
            {
                go.transform.position = tmpPos * (1f - factor) + to.position * factor;
                go.transform.localScale = tmpScale * (1f - factor) + to.localScale * factor;
                go.transform.rotation = Quaternion.Slerp(tmpRot, to.rotation, factor);
                return true;
            };
            comp.ignoreTimeScale = ignoreTimeScale;
            comp.duration = duration;
            comp.delay = 0;
            comp.replaceCurve = curve;
            if (curve != null) { comp.useCurve = true; }
            comp.Play();
        }
        public void PlayTweener(Vector3 to, float duration, AnimationCurve curve = null, bool ignoreTimeScale = true)
        {
            var go = camera3d.gameObject;
            var comp = go.GetComponent<UITweener>();
            if (comp == null) { comp = go.AddComponent<UITweener>(); }
            SaveCamera();
            Vector3 tmpPos = go.transform.position;
            comp.messageProcessed = (factor) =>
            {
                go.transform.position = tmpPos * (1f - factor) + to * factor;
                return true;
            };
            comp.ignoreTimeScale = ignoreTimeScale;
            comp.duration = duration;
            comp.delay = 0;
            comp.replaceCurve = curve;
            if (curve != null) { comp.useCurve = true; }
            comp.Play();
            return;
        }
        public void PlayTweener(TransBackUp to, float duration, AnimationCurve curve = null, bool ignoreTimeScale = true)
        {
            var go = camera3d.gameObject;
            var comp = go.GetComponent<UITweener>();
            if (comp == null) { comp = go.AddComponent<UITweener>(); }

            Vector3 tmpPos = go.transform.position;
            Quaternion tmpRot = go.transform.rotation;
            Vector3 tmpScale = go.transform.localScale;
            comp.messageProcessed = (factor) =>
            {
                go.transform.position = tmpPos * (1f - factor) + to.position * factor;
                go.transform.localScale = tmpScale * (1f - factor) + to.localScale * factor;
                go.transform.rotation = Quaternion.Slerp(tmpRot, to.rotation, factor);
                return true;
            };
            comp.ignoreTimeScale = ignoreTimeScale;
            comp.duration = duration;
            comp.delay = 0;
            comp.replaceCurve = curve;
            if (curve != null) { comp.useCurve = true; }
            comp.Play();
        }
        public void PlayShake(ShakeParam shake, System.Action whenFinish = null)
        {
            var go = camera3d.gameObject;
            var comp = go.GetComponent<UITweener>();
            if (comp == null) { comp = go.AddComponent<UITweener>(); }
            var m_ray = new Ray(go.transform.position, go.transform.forward);

            var orign = m_ray.GetPoint(shake.distance) - go.transform.position;
            comp.messageProcessed = (factor) =>
            {
                var lookAt = go.transform.position +
                orign + new Vector3(
                    (shake.shakeType & ShakeParam.ShakeType.Horizontal) == ShakeParam.ShakeType.Horizontal ? Random.Range(-shake.intensity, shake.intensity) : 0,
                    0,
                    (shake.shakeType & ShakeParam.ShakeType.Horizontal) == ShakeParam.ShakeType.Horizontal ? Random.Range(-shake.intensity, shake.intensity) : 0);
                go.transform.LookAt(lookAt);
                return true;
            };
            comp.ignoreTimeScale = false;
            comp.duration = shake.duration;
            comp.delay = shake.delay;
            comp.replaceCurve = null;
            comp.useCurve = false;
            comp.onStop.Add(() =>
            {
                go.transform.forward = m_ray.direction;
                comp.onStop.Clear();
                if (whenFinish != null) { whenFinish(); }
            });
            comp.Play();
        }
        public void StopTweener()
        {
            m_coroutineHelper.StopAll();
            var scripts = camera3d.GetComponentsInChildren<UITweener>();
            for (int index = 0; index < scripts.Length; ++index)
            {
                var script = scripts[index];
                if (script)
                {
                    script.Stop();
                    script.enabled = false;
                }
            }
        }
    }
}