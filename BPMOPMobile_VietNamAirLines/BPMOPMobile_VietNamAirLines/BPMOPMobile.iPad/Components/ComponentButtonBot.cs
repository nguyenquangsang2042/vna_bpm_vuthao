using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    class ComponentButtonBot : ComponentBase
    {
        UIViewController parentView { get; set; }
        ViewRow control { get; set; }
        public ControlButtonBot btn1, btn2, btn3, btn4;
        public List<ButtonAction> lst_moreActions;

        public ComponentButtonBot(UIViewController _parentView, ViewRow _control)
        {
            parentView = _parentView;
            control = _control;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            //if (control.Elements.Any())
            //    btn1 = new ControlButtonBot(parentView, control.Elements[0]);
            //else
            //    btn1 = new ControlButtonBot();

            //if (control.Elements.Count < 2)
            //    btn2 = new ControlButtonBot();
            //else
            //    btn2 = new ControlButtonBot(parentView, control.Elements[1]);

            //if (control.Elements.Count < 3)
            //    btn3 = new ControlButtonBot();
            //else
            //    btn3 = new ControlButtonBot(parentView, control.Elements[2]);

            //if (control.Elements.Count < 4)
            //    btn4 = new ControlButtonBot();
            //else
            //{
            //    btn4 = new ControlButtonBot(parentView, control.Elements[3]);
            //    ViewElement element_more = new ViewElement { ID = "more", Value = "" };
            //    btn4 = new ControlButtonBot(parentView, element_more);

            //    lst_moreActions = new List<ButtonAction>();
            //    for (int i = 3; i < control.Elements.Count; i++)
            //    {
            //        ButtonAction action = new ButtonAction
            //        {
            //            ID = Convert.ToInt32(control.Elements[i].ID),
            //            Title = control.Elements[i].Title,
            //            Value = control.Elements[i].Value,
            //            Notes = control.Elements[i].Notes
            //        };
            //        lst_moreActions.Add(action);
            //    }
            //}

            if (control.Elements.Count == 1)
            {
                btn1 = new ControlButtonBot(parentView, control.Elements[0]);
                this.Add(btn1);
            }
            else if (control.Elements.Count == 2)
            {
                btn1 = new ControlButtonBot(parentView, control.Elements[0]);
                btn2 = new ControlButtonBot(parentView, control.Elements[1]);
                this.Add(btn1);
                this.Add(btn2);
            }
            else if (control.Elements.Count == 3)
            {
                btn1 = new ControlButtonBot(parentView, control.Elements[0]);
                btn2 = new ControlButtonBot(parentView, control.Elements[1]);
                btn3 = new ControlButtonBot(parentView, control.Elements[2]);
                this.Add(btn1);
                this.Add(btn2);
                this.Add(btn3);
            }
            else if (control.Elements.Count > 3)
            {
                btn4 = new ControlButtonBot(parentView, control.Elements[3]);
                ViewElement element_more = new ViewElement { ID = "more", Value = "" };
                btn1 = new ControlButtonBot(parentView, control.Elements[0]);
                btn2 = new ControlButtonBot(parentView, control.Elements[1]);
                btn3 = new ControlButtonBot(parentView, control.Elements[2]);
                btn4 = new ControlButtonBot(parentView, element_more);

                lst_moreActions = new List<ButtonAction>();
                for (int i = 3; i < control.Elements.Count; i++)
                {
                    ButtonAction action = new ButtonAction
                    {
                        ID = Convert.ToInt32(control.Elements[i].ID),
                        Title = control.Elements[i].Title,
                        Value = control.Elements[i].Value,
                        Notes = control.Elements[i].Notes
                    };
                    lst_moreActions.Add(action);
                }
                this.Add(btn1);
                this.Add(btn2);
                this.Add(btn3);
                this.Add(btn4);
            }
        }

        public override void InitializeFrameView(CGRect frame)
        {
            base.InitializeFrameView(frame);
            var widthRow = (frame.Width - 20) / 4;
            var spaceBetweenView = 5;

            //if (btn1 != null)
            //    btn1.InitializeFrameView(new CGRect(0, 0, widthRow, frame.Height));
            //if (btn2 != null)
            //    btn2.InitializeFrameView(new CGRect(widthRow + spaceBetweenView, 0, widthRow, frame.Height));
            //if (btn3 != null)
            //    btn3.InitializeFrameView(new CGRect((widthRow * 2) + (spaceBetweenView * 2), 0, widthRow, frame.Height));
            //if (btn4 != null)
            //    btn4.InitializeFrameView(new CGRect((widthRow * 3) + (spaceBetweenView * 3), 0, widthRow, frame.Height));

            //sort nguoc lai
            if (control.Elements.Count == 1)
                btn1.InitializeFrameView(new CGRect((widthRow * 3) + (spaceBetweenView * 3), 0, widthRow, frame.Height));
            if (control.Elements.Count == 2)
            {
                btn2.InitializeFrameView(new CGRect((widthRow * 3) + (spaceBetweenView * 3), 0, widthRow, frame.Height));
                btn1.InitializeFrameView(new CGRect((widthRow * 2) + (spaceBetweenView * 2), 0, widthRow, frame.Height));
            }
            if (control.Elements.Count == 3)
            {
                btn3.InitializeFrameView(new CGRect((widthRow * 3) + (spaceBetweenView * 3), 0, widthRow, frame.Height));
                btn2.InitializeFrameView(new CGRect((widthRow * 2) + (spaceBetweenView * 2), 0, widthRow, frame.Height));
                btn1.InitializeFrameView(new CGRect(widthRow + spaceBetweenView, 0, widthRow, frame.Height));
            }
            if (control.Elements.Count > 3)
            {
                btn4.InitializeFrameView(new CGRect((widthRow * 3) + (spaceBetweenView * 3), 0, widthRow, frame.Height));
                btn3.InitializeFrameView(new CGRect((widthRow * 2) + (spaceBetweenView * 2), 0, widthRow, frame.Height));
                btn2.InitializeFrameView(new CGRect(widthRow + spaceBetweenView, 0, widthRow, frame.Height));
                btn1.InitializeFrameView(new CGRect(0, 0, widthRow, frame.Height));
            }
        }

        public override void SetTitle()
        {
            if (btn1 != null)
                btn1.SetTitle();
            if (btn2 != null)
                btn2.SetTitle();
            if (btn3 != null)
                btn3.SetTitle();
            if (btn4 != null)
                btn4.SetTitle();
        }

        public override void SetValue()
        {
            base.SetValue();

            if (btn1 != null)
                btn1.SetValue();
            if (btn2 != null)
                btn2.SetValue();
            if (btn3 != null)
                btn3.SetValue();
            if (btn4 != null)
                btn4.SetValue();
        }

        public override void SetProprety()
        {
            if (btn1 != null)
                btn1.SetProprety();
            if (btn2 != null)
                btn2.SetProprety();
            if (btn3 != null)
                btn3.SetProprety();
            if (btn4 != null)
                btn4.SetProprety();
        }

        public override void SetEnable()
        {
            base.SetEnable();

            if (btn1 != null)
                btn1.SetEnable();
            if (btn2 != null)
                btn2.SetEnable();
            if (btn3 != null)
                btn3.SetEnable();
            if (btn4 != null)
                btn4.SetEnable();
        }

        public override void SetRequire()
        {
            base.SetRequire();

            if (btn1 != null)
                btn1.SetRequire();
            if (btn2 != null)
                btn2.SetRequire();
            if (btn3 != null)
                btn3.SetRequire();
            if (btn4 != null)
                btn4.SetRequire();
        }
    }
}