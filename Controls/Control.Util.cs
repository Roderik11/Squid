using System;
using System.Collections.Generic;

namespace Squid
{
    public partial class Control
    {
        /// <summary>
        /// Makes the control the focused control
        /// </summary>
        public void Focus()
        {
            if (!AllowFocus) return;
            if (Desktop == null) return;

            Desktop.FocusedControl = this;
        }

        /// <summary>
        /// Returns the first control matching the given name.
        /// This method searches recursively.
        /// </summary>
        public Control GetControl(string name)
        {
            if (Name == name) return this;

            Control result = null;

            if (IsContainer)
            {
                foreach (Control child in LocalContainer.Controls)
                {
                    result = child.GetControl(name);
                    if (result != null)
                        return result;
                }
            }

            foreach (Control child in Elements)
            {
                result = child.GetControl(name);
                if (result != null)
                    return result;
            }

            return result;
        }

        /// <summary>
        /// Returns direct children of the given type
        /// </summary>
        public List<T> GetControls<T>() where T : Control
        {
            List<T> result = new List<T>();

            if (IsContainer)
            {
                foreach (Control child in LocalContainer.Controls)
                {
                    if (child is T)
                        result.Add(child as T);
                }
            }

            foreach (Control child in Elements)
            {
                if (child is T)
                    result.Add(child as T);
            }

            return result;
        }

        /// <summary>
        /// Returns true if the given control is a child of the control
        /// </summary>
        public bool IsChildOf(Control control)
        {
            if (control.Elements.Contains(this)) return true;

            foreach (Control child in control.Elements)
            {
                if (IsChildOf(child))
                    return true;
            }

            if (!control.IsContainer) return false;
            if (control.LocalContainer.Controls.Contains(this)) return true;

            foreach (Control child in control.LocalContainer.Controls)
            {
                if (IsChildOf(child))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Sends the control to the front of its container
        /// </summary>
        public void BringToFront()
        {
            if (_parent == null) return;
            if(_isElement)
            {
                var tempparent = _parent;
                tempparent.Elements.Remove(this);
                tempparent.Elements.Add(this);
            }
            else
            {
                if (Container == null) return;
                int index = Container.Controls.IndexOf(this);
                if (index == Container.Controls.Count - 1) return;

                Control parent = _parent;
                Parent = null;
                Parent = parent;
            }
        }

        /// <summary>
        /// Sends the control to the back of its container
        /// </summary>
        public void BringToBack()
        {
            if (_parent == null) return;

            if (_isElement)
            {
                var tempparent = _parent;
                tempparent.Elements.Remove(this);
                tempparent.Elements.Insert(0, this);
            }
            else
            {
                if (Container == null) return;
                int index = Container.Controls.IndexOf(this);
                if (index == 0) return;

                IControlContainer container = Container;
                Parent = null;
                container.Controls.Insert(0, this);
            }
        }

        /// <summary>
        /// Called every frame.
        /// Override this to do per-frame operations.
        /// </summary>
        protected virtual void OnUpdate()
        {
            Update?.Invoke(this);
        }

        /// <summary>
        /// Called every frame during layout.
        /// </summary>
        protected virtual void OnLayout()
        {
            Layout?.Invoke(this);
        }

        /// <summary>
        /// Called late every frame.
        /// </summary>
        protected virtual void OnLateUpdate()
        {
            LateUpdate?.Invoke(this);
        }

        /// <summary>
        /// Override this method to handle any keyevents passed to the control
        /// </summary>
        protected virtual void OnKeyDown(KeyEventArgs args) { }

        /// <summary>
        /// Override this method to handle any keyevents passed to the control
        /// </summary>
        protected virtual void OnKeyUp(KeyEventArgs args) { }

        /// <summary>
        /// Override this to implement your own AutoSize behavior
        /// </summary>
        protected virtual void OnAutoSize() { }

        /// <summary>
        /// Override this to handle state changes.
        /// </summary>
        protected virtual void OnStateChanged() { }

        public event Action StateChanged;

        public float UIScale = 1;
             
        public float GetScale()
        {
            if (_parent == null)
                return UIScale;

            return _parent.UIScale * _parent.GetScale();
        }

        private void SetEnabled(bool value)
        {
            if (_parentEnabled == value)
                return;

            _parentEnabled = value;

            if (IsContainer)
            {
                foreach (Control control in LocalContainer.Controls)
                    control.SetEnabled(value);
            }

            foreach (Control control in Elements)
                control.SetEnabled(value);
        }

        internal void PerformLateUpdate()
        {
            OnLateUpdate();

            for (int i = 0; i < Elements.Count; i++)
                Elements[i].PerformLateUpdate();

            if (IsContainer)
            {
                for (int i = 0; i < LocalContainer.Controls.Count; i++)
                    LocalContainer.Controls[i].PerformLateUpdate();
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is Control control)) return false;
            return control.AutoId == AutoId;
        }
    }
}
