using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace LazyTimer.Lib.Wpf.Behaviour
{
    public static class GestureListenerBehaviours
    {
        #region DependencyProperties

        public static readonly DependencyProperty TapCommandProperty =
            DependencyProperty.RegisterAttached("TapCommand", typeof (ICommand), typeof (GestureListenerBehaviours), new PropertyMetadata(OnSetTapCommandCallback));

        public static readonly DependencyProperty DoubleTapCommandProperty =
          DependencyProperty.RegisterAttached("DoubleTapCommand", typeof(ICommand), typeof(GestureListenerBehaviours), new PropertyMetadata(OnSetDoubleTapCommandCallback));

      
        public static readonly DependencyProperty GestureListenerBehavioursBehaviourProperty =
            DependencyProperty.RegisterAttached("GestureListenerBehavioursBehaviour",
                                                typeof (GestureListenerBehavioursBehaviour), typeof (GestureListenerBehaviours), null);

        #endregion

        public static ICommand GetTapCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(TapCommandProperty);
        }

        public static void SetTapCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(TapCommandProperty, value);
        }

        public static ICommand GetDoubleTapCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(DoubleTapCommandProperty);
        }

        public static void SetDoubleTapCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleTapCommandProperty, value);
        }

      

        #region CallBack

        private static void OnSetTapCommandCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var element = dependencyObject as Control;
            if (element != null)
            {
                GestureListenerBehavioursBehaviour behavior = GetOrCreateBehaviour(element);
                behavior.TapCommand = e.NewValue as ICommand;
            }
            else
            {
                Console.Out.WriteLine("Error: Expected type Control but found " + dependencyObject.GetType().Name);
            }
        }

        private static void OnSetDoubleTapCommandCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var element = dependencyObject as Control;
            if (element != null)
            {
                GestureListenerBehavioursBehaviour behavior = GetOrCreateBehaviour(element);
                behavior.DoubleTapCommand = e.NewValue as ICommand;
            }
            else
            {
                Console.Out.WriteLine("Error: Expected type Control but found " + dependencyObject.GetType().Name);
            }
        }


        private static GestureListenerBehavioursBehaviour GetOrCreateBehaviour(Control element)
        {
            var behavior = element.GetValue(GestureListenerBehavioursBehaviourProperty) as GestureListenerBehavioursBehaviour;
            if (behavior == null)
            {
                behavior = new GestureListenerBehavioursBehaviour(element);
                element.SetValue(GestureListenerBehavioursBehaviourProperty, behavior);
            }

            return behavior;
        }

        #endregion

        public class GestureListenerBehavioursBehaviour
        {
            public GestureListenerBehavioursBehaviour(Control element)
            {
                var gestureListener = GestureService.GetGestureListener(element);
                gestureListener.Tap += TapCalled;
                gestureListener.DoubleTap += DoubleTapCalled;
                //element.MouseEnter += new MouseEventHandler(MouseIsOverChanged);
            }

            private void DoubleTapCalled(object sender, GestureEventArgs e)
            {
                if (DoubleTapCommand != null && DoubleTapCommand.CanExecute(sender))
                {
                    DoubleTapCommand.Execute(sender);
                }
            }

            
            private void TapCalled(object sender, GestureEventArgs e)
            {
                if (TapCommand != null && TapCommand.CanExecute(sender))
                {
                    TapCommand.Execute(sender);
                }
            }

            public ICommand TapCommand { get; set; }
            public ICommand DoubleTapCommand { get; set; }
        }
    }
    
    
    
    
    
    
    
    
    
}