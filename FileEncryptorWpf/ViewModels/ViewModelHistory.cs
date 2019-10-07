using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptorWpf.ViewModels
{
    public abstract class ViewModelHistory
    {
        protected readonly Stack<object> viewModelHistory = new Stack<object>();

        protected abstract void SetCurrentControl(object obj);

        public void GoToPreviousViewModel()
        {
            if (viewModelHistory.Count > 1)
                viewModelHistory.Pop();
            SetCurrentControl(viewModelHistory.Peek());
        }

        public void GoToViewModel(object x)
        {
            viewModelHistory.Push(x);
            SetCurrentControl(x);
        }
    }
}
