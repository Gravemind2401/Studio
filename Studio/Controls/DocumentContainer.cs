﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Studio.Controls
{
    public class DocumentContainer : ItemsControl, IDockReceiver
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static DocumentContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentContainer), new FrameworkPropertyMetadata(typeof(DocumentContainer)));
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(DocumentContainer), new PropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty DockCommandProperty =
            DependencyProperty.Register(nameof(DockCommand), typeof(ICommand), typeof(DocumentContainer), new PropertyMetadata((ICommand)null));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public ICommand DockCommand
        {
            get { return (ICommand)GetValue(DockCommandProperty); }
            set { SetValue(DockCommandProperty, value); }
        }

        public DocumentContainer()
        {
            Loaded += DocumentContainer_Loaded;
            Unloaded += DocumentContainer_Unloaded;
        }

        private void DocumentContainer_Loaded(object sender, RoutedEventArgs e)
        {
            DockManager.Register(this);
        }

        private void DocumentContainer_Unloaded(object sender, RoutedEventArgs e)
        {
            DockManager.Unregister(this);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DocumentWell();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DocumentWell;
        }
    }
}
