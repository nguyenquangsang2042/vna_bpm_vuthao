using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.Components
{
    class ComponentButtonBot : ComponentBase
    {
        UIViewController parentView { get; set; }
        ViewRow control { get; set; }
        public ControlButtonBot btn1, btn2, btn3;
        public List<ButtonAction> lst_moreActions;

        public ComponentButtonBot(UIViewController _parentView, ViewRow _control)
        {
            parentView = _parentView;
            control = _control;
            InitializeComponent();
        }
        int totalButton = 0;
        public override void InitializeComponent()
        {
            totalButton = 0;
            if (control.Elements.Any())
            {
                btn1 = new ControlButtonBot(parentView, control.Elements[0]);
                totalButton++;
                this.Add(btn1);
            }
            else
                btn1 = new ControlButtonBot();

            if (control.Elements.Count < 2)
                btn2 = new ControlButtonBot();
            else
            {

                btn2 = new ControlButtonBot(parentView, control.Elements[1]);
                totalButton++;
                this.Add(btn2);
            }
            if (control.Elements.Count < 3)
                btn3 = new ControlButtonBot();
            else
            {

                btn3 = new ControlButtonBot(parentView, control.Elements[2]);
                ViewElement element_more = new ViewElement { ID = "more", Value = "" };
                btn3 = new ControlButtonBot(parentView, element_more);

                lst_moreActions = new List<ButtonAction>();
                for (int i = 2; i < control.Elements.Count; i++)
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
                totalButton++;
                this.Add(btn3);
            }
        }

        public override void InitializeFrameView(CGRect frame)
        {
            base.InitializeFrameView(frame);
            if(totalButton <= 2) // chia deu ra
            {
                var widthRow = (frame.Width - 50) / totalButton; // 1 space(10) + 20 padding left + 20 padding right
                var spaceBetweenView = 10;

                btn1.InitializeFrameView(new CGRect(30, 0, widthRow, frame.Height));
                btn2.InitializeFrameView(new CGRect(30 + widthRow + spaceBetweenView, 0, widthRow, frame.Height));
                //btn3.InitializeFrameView(new CGRect((widthRow * 2) + (spaceBetweenView * 2), 0, widthRow, frame.Height));
            } else if (totalButton == 3) // 
            {
                var widthRow = (frame.Width - 60 - 30) / 2; // 2 space(10),  20 padding left , 20 padding right, 30 width image
                var spaceBetweenView = 10;

                btn1.InitializeFrameView(new CGRect(30, 0, widthRow, frame.Height));
                btn2.InitializeFrameView(new CGRect(30 + widthRow + spaceBetweenView, 0, widthRow, frame.Height));
                btn3.InitializeFrameView(new CGRect(30 + (widthRow * 2) + (spaceBetweenView * 2), 0, 30, frame.Height));
            }
           
        }

        public override void SetTitle()
        {
            btn1.SetTitle();
            btn2.SetTitle();
            btn3.SetTitle();
        }

        public override void SetValue()
        {
            base.SetValue();

            btn1.SetValue();
            btn2.SetValue();
            btn3.SetValue();
        }

        public override void SetProprety()
        {
            btn1.SetProprety();
            btn2.SetProprety();
            btn3.SetProprety();
        }

        public override void SetEnable()
        {
            base.SetEnable();

            btn1.SetEnable();
            btn2.SetEnable();
            btn3.SetEnable();
        }
    }
}