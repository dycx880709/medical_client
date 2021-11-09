using Ms.Controls;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace MM.Medical.Client.Controls
{
    public class MenuModule : UserControl
    {
        static MenuModule()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuModule), new FrameworkPropertyMetadata(typeof(MenuModule)));
        }

        public override void OnApplyTemplate()
        {
            PART_PAGER = GetTemplateChild("PART_PAGER") as Pager;
            PART_PAGER.PageChanged += PageChanged;
            base.OnApplyTemplate();
        }

        #region 右侧按钮

        public UIElement HeaderPanel
        {
            get { return (UIElement)GetValue(HeaderPanelProperty); }
            set { SetValue(HeaderPanelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderPanelProperty =
            DependencyProperty.Register("HeaderPanel", typeof(UIElement), typeof(MenuModule), new PropertyMetadata(null));

        #endregion

        #region 标题

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MenuModule), new PropertyMetadata(""));



        #endregion

        #region 分页

        Pager PART_PAGER;

        public event EventHandler<PageChangedEventArgs> NotifyPageChanged;

        public Visibility PagerVisibility
        {
            get { return (Visibility)GetValue(PagerVisibilityProperty); }
            set { SetValue(PagerVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PagerVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PagerVisibilityProperty =
            DependencyProperty.Register("PagerVisibility", typeof(Visibility), typeof(MenuModule), new PropertyMetadata(Visibility.Visible));

        public int CurrentPage
        {
            get
            {
                return PART_PAGER.PageIndex;
            }
            set
            {
                PART_PAGER.PageIndex = value;
            }
        }

        public void SetTotalCount(int count)
        {
            PART_PAGER.TotalCount = count;
        }

        public void SetSelectedCount(int count)
        {
            PART_PAGER.SelectedCount = count;
        }

        private void PageChanged(object sender, PageChangedEventArgs args)
        {
            NotifyPageChanged?.Invoke(this, args);
        }

        #endregion
    }
}
