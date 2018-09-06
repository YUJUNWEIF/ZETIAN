using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace geniusbaby
{
    public enum ChatType
    {
        None = 0,
        Text = 1,
        Audio = 2,
        Multi = 3,
    }
    public interface IGMapChatData
    {
        ChatType type { get; }
        int talkerSId { get; }
        void Play();
        void Clear();
    }
    public class ChatData : IGMapChatData, IAMRVisit
    {
        const int SEGMENTLEN = 1024;
        public int talkerSId { get; private set; }
        public bool isNew;
        List<byte[]> fragments = new List<byte[]>();
        int length;
        public ChatData(int talkerSId)
        {
            this.talkerSId = talkerSId;
            this.isNew = true;
            this.fragments = new List<byte[]>();
            this.length = 0;
        }
        public void Fill(byte[] data)
        {
            Fill(data, 0, data.Length);
        }
        public void Fill(byte[] data, int offset, int count)
        {
            if (count <= 0) { return; }

            var free = (fragments.Count * SEGMENTLEN) - length;
            if (free >= count)
            {
                var buffer = fragments[fragments.Count - 1];
                Array.Copy(data, offset, buffer, buffer.Length - free, count);
                length += count;
            }
            else
            {
                Fill(data, offset, free);
                fragments.Add(Util.PoolByteArrayAlloc.alloc.Alloc(SEGMENTLEN));
                Fill(data, offset + free, count - free);
            }
        }

        public ChatType type { get { return ChatType.Audio; } }
        public void Play()
        {
            //ChatManager.Inst().soundPlayer.Play(this);
        }
        public void Clear()
        {
            for (int index = 0; index < fragments.Count; ++index)
            {
                var buffer = fragments[index];
                Util.PoolByteArrayAlloc.alloc.Free(ref buffer);
            }
            fragments.Clear();
            length = 0;
        }
        public int GetData(IntPtr dst, int srcIndex, int maxLength)
        {
            int free = length - srcIndex;
            if (free <= maxLength) { maxLength = free; }

            int start = srcIndex / SEGMENTLEN;
            int end = (srcIndex + maxLength + SEGMENTLEN - 1) / SEGMENTLEN;

            if (start + 1 == end)
            {
                Marshal.Copy(fragments[start], srcIndex % SEGMENTLEN, dst, maxLength);
            }
            else
            {
                for (int index = start; index < end; ++index)
                {
                    if (index == start)
                    {
                        int offset = srcIndex % SEGMENTLEN;
                        Marshal.Copy(fragments[index], offset, dst, SEGMENTLEN - offset);
                        dst = AMRCodec.MemoryOffset(dst, SEGMENTLEN - offset);
                    }
                    else if (index == fragments.Count - 1)
                    {
                        int actualLength = (srcIndex + maxLength) % SEGMENTLEN;
                        Marshal.Copy(fragments[index], 0, dst, actualLength);
                        dst = AMRCodec.MemoryOffset(dst, actualLength);
                        break;
                    }
                    else
                    {
                        int offset = 0;
                        Marshal.Copy(fragments[index], offset, dst, SEGMENTLEN - offset);
                        dst = AMRCodec.MemoryOffset(dst, SEGMENTLEN - offset);
                    }
                }
            }
            return maxLength;
        }        
    }

    public class TextChatData : IGMapChatData
    {
        public int talkerSId { get; private set; }
        public string msg { get; private set; }
        public TextChatData(int talkerSId, string msg)
        {
            this.talkerSId = this.talkerSId;
            this.msg = msg;
        }
        public ChatType type { get { return ChatType.Text; } }
        public void Play()
        {

        }
        public void Clear()
        {
        }
    }
}
