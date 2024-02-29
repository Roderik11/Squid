using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Squid
{
    public class VirtualList : Frame
    {
        public ScrollBar Scrollbar { get; private set; }
        public Frame ClipFrame { get; private set; }
        public Frame ScrollContent { get; private set; }
        public Frame Content { get; private set; }

        public int ItemHeight = 28;
        public int ItemSpacing = 0;

        public Action<Control, int> BindItem;

        public Func<int, Control> CreateItem;

        private Stack<Control> itemStack = new Stack<Control>();
        
        private IList dataSource;
        private Control topSpace;
        private Control bottomSpace;

        public IList DataSource
        {
            get { return dataSource; }
            set
            {
                foreach(var child in Content.Controls)
                    itemStack.Push(child);

                Content.Controls.Clear();
                dataSource = value;
            }
        }

        public VirtualList()
        {
            NoEvents = false;
            Size = new Point(100, 100);

            Scrollbar = new ScrollBar();
            Scrollbar.Dock = DockStyle.Right;
            Scrollbar.Size = new Point(25, 25);
            Scrollbar.Orientation = Orientation.Vertical;
            Scrollbar.Size = new Point(12, 16);
            Scrollbar.ButtonDown.Visible = false;
            Scrollbar.ButtonUp.Visible = false;
            Scrollbar.Slider.Button.Margin = new Margin(2, 4, 0, 4);
            Scrollbar.Slider.Ease = false;
            Scrollbar.Slider.MinHandleSize = 128;
            Elements.Add(Scrollbar);

            ClipFrame = new Frame();
            ClipFrame.Dock = DockStyle.Fill;
            ClipFrame.Scissor = true;
            Elements.Add(ClipFrame);

            ScrollContent = new Frame();
            ScrollContent.Dock = DockStyle.FillX;
            ScrollContent.Parent = ClipFrame;

            topSpace = new Control { Dock = DockStyle.Top, NoEvents = true, Style = "category" };
            bottomSpace = new Control { Dock = DockStyle.Bottom, NoEvents = true, Style = "category" };
            Content = new Frame { Dock = DockStyle.Fill, Style = "alterRows" };
            ScrollContent.Controls.Add(topSpace);
            ScrollContent.Controls.Add(bottomSpace);
            ScrollContent.Controls.Add(Content);

            var list = new List<int>();
            for (int i = 0; i < 1000; i++)
                list.Add(i);

            DataSource = list;
            MouseWheel += On_MouseWheel;
            ClipFrame.SizeChanged += VirtualList_SizeChanged;
        }

        private bool fullRefresh;
        private int lastTop = 0;

        private void VirtualList_SizeChanged(Control sender)
        {
            fullRefresh = true;
        }

        void On_MouseWheel(Control sender, MouseEventArgs args)
        {
            Scrollbar.Scroll(Gui.MouseScroll);
            args.Cancel = true;
        }

        public void Refresh()
        {
            fullRefresh = true;
        }

        public void ScrollTo(int index)
        {
            var height = ItemHeight + ItemSpacing;
            var vertical = index * height;
            var pos = (float)vertical / (ScrollContent.Size.y - ClipFrame.Size.y);
            pos -= (float)(height * 3) / (ScrollContent.Size.y - ClipFrame.Size.y);
            Scrollbar.SetValue(pos);
            PerformLayout();
        }

        public void UpdateVirtualList()
        {
            var itemCount = dataSource.Count;
            var containerSize = ScrollContent.Size;
            containerSize.y = ItemHeight * itemCount + ItemSpacing * itemCount - ItemSpacing;
            ScrollContent.Size = containerSize;

            bool visible = Scrollbar.ShowAlways || ScrollContent.Size.y > ClipFrame.Size.y;
            Scrollbar.Visible = visible;
            Scrollbar.Scale = visible ? Math.Min(1, (float)Size.y / ScrollContent.Size.y) : 1;
            ScrollContent.Position = visible ? new Point(0, (int)((ClipFrame.Size.y - ScrollContent.Size.y) * Scrollbar.EasedValue)) : Point.Zero;

            int itemSize = ItemHeight + ItemSpacing;
            int top = Math.Max(0, (int)Math.Floor(Math.Abs((float)ScrollContent.Position.y) / itemSize));
            int bottom = Math.Max(0, (int)Math.Floor(Math.Max(0, (float)ScrollContent.Position.y + ScrollContent.Size.y - ClipFrame.Size.y) / itemSize));

            bottomSpace.Size = new Point(bottomSpace.Size.x, bottom * itemSize);
            topSpace.Size = new Point(topSpace.Size.x, top * itemSize);

            var requiredCount = Math.Min(dataSource.Count, Math.Max(0, (int)Math.Floor(ClipFrame.Size.y / (float)itemSize)) + 2);
            int topIndex = Math.Max(0, (int)Math.Floor(Math.Abs((float)ScrollContent.Position.y) / itemSize));

            var addcount = requiredCount - Content.Controls.Count;
            for (int i = 0; i < addcount; i++)
            {
                var control = itemStack.Count > 0 ? itemStack.Pop() : CreateItem(topIndex + i);
                Content.Controls.Add(control);
                lastTop = 0;
            }

            var removecount = Content.Controls.Count - requiredCount;
            for (int i = 0; i < removecount; i++)
            {
                var control = Content.Controls.Last();
                itemStack.Push(control);
                Content.Controls.Remove(control);
                lastTop = 0;
            }

            #region exp
            //if (topIndex > lastTop)
            //{
            //    var max = Math.Min(requiredCount, topIndex - lastTop);

            //    for (int i = 0; i < max; i++)
            //    {
            //        var index = topIndex + (requiredCount - 2 - (max - 1)) + i + 1;
            //        var control = content.Controls[0];
            //        control.BringToFront();

            //        if(index >= dataSource.Count)
            //        {
            //            int test = 0;
            //            test++;
            //        }
            //        BindItem(control, index);
            //    }
            //}
            //else if (topIndex < lastTop)
            //{
            //    var max = Math.Min(requiredCount, lastTop - topIndex);

            //    for (int i = 0; i < max; i++)
            //    {
            //        var control = content.Controls[content.Controls.Count - 1];
            //        control.BringToBack();
            //        BindItem(control, topIndex + max - i - 1);
            //    }
            //}
            #endregion

            if (topIndex != lastTop || fullRefresh)
            {
                var maxCount = dataSource.Count;
            
                for (int i = 0; i < Content.Controls.Count; i++)
                {
                    if (!fullRefresh && Content.Controls[i].Rectangle.IsEmpty)
                        break;

                    var index = topIndex + i;
                    if (index < maxCount)
                        BindItem(Content.Controls[i], index);
                }

                fullRefresh = false;
            }

            lastTop = topIndex;
        }

        protected override void OnUpdate()
        {
            UpdateVirtualList();
        }
    }
}
