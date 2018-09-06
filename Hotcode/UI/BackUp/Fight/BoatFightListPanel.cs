using System.Collections.Generic;
using UnityEngine;

namespace geniusbaby.ui
{
    public class BoatFightListPanel : ListContainer<BoatFightItemPanel, BoatFightItemPanel, int>
    {
        public void LayoutUpdate(float delta)
        {
            var p = Mathf.RoundToInt(delta * 200);
            for (int index = 0; index < m_listItems.Count; ++index)
            {
                var it = m_listItems[index];
                var ap = it.cachedRc.anchoredPosition;
                int x = Mathf.RoundToInt(ap.x);

                var prevX = 0;
                var prevIndex = index - 1;
                if (prevIndex >= 0)
                {
                    var prevIt = m_listItems[prevIndex];
                    var prevW = Mathf.RoundToInt(prevIt.cachedRc.rect.width);
                    prevX = Mathf.RoundToInt(prevIt.cachedRc.anchoredPosition.x) - prevW;
                }

                if (x < prevX)
                {
                    x += p;
                    if (x > p)
                    {
                        x = p;
                    }
                    ap.x = x;
                    it.cachedRc.anchoredPosition = ap;
                }
            }
        }
        public void LayoutLast()
        {
            var index = values.Count - 1;
            var it = GetItem(index);

            var ap = it.cachedRc.anchoredPosition;
            ap.x = -5 * Mathf.RoundToInt(it.cachedRc.rect.width);
            it.cachedRc.anchoredPosition = ap;
        }
    }
}