using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ArdumotoBot.Remote
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        private bool _isDirty;

        protected bool SetField<T>(ref T field, T value, Expression<Func<T>> propertyExpression)
        {
            return SetField(ref field, value, propertyExpression, true);
        }

        protected bool SetField<T>(ref T field, T value, Expression<Func<T>> propertyExpression, bool makeDirty)
        {
            var changed = !EqualityComparer<T>.Default.Equals(field, value);
            if (changed)
            {
                field = value;
                RaisePropertyChanged(ExtractPropertyName(propertyExpression), makeDirty);
            }
            return changed;
        }

        private static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Expression must be a MemberExpression.", "propertyExpression");
            return memberExpression.Member.Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected void RaisePropertyChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName, true);
        }

        protected virtual void RaisePropertyChanged(string propertyName, bool makesDirty)
        {
            if (makesDirty)
                IsDirty = true;

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set { SetField(ref _isDirty, value, () => IsDirty, false); }
        }

        public static void MarkAllAsNotDirty<T>(IEnumerable<T> enumerable) where T : NotifyPropertyChangedBase
        {
            foreach (var changedBase in enumerable)
            {
                changedBase._isDirty = false;
            }
        }
    }
}