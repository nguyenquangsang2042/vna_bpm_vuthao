using System;
using System.Collections.Generic;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using UIKit;

namespace BPMOPMobileV1.iOS.IOSClass
{
    public class ButtonActionApp
    {
        AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        public int ID { get; set; }
        public int section { get; set; }
        public string title { get; set; }
        public string titleEN { get; set; }
        public string iconUrl { get; set; }
        public string count { get; set; }
        public bool isSelected { get; set; }
        public bool opption { get; set; }
        public string tinColor { get; set; }
        public string TypeParent { get; set; }

        public void Select(BeanWorkflow workflow, MainViewApp mainViewApp)
        {
            if (TypeParent == typeof(MainViewApp).ToString())
            {
                if (appD.SlideMenuController.MainViewController.GetType() != typeof(MainViewApp))
                {
                    if (CmmVariable.SysConfig.LangCode == "1066")
                        mainViewApp.setContent(workflow.Title, workflow);
                    else
                        mainViewApp.setContent(workflow.TitleEN, workflow);
                    appD.SlideMenuController.ChangeMainViewcontroller(mainViewApp, true);
                }
            }
            else if (TypeParent == typeof(RequestListViewApp).ToString())
            {
                RequestListViewApp requestListViewApp = (RequestListViewApp)mainViewApp.Storyboard.InstantiateViewController("RequestListViewApp");

                if (appD.SlideMenuController.MainViewController.GetType() != typeof(RequestListViewApp))
                {
                    if (CmmVariable.SysConfig.LangCode == "1066")
                        requestListViewApp.setContent(workflow.Title, workflow);
                    else
                        requestListViewApp.setContent(workflow.TitleEN, workflow);
                    appD.SlideMenuController.ChangeMainViewcontroller(requestListViewApp, true);
                }
            }
            else if (TypeParent== typeof(MyRequestListViewApp).ToString())
            {
                MyRequestListViewApp myRequestListViewApp = (MyRequestListViewApp)mainViewApp.Storyboard.InstantiateViewController("MyRequestListViewApp");

                if (CmmVariable.SysConfig.LangCode == "1066")
                    myRequestListViewApp.setContent(workflow.Title, workflow);
                else
                    myRequestListViewApp.setContent(workflow.TitleEN, workflow);

                if (appD.SlideMenuController.MainViewController.GetType() != typeof(MyRequestListViewApp))
                    appD.SlideMenuController.ChangeMainViewcontroller(myRequestListViewApp, true);
            }
            else if (TypeParent == typeof(KanBanView).ToString())
            {
                KanBanView kanBanView = (KanBanView)mainViewApp.Storyboard.InstantiateViewController("KanBanView");

                if (CmmVariable.SysConfig.LangCode == "1066")
                    kanBanView.SetContent(workflow);
                else
                    kanBanView.SetContent(workflow);

                if (appD.SlideMenuController.MainViewController.GetType() != typeof(KanBanView))
                    appD.SlideMenuController.ChangeMainViewcontroller(kanBanView, true);
            }
            else if (TypeParent == typeof(ListView).ToString())
            {
                ListView listView = (ListView)mainViewApp.Storyboard.InstantiateViewController("ListView");

                if (CmmVariable.SysConfig.LangCode == "1066")
                    listView.SetContent(workflow);
                else
                    listView.SetContent(workflow);

                if (appD.SlideMenuController.MainViewController.GetType() != typeof(ListView))
                    appD.SlideMenuController.ChangeMainViewcontroller(listView, true);

            }
        }

        public void SelectMore(MainViewApp mainViewApp, UIViewController parent, List<ButtonActionApp> classMenuOption)
        {
            PresentationDelegate transitioningDelegate;
            CGRect startFrame = new CGRect(UIScreen.MainScreen.Bounds.X, UIScreen.MainScreen.Bounds.Height, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
            CGSize showSize = new CGSize(UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
            ActionMoreApp actionMoreAppMainViewApp = (ActionMoreApp)mainViewApp.Storyboard.InstantiateViewController("ActionMoreApp");
            actionMoreAppMainViewApp.setContent(parent, classMenuOption);
            transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            actionMoreAppMainViewApp.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            actionMoreAppMainViewApp.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            actionMoreAppMainViewApp.TransitioningDelegate = transitioningDelegate;
            parent.PresentViewControllerAsync(actionMoreAppMainViewApp, true);
        }
    }
}
