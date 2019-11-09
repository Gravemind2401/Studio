﻿using Prism.Commands;
using Sandbox.Controls;
using Sandbox.ViewModels;
using Studio.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sandbox.Models
{
    public class TabGroupModel : ModelBase
    {
        private ObservableCollection<TabModel> children = new ObservableCollection<TabModel>();
        public ObservableCollection<TabModel> Children
        {
            get { return children; }
            set { SetProperty(ref children, value, OnCollectionChanged); }
        }

        private readonly TabItemType groupType;
        public TabItemType GroupType => groupType;

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set { SetProperty(ref isActive, value); }
        }

        private bool isWindow;
        public bool IsWindow
        {
            get { return isWindow; }
            internal set { SetProperty(ref isWindow, value); }
        }

        private TabModel selectedItem;
        public TabModel SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value); }
        }

        private double width;
        public double Width
        {
            get { return width; }
            set { SetProperty(ref width, value, UpdateChildrenWidth); }
        }

        private double height;
        public double Height
        {
            get { return height; }
            set { SetProperty(ref height, value, UpdateChildrenHeight); }
        }

        private Dock dock;
        public Dock Dock
        {
            get { return dock; }
            internal set { SetProperty(ref dock, value); }
        }

        public DelegateCommand<TabModel> CloseTabCommand { get; }
        public DelegateCommand<TabModel> TogglePinStatusCommand { get; }
        public DelegateCommand<TabModel> SelectItemCommand { get; }
        public DelegateCommand<FloatEventArgs> FloatTabCommand { get; }
        public DelegateCommand<DockEventArgs> DockCommand { get; }

        public TabGroupModel(TabItemType groupType)
        {
            this.groupType = groupType;

            CloseTabCommand = new DelegateCommand<TabModel>(CloseTabExecuted);
            TogglePinStatusCommand = new DelegateCommand<TabModel>(TogglePinStatusExecuted);
            SelectItemCommand = new DelegateCommand<TabModel>(SelectItemExecuted);
            FloatTabCommand = new DelegateCommand<FloatEventArgs>(FloatTabExecuted);
            DockCommand = new DelegateCommand<DockEventArgs>(DockExecuted);
            Subscribe(children);
        }

        private void CloseTabExecuted(TabModel item)
        {
            var parent = ParentViewModel;
            Children.Remove(item ?? SelectedItem);

            if (parent != null && parent.IsRafted && !parent.AllTabs.Any())
                parent.Host.Close();
        }

        private void TogglePinStatusExecuted(TabModel item)
        {
            if (GroupType == TabItemType.Document)
                item.IsPinned = !item.IsPinned;
            else
            {
                var temp = ParentViewModel;
                foreach (var c in Children.ToList())
                {
                    children.Remove(c);
                    c.IsActive = true;

                    if (Dock == Dock.Left)
                        temp.LeftDockItems.Add(c);
                    else if (Dock == Dock.Top)
                        temp.TopDockItems.Add(c);
                    else if (Dock == Dock.Right)
                        temp.RightDockItems.Add(c);
                    else if (Dock == Dock.Bottom)
                        temp.BottomDockItems.Add(c);
                }
            }
        }

        private void SelectItemExecuted(TabModel item)
        {
            SelectedItem = item;
        }

        private void FloatTabExecuted(FloatEventArgs e)
        {
            var item = e.DataContext as TabModel;
            Children.Remove(item);
            Window wnd;

            if (GroupType == TabItemType.Tool)
            {
                var group = new TabGroupModel(TabItemType.Tool) { IsWindow = true };
                group.Children.Add(item);

                wnd = new ToolWindow
                {
                    Content = group,
                    Left = e.VisualBounds.X,
                    Top = e.VisualBounds.Y,
                    Width = e.VisualBounds.Width,
                    Height = e.VisualBounds.Height
                };
            }
            else
            {
                var model = new WindowViewModel { IsRafted = true };
                var group = new TabGroupModel(TabItemType.Document);
                group.Children.Add(item);
                model.Content = group;

                wnd = new RaftedWindow
                {
                    Model = model,
                    Left = e.VisualBounds.X,
                    Top = e.VisualBounds.Y,
                    Width = e.VisualBounds.Width,
                    Height = e.VisualBounds.Height
                };

                model.Host = wnd;
            }

            if (ParentViewModel != null && ParentViewModel.IsRafted && !ParentViewModel.AllTabs.Any())
                ShowOnClose(ParentViewModel.Host, wnd);
            else
            {
                wnd.Show();
                wnd.DragMove();
            }
        }

        private void DockExecuted(DockEventArgs e)
        {
            var groups = e.SourceContent.OfType<TabGroupModel>().ToList();
            var target = e.TargetIndex as TabModel;
            var index = target == null || target.IsPinned ? 0 : Children.IndexOf(target);

            foreach (var group in groups)
            {
                var allChildren = group.Children.ToList();
                foreach (var item in allChildren)
                {
                    group.Children.Remove(item);
                    item.IsPinned = false;
                    item.IsActive = false;

                    Children.Insert(index, item);
                }
            }

            e.Source.Close();
            IsActive = true;
            SelectedItem = Children[index];
        }

        private void OnCollectionChanged(ObservableCollection<TabModel> prev, ObservableCollection<TabModel> next)
        {
            Unsubscribe(prev);
            Subscribe(next);
        }

        private void Unsubscribe(ObservableCollection<TabModel> collection)
        {
            if (collection == null)
                return;

            foreach (var tool in collection)
                tool.Parent = null;

            collection.CollectionChanged -= Children_CollectionChanged;
        }

        private void Subscribe(ObservableCollection<TabModel> collection)
        {
            if (collection == null)
                return;

            foreach (var tool in collection)
                tool.Parent = this;

            collection.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var tool in e.OldItems.OfType<TabModel>())
                    tool.Parent = null;
            }

            if (e.NewItems != null)
            {
                foreach (var tool in e.NewItems.OfType<TabModel>())
                    tool.Parent = this;
            }

            if (GroupType == TabItemType.Tool && Children.Count == 0)
                Remove();
        }

        public void Remove()
        {
            if (ParentModel != null)
                ParentModel.Remove(this);
            else if (ParentViewModel != null)
                ParentViewModel.Content = null;
        }

        private void UpdateChildrenWidth()
        {
            foreach (var item in Children)
                item.Width = Width;
        }

        private void UpdateChildrenHeight()
        {
            foreach (var item in Children)
                item.Height = Height;
        }

        internal override IEnumerable<TabModel> AllTabs => Children;
    }
}
