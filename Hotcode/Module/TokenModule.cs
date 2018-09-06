using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public class TokenModule : Singleton<TokenModule>, IModule
    {
        public struct Token
        {
            public int id;
            public int value;
            public Token(int id, int value) { this.id = id; this.value = value; }
        }
        List<Token> m_tokens = new List<Token>();
        public Util.ParamActions onSync = new Util.ParamActions();
        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { m_tokens.Clear(); }
        public void Sync(List<Token> tokens)
        {
            m_tokens = tokens;
            onSync.Fire();
        }
        public void Update(List<Token> ups)
        {
            for (int index = 0; index < ups.Count; ++index)
            {
                int exist = m_tokens.FindIndex(it => it.id == ups[index].id);
                if (exist >= 0)
                {
                    m_tokens[exist] = ups[index];
                }
                else
                {
                    m_tokens.Add(ups[index]);
                }
            }
            onSync.Fire();
        }
        public int Get(int resId)
        {
            var token = m_tokens.Find(it => it.id == resId);
            return token.value;
        }
    }
}
