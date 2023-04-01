using System;
using System.Collections.Generic;

namespace ZundaTeller.Presentation
{
    public interface INavigationControl
    {
        BaseController Current { get; }
        IReadOnlyList<BaseController> List { get; }
        void Restart();
        void Push(BaseController screen);
        void Switch(BaseController screen);
        void Pop();
    }
}