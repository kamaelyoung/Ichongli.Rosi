using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Thinkwp.Controls
{
    public class ThinkPivot : Pivot
    {
        private PhoneApplicationFrame _currentApplicationFrame;
        private PhoneApplicationPage _currentPage;
        private bool _initilized;

        public ThinkPivot()
        {
            LoadedPivotItem += OnLoadedPivotItem;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!_initilized)
            {
                InitilizeEvent();
                _initilized = true;
            }
        }

        private void CurrentPageOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SetSelectedItemVisible();
        }

        private void FrameOnNavigated(object sender, NavigationEventArgs e)
        {
            string currentPageName = _currentPage.ToString().Split('.').LastOrDefault();
            if (currentPageName != null)
            {
                //从_current页面离开的情况下将当前页面中的的Pivot SelectedItem隐藏
                if (e.NavigationMode == NavigationMode.New)
                {
                    if (CurrentPageAtBackStackTop(currentPageName))
                    {
                        SetSelectedItemCollapse();
                    }
                }
                if (e.NavigationMode == NavigationMode.Back)
                {
                    if (CurrentPageNotExsitInBackStack(currentPageName))
                    {
                        //当前页面不在回退栈的情况分为两种：
                        //1、从当前页面离开
                        //2、回退到当前页面
                        //第一种情况下注销引用
                        if (NotNavigateToCurrentPage(e, currentPageName))
                        {
                            ReleasePageAndFrame();
                        }
                    }
                }
            }
        }

        private static bool NotNavigateToCurrentPage(NavigationEventArgs e, string currentPageName)
        {
            return e.Uri.OriginalString.Contains(currentPageName) == false;
        }

        private bool CurrentPageNotExsitInBackStack(string currentPageName)
        {
            return
                !_currentApplicationFrame.BackStack.Any(
                    journalEntry => journalEntry.Source.OriginalString.Contains(currentPageName));
        }

        private bool CurrentPageAtBackStackTop(string currentPageName)
        {
            JournalEntry backStackTopPage = _currentApplicationFrame.BackStack.FirstOrDefault();
            return (backStackTopPage != null && backStackTopPage.Source.OriginalString.Contains(currentPageName));
        }

        private void SetSelectedItemCollapse()
        {
            if (SelectedItem != null)
            {
                var content = ((PivotItem)SelectedItem).Content;
                if (content != null)
                    ((UIElement)content).Visibility = Visibility.Collapsed;
            }
        }

        private void SetSelectedItemVisible()
        {
            if (SelectedItem != null)
            {
                var content = ((PivotItem)SelectedItem).Content;
                if (content != null)
                    ((UIElement)content).Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        ///  注册Frame和Page的事件以根据Page事件控制自身显示
        /// </summary>
        private void InitilizeEvent()
        {
            _currentApplicationFrame = (Application.Current.RootVisual) as PhoneApplicationFrame;
            if (_currentApplicationFrame != null)
            {
                _currentApplicationFrame.Navigated += FrameOnNavigated;
                _currentPage = _currentApplicationFrame.Content as PhoneApplicationPage;
                if (_currentPage != null)
                    _currentPage.Loaded += CurrentPageOnLoaded;
            }
        }

        /// <summary>
        /// 释放掉引用的资源
        /// </summary>
        private void ReleasePageAndFrame()
        {
            _currentApplicationFrame.Navigated -= FrameOnNavigated;
            _currentApplicationFrame = null;
            _currentPage.Loaded -= CurrentPageOnLoaded;
            _currentPage = null;

        }

        /// <summary>
        ///     PivotItemLoaded之后根据当前所在的PivotItem控制Items的可见行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoadedPivotItem(object sender, PivotItemEventArgs e)
        {
            foreach (object pivotItem in Items)
            {
                if (pivotItem != e.Item)
                //非当前Item则隐藏 
                {
                    if (pivotItem != null)
                    {
                        var content = ((PivotItem)pivotItem).Content;
                        if (content != null)
                            ((UIElement)content).Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    //Item为当前选中的Item则显示
                    if (pivotItem != null)
                    {
                        var content = ((PivotItem)pivotItem).Content;
                        if (content != null)
                            ((UIElement)content).Visibility = Visibility.Visible;
                    }
                }
            }
        }
    }
}
